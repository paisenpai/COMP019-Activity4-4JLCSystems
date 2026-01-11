using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP019_Activity4_4JLCSystems.Data;
using COMP019_Activity4_4JLCSystems.Models.Entities;
using COMP019_Activity4_4JLCSystems.Models.ViewModels;

namespace COMP019_Activity4_4JLCSystems.Controllers
{
    [Route("admin/finance")]
    public class FinanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FinanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, string? type, string? category)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Account");

            var inventory = await _context.Inventories
                .Include(i => i.Product)
                .Where(i => i.Product.IsActive)
                .ToListAsync();

            var pendingOrders = await _context.Orders
                .Where(o => o.OrderStatus == "Pending")
                .ToListAsync();

            var cashFlowQuery = _context.CashFlows.AsQueryable();

            if (startDate.HasValue)
            {
                cashFlowQuery = cashFlowQuery.Where(cf => cf.TransactionDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                cashFlowQuery = cashFlowQuery.Where(cf => cf.TransactionDate <= endDate.Value.AddDays(1));
            }
            if (!string.IsNullOrEmpty(type))
            {
                cashFlowQuery = cashFlowQuery.Where(cf => cf.TransactionType == type);
            }
            if (!string.IsNullOrEmpty(category))
            {
                cashFlowQuery = cashFlowQuery.Where(cf => cf.Category == category);
            }

            var cashFlows = await cashFlowQuery
                .OrderByDescending(cf => cf.TransactionDate)
                .ToListAsync();

            // Get all cash flows for summary totals (without date filter for overall totals)
            var allCashFlows = await _context.CashFlows.ToListAsync();

            var viewModel = new CashFlowSummaryViewModel
            {
                // Inventory value (capital)
                TotalInventoryValue = inventory.Sum(i => i.Product.SellingPrice * i.QuantityInStock),
                TotalInventoryCost = inventory.Sum(i => i.Product.CostPrice * i.QuantityInStock),

                // Pending payments
                PendingPayments = pendingOrders.Sum(o => o.TotalAmount),
                PendingOrderCount = pendingOrders.Count,

                // Total money in/out
                TotalMoneyIn = allCashFlows.Where(cf => cf.TransactionType == "Income").Sum(cf => cf.Amount),
                TotalMoneyOut = allCashFlows.Where(cf => cf.TransactionType == "Expense").Sum(cf => cf.Amount),

                // Category breakdown
                SalesIncome = allCashFlows.Where(cf => cf.TransactionType == "Income" && cf.Category == "Sales").Sum(cf => cf.Amount),
                OtherIncome = allCashFlows.Where(cf => cf.TransactionType == "Income" && cf.Category != "Sales").Sum(cf => cf.Amount),
                LogisticsExpense = allCashFlows.Where(cf => cf.TransactionType == "Expense" && cf.Category == "Logistics").Sum(cf => cf.Amount),
                ShippingExpense = allCashFlows.Where(cf => cf.TransactionType == "Expense" && cf.Category == "Shipping").Sum(cf => cf.Amount),
                PurchaseExpense = allCashFlows.Where(cf => cf.TransactionType == "Expense" && cf.Category == "Purchase").Sum(cf => cf.Amount),
                OtherExpense = allCashFlows.Where(cf => cf.TransactionType == "Expense" && 
                    cf.Category != "Logistics" && cf.Category != "Shipping" && cf.Category != "Purchase").Sum(cf => cf.Amount),

                // Recent transactions (filtered)
                RecentTransactions = cashFlows.Take(50).Select(cf => new CashFlowItemViewModel
                {
                    CashFlowId = cf.CashFlowId,
                    TransactionDate = cf.TransactionDate,
                    TransactionType = cf.TransactionType,
                    Category = cf.Category,
                    Description = cf.Description,
                    Amount = cf.Amount,
                    ReferenceNumber = cf.ReferenceNumber,
                    Notes = cf.Notes,
                    OrderId = cf.OrderId,
                    ShipmentId = cf.ShipmentId
                }).ToList(),

                StartDate = startDate,
                EndDate = endDate,
                TransactionType = type,
                Category = category
            };

            return View(viewModel);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            var model = new CreateCashFlowViewModel
            {
                TransactionDate = DateTime.Now
            };
            return View(model);
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCashFlowViewModel model)
        {
            if (ModelState.IsValid)
            {
                var cashFlow = new CashFlow
                {
                    TransactionDate = model.TransactionDate,
                    TransactionType = model.TransactionType,
                    Category = model.Category,
                    Description = model.Description,
                    Amount = model.Amount,
                    ReferenceNumber = model.ReferenceNumber ?? $"MAN-{DateTime.Now:yyyyMMddHHmmss}",
                    Notes = model.Notes,
                    CreatedDate = DateTime.Now
                };

                _context.CashFlows.Add(cashFlow);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"{model.TransactionType} of ?{model.Amount:N2} recorded successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashFlow = await _context.CashFlows
                .Include(cf => cf.Order)
                .Include(cf => cf.Shipment)
                .FirstOrDefaultAsync(cf => cf.CashFlowId == id);

            if (cashFlow == null)
            {
                return NotFound();
            }

            return View(cashFlow);
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cashFlow = await _context.CashFlows.FindAsync(id);
            if (cashFlow == null)
            {
                return NotFound();
            }

            return View(cashFlow);
        }

        [HttpPost("delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cashFlow = await _context.CashFlows.FindAsync(id);
            if (cashFlow != null)
            {
                _context.CashFlows.Remove(cashFlow);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cash flow entry deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("income")]
        public IActionResult Income()
        {
            return RedirectToAction(nameof(Index), new { type = "Income" });
        }

        [HttpGet("expenses")]
        public IActionResult Expenses()
        {
            return RedirectToAction(nameof(Index), new { type = "Expense" });
        }
    }
}
