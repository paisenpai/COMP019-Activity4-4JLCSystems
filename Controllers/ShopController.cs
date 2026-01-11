using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP019_Activity4_4JLCSystems.Data;
using COMP019_Activity4_4JLCSystems.Models.Entities;
using COMP019_Activity4_4JLCSystems.Models.ViewModels;

namespace COMP019_Activity4_4JLCSystems.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetSessionId()
        {
            var sessionId = HttpContext.Session.GetString("CartSessionId");
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("CartSessionId", sessionId);
            }
            return sessionId;
        }

        [Route("shop")]
        public async Task<IActionResult> Index(string? category, string? search)
        {
            var query = _context.Products
                .Include(p => p.Inventory)
                .Where(p => p.IsActive && p.Inventory != null && p.Inventory.QuantityInStock > 0)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search) || 
                                         (p.Brand != null && p.Brand.Contains(search)));
            }

            var products = await query.ToListAsync();
            var categories = await _context.Products
                .Where(p => p.IsActive && p.Category != null)
                .Select(p => p.Category!)
                .Distinct()
                .ToListAsync();

            ViewBag.Categories = categories;
            ViewBag.SelectedCategory = category;
            ViewBag.SearchTerm = search;

            return View(products);
        }

        [Route("cart")]
        public async Task<IActionResult> Cart()
        {
            var sessionId = GetSessionId();
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p!.Inventory)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);

            return View(cart);
        }

        [HttpPost]
        [Route("cart/add")]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var sessionId = GetSessionId();
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);

            if (cart == null)
            {
                cart = new Cart
                {
                    SessionId = sessionId,
                    CreatedDate = DateTime.Now,
                    LastUpdated = DateTime.Now
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    DateAdded = DateTime.Now
                });
            }

            cart.LastUpdated = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Item added to cart!";
            return Redirect("/cart");
        }

        [HttpPost]
        [Route("cart/update")]
        public async Task<IActionResult> UpdateCart(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Product)
                    .ThenInclude(p => p!.Inventory)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);
                
            if (cartItem != null)
            {
                if (quantity <= 0)
                {
                    _context.CartItems.Remove(cartItem);
                }
                else
                {
                    // Validate against stock
                    var maxStock = cartItem.Product?.Inventory?.QuantityInStock ?? 0;
                    cartItem.Quantity = Math.Min(quantity, maxStock > 0 ? maxStock : quantity);
                }
                await _context.SaveChangesAsync();
            }
            return Redirect("/cart");
        }

        [HttpPost]
        [Route("cart/remove")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return Redirect("/cart");
        }

        [Route("order")]
        public async Task<IActionResult> Checkout()
        {
            var sessionId = GetSessionId();
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return Redirect("/shop");
            }

            var viewModel = new CheckoutViewModel
            {
                CartItems = cart.CartItems.Select(ci => new CartItemViewModel
                {
                    CartItemId = ci.CartItemId,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product?.ProductName ?? "Unknown",
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product?.SellingPrice ?? 0
                }).ToList(),
                Subtotal = cart.CartItems.Sum(ci => (ci.Product?.SellingPrice ?? 0) * ci.Quantity),
                ShippingFee = 50.00m
            };

            return View("Checkout", viewModel);
        }

        [HttpPost]
        [Route("payment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(string CustomerName, string ShippingAddress, string? ContactNumber, decimal ShippingFee, string? Notes)
        {
            var sessionId = GetSessionId();
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p!.Inventory)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return Redirect("/shop");
            }
            
            if (string.IsNullOrWhiteSpace(CustomerName) || string.IsNullOrWhiteSpace(ShippingAddress))
            {
                TempData["Error"] = "Please fill in all required fields.";
                return Redirect("/order");
            }

            // Create order
            var order = new Order
            {
                OrderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{DateTime.Now:HHmmss}",
                OrderDate = DateTime.Now,
                CustomerName = CustomerName,
                ShippingAddress = ShippingAddress,
                ContactNumber = ContactNumber,
                Subtotal = cart.CartItems.Sum(ci => (ci.Product?.SellingPrice ?? 0) * ci.Quantity),
                ShippingFee = ShippingFee,
                TotalAmount = cart.CartItems.Sum(ci => (ci.Product?.SellingPrice ?? 0) * ci.Quantity) + ShippingFee,
                OrderStatus = "Pending",
                Notes = Notes
            };

            // Add order items
            foreach (var item in cart.CartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product?.ProductName ?? "Unknown",
                    Quantity = item.Quantity,
                    UnitPrice = item.Product?.SellingPrice ?? 0,
                    UnitCost = item.Product?.CostPrice ?? 0
                });

                // Deduct from inventory
                var inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.ProductId == item.ProductId);
                if (inventory != null)
                {
                    inventory.QuantityInStock -= item.Quantity;
                    inventory.LastUpdated = DateTime.Now;
                }
            }

            _context.Orders.Add(order);

            // Clear cart
            _context.CartItems.RemoveRange(cart.CartItems);
            _context.Carts.Remove(cart);

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Order {order.OrderNumber} placed successfully!";
            return RedirectToAction("OrderConfirmation", new { orderNumber = order.OrderNumber });
        }

        [Route("order/confirmation/{orderNumber}")]
        public async Task<IActionResult> OrderConfirmation(string orderNumber)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [Route("track")]
        public IActionResult TrackOrder()
        {
            return View();
        }

        [HttpPost]
        [Route("track")]
        public async Task<IActionResult> TrackOrder(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                ModelState.AddModelError("", "Please enter an order number.");
                return View();
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
            {
                ModelState.AddModelError("", "Order not found. Please check the order number.");
                return View();
            }

            return View("TrackOrderResult", order);
        }
    }
}
