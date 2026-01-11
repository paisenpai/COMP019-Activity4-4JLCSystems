using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP019_Activity4_4JLCSystems.Data;
using COMP019_Activity4_4JLCSystems.Models.ViewModels;

namespace COMP019_Activity4_4JLCSystems.Controllers
{
    [Route("admin/dashboard")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Account");

            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

            var paidOrders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.OrderStatus == "Paid" || o.OrderStatus == "Shipped" || o.OrderStatus == "Delivered")
                .ToListAsync();

            var allOrders = await _context.Orders.ToListAsync();

            var inventory = await _context.Inventories
                .Include(i => i.Product)
                .ToListAsync();

            var shipments = await _context.Shipments
                .Include(s => s.ShipmentItems)
                .ToListAsync();

            // Get cash flow data
            var cashFlows = await _context.CashFlows.ToListAsync();

            // Calculate dashboard metrics
            var viewModel = new DashboardViewModel
            {
                // Sales Metrics - Only count paid orders
                TotalSales = paidOrders.Sum(o => o.Subtotal),
                SalesThisMonth = paidOrders.Where(o => o.PaymentDate >= firstDayOfMonth).Sum(o => o.Subtotal),
                SalesToday = paidOrders.Where(o => o.PaymentDate?.Date == today).Sum(o => o.Subtotal),

                // Order Metrics
                TotalOrders = allOrders.Count,
                PendingOrders = allOrders.Count(o => o.OrderStatus == "Pending"),
                PaidOrders = allOrders.Count(o => o.OrderStatus == "Paid"),
                ShippedOrders = allOrders.Count(o => o.OrderStatus == "Shipped"),
                DeliveredOrders = allOrders.Count(o => o.OrderStatus == "Delivered"),

                // Profit Calculation: Profit = Total Sales - Item Cost - Shipping Fees
                TotalItemCosts = paidOrders.SelectMany(o => o.OrderItems).Sum(oi => oi.UnitCost * oi.Quantity),
                TotalShippingFeesDeducted = paidOrders.Sum(o => o.ShippingFee),
                TotalProfit = paidOrders.Sum(o => o.Subtotal) 
                    - paidOrders.SelectMany(o => o.OrderItems).Sum(oi => oi.UnitCost * oi.Quantity) 
                    - paidOrders.Sum(o => o.ShippingFee),
                ProfitThisMonth = paidOrders.Where(o => o.PaymentDate >= firstDayOfMonth)
                    .Sum(o => o.Subtotal - o.OrderItems.Sum(oi => oi.UnitCost * oi.Quantity) - o.ShippingFee),

                // Shipment/Logistics Metrics
                TotalShipments = shipments.Count,
                PendingShipments = shipments.Count(s => s.Status == "Pending" || s.Status == "In Transit"),
                ReceivedShipments = shipments.Count(s => s.Status == "Received"),

                // Inventory Metrics
                TotalProducts = inventory.Count,
                LowStockItems = inventory.Count(i => i.IsLowStock),
                OutOfStockItems = inventory.Count(i => i.IsOutOfStock),
                TotalInventoryValue = inventory.Sum(i => i.Product.SellingPrice * i.QuantityInStock),

                // Cash Flow Metrics
                TotalMoneyIn = cashFlows.Where(cf => cf.TransactionType == "Income").Sum(cf => cf.Amount),
                TotalMoneyOut = cashFlows.Where(cf => cf.TransactionType == "Expense").Sum(cf => cf.Amount),

                // Recent Orders (last 5)
                RecentOrders = allOrders
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .Select(o => new RecentOrderViewModel
                    {
                        OrderId = o.OrderId,
                        OrderNumber = o.OrderNumber,
                        OrderDate = o.OrderDate,
                        CustomerName = o.CustomerName,
                        TotalAmount = o.TotalAmount,
                        Status = o.OrderStatus
                    }).ToList(),

                // Recent Shipments (last 5)
                RecentShipments = shipments
                    .OrderByDescending(s => s.OrderDate)
                    .Take(5)
                    .Select(s => new RecentShipmentViewModel
                    {
                        ShipmentId = s.ShipmentId,
                        ShipmentNumber = s.ShipmentNumber,
                        OrderDate = s.OrderDate,
                        StoreSource = s.StoreSource,
                        TotalItems = s.ShipmentItems.Sum(si => si.Quantity),
                        Status = s.Status
                    }).ToList(),

                // Low Stock Products
                LowStockProducts = inventory
                    .Where(i => i.IsLowStock || i.IsOutOfStock)
                    .OrderBy(i => i.QuantityInStock)
                    .Take(10)
                    .Select(i => new LowStockItemViewModel
                    {
                        ProductId = i.ProductId,
                        ProductName = i.Product.ProductName,
                        ItemCode = i.Product.ItemCode,
                        QuantityInStock = i.QuantityInStock,
                        ReorderLevel = i.ReorderLevel,
                        StockStatus = i.StockStatus
                    }).ToList()
            };

            return View(viewModel);
        }
    }
}
