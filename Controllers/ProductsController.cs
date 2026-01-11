using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP019_Activity4_4JLCSystems.Data;
using COMP019_Activity4_4JLCSystems.Models.Entities;
using COMP019_Activity4_4JLCSystems.Models.ViewModels;

namespace COMP019_Activity4_4JLCSystems.Controllers
{
    /// 
    /// ProductsController - Manages product catalog
    /// CRUD operations for products with inventory management
    /// Admin access only
    /// 
    public class ProductsController : Controller
    {
        // Dependency Injection - ApplicationDbContext
        private readonly ApplicationDbContext _context;

        // Constructor injection
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Check if user is admin
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        /// 
        /// GET: Products/Index
        /// Lists all products with filtering options
        /// Admin access only
        /// 
        [HttpGet]
        public async Task<IActionResult> Index(string? category, string? stockFilter, string? search)
        {
            // Check admin access
            if (!IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var query = _context.Products
                .Include(p => p.Inventory)
                .Where(p => p.IsActive)
                .AsQueryable();

            // Apply category filter
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search) || 
                                         p.ItemCode.Contains(search) ||
                                         (p.Brand != null && p.Brand.Contains(search)));
            }

            var products = await query.ToListAsync();

            // Apply stock filter after fetching (for computed properties)
            var productList = products.Select(p => new ProductDisplayViewModel
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Brand = p.Brand,
                ItemCode = p.ItemCode,
                Category = p.Category,
                Description = p.Description,
                ImageUrl = p.ImageUrl,
                CostPrice = p.CostPrice,
                SellingPrice = p.SellingPrice,
                IsActive = p.IsActive,
                DateAdded = p.DateAdded,
                QuantityInStock = p.Inventory?.QuantityInStock ?? 0,
                ReorderLevel = p.Inventory?.ReorderLevel ?? 0,
                IsLowStock = p.Inventory?.IsLowStock ?? false,
                IsOutOfStock = p.Inventory?.IsOutOfStock ?? true,
                StockStatus = p.Inventory?.StockStatus ?? "No Inventory"
            }).ToList();

            // Apply stock filter
            if (!string.IsNullOrEmpty(stockFilter))
            {
                productList = stockFilter switch
                {
                    "InStock" => productList.Where(p => !p.IsLowStock && !p.IsOutOfStock).ToList(),
                    "LowStock" => productList.Where(p => p.IsLowStock).ToList(),
                    "OutOfStock" => productList.Where(p => p.IsOutOfStock).ToList(),
                    _ => productList
                };
            }

            // Get available categories for filter dropdown
            var categories = await _context.Products
                .Where(p => p.Category != null)
                .Select(p => p.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            var viewModel = new ProductListViewModel
            {
                Products = productList.OrderBy(p => p.ProductName).ToList(),
                CategoryFilter = category,
                StockFilter = stockFilter,
                SearchTerm = search,
                AvailableCategories = categories
            };

            return View(viewModel);
        }

        ///
        /// GET: Products/Details/5
        /// Displays detailed product information
        /// 
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductDisplayViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Brand = product.Brand,
                ItemCode = product.ItemCode,
                Category = product.Category,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                CostPrice = product.CostPrice,
                SellingPrice = product.SellingPrice,
                IsActive = product.IsActive,
                DateAdded = product.DateAdded,
                QuantityInStock = product.Inventory?.QuantityInStock ?? 0,
                ReorderLevel = product.Inventory?.ReorderLevel ?? 0,
                IsLowStock = product.Inventory?.IsLowStock ?? false,
                IsOutOfStock = product.Inventory?.IsOutOfStock ?? true,
                StockStatus = product.Inventory?.StockStatus ?? "No Inventory"
            };

            return View(viewModel);
        }

        /// 
        /// GET: Products/Create
        /// Displays form to create a new product
        /// 
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateProductViewModel());
        }

        /// 
        /// POST: Products/Create
        /// Creates a new product with initial inventory
        /// 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if item code already exists
                if (await _context.Products.AnyAsync(p => p.ItemCode == model.ItemCode))
                {
                    ModelState.AddModelError("ItemCode", "This item code already exists.");
                    return View(model);
                }

                // Create the product
                var product = new Product
                {
                    ProductName = model.ProductName,
                    Brand = model.Brand,
                    ItemCode = model.ItemCode,
                    Category = model.Category,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl,
                    CostPrice = model.CostPrice,
                    SellingPrice = model.SellingPrice,
                    DateAdded = DateTime.Now,
                    IsActive = true
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Create inventory record for the product
                var inventory = new Inventory
                {
                    ProductId = product.ProductId,
                    QuantityInStock = model.InitialStock,
                    ReorderLevel = model.ReorderLevel,
                    LastUpdated = DateTime.Now
                };

                _context.Inventories.Add(inventory);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Product '{product.ProductName}' created successfully with {model.InitialStock} units in stock.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        /// 
        /// GET: Products/Edit/5
        /// Displays form to edit a product
        /// 
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new EditProductViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Brand = product.Brand,
                ItemCode = product.ItemCode,
                Category = product.Category,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                CostPrice = product.CostPrice,
                SellingPrice = product.SellingPrice,
                IsActive = product.IsActive
            };

            return View(viewModel);
        }

        /// 
        /// POST: Products/Edit/5
        /// Updates an existing product
        /// 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditProductViewModel model)
        {
            if (id != model.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check if item code already exists for different product
                if (await _context.Products.AnyAsync(p => p.ItemCode == model.ItemCode && p.ProductId != id))
                {
                    ModelState.AddModelError("ItemCode", "This item code is already used by another product.");
                    return View(model);
                }

                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                // Update product properties
                product.ProductName = model.ProductName;
                product.Brand = model.Brand;
                product.ItemCode = model.ItemCode;
                product.Category = model.Category;
                product.Description = model.Description;
                product.ImageUrl = model.ImageUrl;
                product.CostPrice = model.CostPrice;
                product.SellingPrice = model.SellingPrice;
                product.IsActive = model.IsActive;

                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Product '{product.ProductName}' updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(id))
                    {
                        return NotFound();
                    }
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        /// 
        /// GET: Products/Delete/5
        /// Displays confirmation for product deletion
        /// 
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        /// 
        /// POST: Products/Delete/5
        /// Deletes a product (soft delete by setting IsActive = false)
        /// 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
              
                product.IsActive = false;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Product '{product.ProductName}' has been deactivated.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
