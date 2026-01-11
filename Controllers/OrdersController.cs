using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP019_Activity4_4JLCSystems.Data;
using COMP019_Activity4_4JLCSystems.Models.Entities;
using COMP019_Activity4_4JLCSystems.Models.ViewModels;

namespace COMP019_Activity4_4JLCSystems.Controllers
{
    [Route("admin/orders")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> Index(string? status, DateTime? startDate, DateTime? endDate, string? search, int page = 1)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .AsQueryable();

            // Apply status filter
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.OrderStatus == status);
            }

            // Apply date filters
            if (startDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= endDate.Value.AddDays(1));
            }

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o => o.OrderNumber.Contains(search) ||
                                         (o.CustomerName != null && o.CustomerName.Contains(search)));
            }

            // Pagination
            int pageSize = 10;
            var totalOrders = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalOrders / (double)pageSize);

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new OrderListViewModel
            {
                Orders = orders,
                StatusFilter = status,
                StartDate = startDate,
                EndDate = endDate,
                SearchTerm = search,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(viewModel);
        }


        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            var viewModel = new OrderDetailsViewModel
            {
                Order = order,
                OrderItems = order.OrderItems.ToList()
            };

            return View(viewModel);
        }

        [HttpGet("processpayment/{id}")]
        public async Task<IActionResult> ProcessPayment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.OrderStatus != "Pending")
            {
                TempData["Error"] = "This order has already been processed.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var viewModel = new ProcessPaymentViewModel
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                TotalAmount = order.TotalAmount
            };

            return View(viewModel);
        }

        [HttpPost("processpayment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(ProcessPaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.OrderId == model.OrderId);

                if (order == null)
                {
                    return NotFound();
                }

                if (order.OrderStatus != "Pending")
                {
                    TempData["Error"] = "This order has already been processed.";
                    return RedirectToAction(nameof(Details), new { id = model.OrderId });
                }

                // Update order status to Paid
                order.OrderStatus = "Paid";
                order.PaymentDate = DateTime.Now;
                order.PaymentMethod = model.PaymentMethod;
                if (!string.IsNullOrEmpty(model.PaymentNotes))
                {
                    order.Notes = string.IsNullOrEmpty(order.Notes) 
                        ? $"Payment: {model.PaymentNotes}" 
                        : $"{order.Notes}\nPayment: {model.PaymentNotes}";
                }

                // Record income in Cash Flow
                var cashFlow = new CashFlow
                {
                    TransactionDate = DateTime.Now,
                    TransactionType = "Income",
                    Category = "Sales",
                    Description = $"Payment received for Order {order.OrderNumber}",
                    Amount = order.TotalAmount,
                    ReferenceNumber = order.OrderNumber,
                    OrderId = order.OrderId,
                    Notes = $"Customer: {order.CustomerName}, Payment Method: {model.PaymentMethod}"
                };
                _context.CashFlows.Add(cashFlow);

                await _context.SaveChangesAsync();

                // Calculate profit for display
                decimal totalCost = order.OrderItems.Sum(oi => oi.UnitCost * oi.Quantity);
                decimal profit = order.Subtotal - totalCost - order.ShippingFee;

                TempData["Success"] = $"Payment processed successfully! Order {order.OrderNumber} is now Paid. Profit: ?{profit:N2}";
                return RedirectToAction(nameof(Details), new { id = model.OrderId });
            }


            return View(model);
        }

        [HttpPost("updatestatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Inventory)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            var validStatuses = new[] { "Pending", "Paid", "Shipped", "Delivered", "Cancelled" };
            if (!validStatuses.Contains(status))
            {
                TempData["Error"] = "Invalid status.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (status == "Cancelled" && order.OrderStatus == "Pending")
            {
                foreach (var item in order.OrderItems)
                {
                    if (item.Product?.Inventory != null)
                    {
                        item.Product.Inventory.QuantityInStock += item.Quantity;
                        item.Product.Inventory.LastUpdated = DateTime.Now;
                    }
                }
                TempData["Success"] = $"Order {order.OrderNumber} cancelled. Inventory restored.";
            }
            else
            {
                TempData["Success"] = $"Order {order.OrderNumber} status updated to {status}.";
            }

            order.OrderStatus = status;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet("pending")]
        public async Task<IActionResult> Pending()
        {
            var pendingOrders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.OrderStatus == "Pending")
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var viewModel = new OrderListViewModel
            {
                Orders = pendingOrders,
                StatusFilter = "Pending"
            };

            return View("Index", viewModel);
        }

        /// GET: Orders/Delete/5
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost("delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Inventory)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            // Only allow deletion of Pending or Cancelled orders
            if (order.OrderStatus != "Pending" && order.OrderStatus != "Cancelled")
            {
                TempData["Error"] = "Cannot delete orders that have been paid, shipped, or delivered.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // If pending, restore inventory
            if (order.OrderStatus == "Pending")
            {
                foreach (var item in order.OrderItems)
                {
                    if (item.Product?.Inventory != null)
                    {
                        item.Product.Inventory.QuantityInStock += item.Quantity;
                        item.Product.Inventory.LastUpdated = DateTime.Now;
                    }
                }
            }

            _context.OrderItems.RemoveRange(order.OrderItems);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Order {order.OrderNumber} deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
