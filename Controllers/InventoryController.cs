using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP019_Activity4_4JLCSystems.Data;
using COMP019_Activity4_4JLCSystems.Models.Entities;
using COMP019_Activity4_4JLCSystems.Models.ViewModels;

namespace COMP019_Activity4_4JLCSystems.Controllers
{
    [Route("admin/inventory")]
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> Index(string? category, string? stockFilter, string? search)
        {
            var query = _context.Inventories
                .Include(i => i.Product)
                .Where(i => i.Product.IsActive)
                .AsQueryable();

            // Apply category filter
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(i => i.Product.Category == category);
            }

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(i => i.Product.ProductName.Contains(search) ||
                                         i.Product.ItemCode.Contains(search) ||
                                         (i.Product.Brand != null && i.Product.Brand.Contains(search)));
            }

            var inventoryItems = await query.ToListAsync();

            // Map to view model
            var itemList = inventoryItems.Select(i => new InventoryItemViewModel
            {
                InventoryId = i.InventoryId,
                ProductId = i.ProductId,
                ProductName = i.Product.ProductName,
                Brand = i.Product.Brand,
                ItemCode = i.Product.ItemCode,
                Category = i.Product.Category,
                ImageUrl = i.Product.ImageUrl,
                CostPrice = i.Product.CostPrice,
                SellingPrice = i.Product.SellingPrice,
                QuantityInStock = i.QuantityInStock,
                ReorderLevel = i.ReorderLevel,
                LastUpdated = i.LastUpdated,
                IsLowStock = i.IsLowStock,
                IsOutOfStock = i.IsOutOfStock,
                StockStatus = i.StockStatus
            }).ToList();

            // Apply stock filter
            if (!string.IsNullOrEmpty(stockFilter))
            {
                itemList = stockFilter switch
                {
                    "InStock" => itemList.Where(i => !i.IsLowStock && !i.IsOutOfStock).ToList(),
                    "LowStock" => itemList.Where(i => i.IsLowStock).ToList(),
                    "OutOfStock" => itemList.Where(i => i.IsOutOfStock).ToList(),
                    _ => itemList
                };
            }

            // Get available categories for filter dropdown
            var categories = await _context.Products
                .Where(p => p.Category != null && p.IsActive)
                .Select(p => p.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            var viewModel = new InventoryListViewModel
            {
                Items = itemList.OrderBy(i => i.ProductName).ToList(),
                CategoryFilter = category,
                StockFilter = stockFilter,
                SearchTerm = search,
                AvailableCategories = categories,
                TotalProducts = itemList.Count,
                InStockCount = itemList.Count(i => !i.IsLowStock && !i.IsOutOfStock),
                LowStockCount = itemList.Count(i => i.IsLowStock),
                OutOfStockCount = itemList.Count(i => i.IsOutOfStock),
                TotalInventoryValue = itemList.Sum(i => i.TotalValue),
                TotalInventoryCost = itemList.Sum(i => i.TotalCost)
            };

            return View(viewModel);
        }

        [HttpGet("adjust/{id}")]
        public async Task<IActionResult> Adjust(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.Inventories
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.InventoryId == id);

            if (inventory == null)
            {
                return NotFound();
            }

            var viewModel = new UpdateInventoryViewModel
            {
                InventoryId = inventory.InventoryId,
                ProductId = inventory.ProductId,
                ProductName = inventory.Product.ProductName,
                ItemCode = inventory.Product.ItemCode,
                CurrentQuantity = inventory.QuantityInStock,
                ReorderLevel = inventory.ReorderLevel
            };

            return View(viewModel);
        }

        [HttpPost("adjust")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adjust(UpdateInventoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var inventory = await _context.Inventories
                    .Include(i => i.Product)
                    .FirstOrDefaultAsync(i => i.InventoryId == model.InventoryId);

                if (inventory == null)
                {
                    return NotFound();
                }

                int previousQuantity = inventory.QuantityInStock;
                int newQuantity = model.AdjustmentType switch
                {
                    "Add" => inventory.QuantityInStock + model.Quantity,
                    "Remove" => Math.Max(0, inventory.QuantityInStock - model.Quantity),
                    "Set" => model.Quantity,
                    _ => inventory.QuantityInStock
                };

                inventory.QuantityInStock = newQuantity;
                inventory.ReorderLevel = model.ReorderLevel;
                inventory.LastUpdated = DateTime.Now;

                await _context.SaveChangesAsync();

                // Log the adjustment in cash flow if it's a significant change
                if (model.AdjustmentType == "Add" && model.Reason?.Contains("Purchase", StringComparison.OrdinalIgnoreCase) == true)
                {
                    var cashFlow = new CashFlow
                    {
                        TransactionDate = DateTime.Now,
                        TransactionType = "Expense",
                        Category = "Purchase",
                        Description = $"Stock adjustment for {inventory.Product.ProductName}: Added {model.Quantity} units. {model.Reason}",
                        Amount = inventory.Product.CostPrice * model.Quantity,
                        ReferenceNumber = $"ADJ-{DateTime.Now:yyyyMMddHHmmss}",
                        Notes = model.Reason
                    };
                    _context.CashFlows.Add(cashFlow);
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = $"Inventory for '{inventory.Product.ProductName}' adjusted from {previousQuantity} to {newQuantity} units.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        /// 
        /// GET: Inventory/LowStock
        /// Shows only low stock and out of stock items
        /// 
        [HttpGet("lowstock")]
        public async Task<IActionResult> LowStock()
        {
            var lowStockItems = await _context.Inventories
                .Include(i => i.Product)
                .Where(i => i.Product.IsActive && i.QuantityInStock <= i.ReorderLevel)
                .OrderBy(i => i.QuantityInStock)
                .Select(i => new InventoryItemViewModel
                {
                    InventoryId = i.InventoryId,
                    ProductId = i.ProductId,
                    ProductName = i.Product.ProductName,
                    Brand = i.Product.Brand,
                    ItemCode = i.Product.ItemCode,
                    Category = i.Product.Category,
                    ImageUrl = i.Product.ImageUrl,
                    CostPrice = i.Product.CostPrice,
                    SellingPrice = i.Product.SellingPrice,
                    QuantityInStock = i.QuantityInStock,
                    ReorderLevel = i.ReorderLevel,
                    LastUpdated = i.LastUpdated,
                    IsLowStock = i.QuantityInStock > 0 && i.QuantityInStock <= i.ReorderLevel,
                    IsOutOfStock = i.QuantityInStock <= 0,
                    StockStatus = i.QuantityInStock <= 0 ? "Out of Stock" : "Low Stock"
                })
                .ToListAsync();

            return View(lowStockItems);
        }
    }
}
