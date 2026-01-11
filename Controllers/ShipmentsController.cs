using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP019_Activity4_4JLCSystems.Data;
using COMP019_Activity4_4JLCSystems.Models.Entities;
using COMP019_Activity4_4JLCSystems.Models.ViewModels;

namespace COMP019_Activity4_4JLCSystems.Controllers
{
    [Route("admin/shipments")]
    public class ShipmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShipmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> Index(string? status, DateTime? startDate, DateTime? endDate, string? search)
        {
            var query = _context.Shipments
                .Include(s => s.ShipmentItems)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(s => s.Status == status);
            }

            if (startDate.HasValue)
            {
                query = query.Where(s => s.OrderDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(s => s.OrderDate <= endDate.Value.AddDays(1));
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s => s.ShipmentNumber.Contains(search) ||
                                         s.StoreSource.Contains(search));
            }

            var shipments = await query
                .OrderByDescending(s => s.OrderDate)
                .ToListAsync();

            var viewModel = new ShipmentListViewModel
            {
                Shipments = shipments.Select(s => new ShipmentDisplayViewModel
                {
                    ShipmentId = s.ShipmentId,
                    ShipmentNumber = s.ShipmentNumber,
                    StoreSource = s.StoreSource,
                    OrderDate = s.OrderDate,
                    ExpectedArrival = s.ExpectedArrival,
                    ReceivedDate = s.ReceivedDate,
                    TotalShippingFee = s.TotalShippingFee,
                    Status = s.Status,
                    TotalItems = s.ShipmentItems.Sum(si => si.Quantity),
                    TotalCost = s.ShipmentItems.Sum(si => si.UnitCost * si.Quantity) + s.TotalShippingFee
                }).ToList(),
                StatusFilter = status,
                StartDate = startDate,
                EndDate = endDate,
                SearchTerm = search
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

            var shipment = await _context.Shipments
                .Include(s => s.ShipmentItems)
                .ThenInclude(si => si.Product)
                .FirstOrDefaultAsync(s => s.ShipmentId == id);

            if (shipment == null)
            {
                return NotFound();
            }

            int totalItems = shipment.ShipmentItems.Sum(si => si.Quantity);
            decimal allocatedShippingPerItem = totalItems > 0 ? shipment.TotalShippingFee / totalItems : 0;

            var viewModel = new ShipmentDetailsViewModel
            {
                ShipmentId = shipment.ShipmentId,
                ShipmentNumber = shipment.ShipmentNumber,
                StoreSource = shipment.StoreSource,
                OrderDate = shipment.OrderDate,
                ExpectedArrival = shipment.ExpectedArrival,
                ReceivedDate = shipment.ReceivedDate,
                TotalShippingFee = shipment.TotalShippingFee,
                Status = shipment.Status,
                Notes = shipment.Notes,
                Items = shipment.ShipmentItems.Select(si => new ShipmentItemDetailViewModel
                {
                    ShipmentItemId = si.ShipmentItemId,
                    ProductId = si.ProductId,
                    ItemName = si.ItemName,
                    ItemCode = si.ItemCode,
                    Category = si.Category,
                    Brand = si.Brand,
                    UnitCost = si.UnitCost,
                    Quantity = si.Quantity,
                    IsReceived = si.IsReceived,
                    ReceivedDate = si.ReceivedDate,
                    AllocatedShippingFee = allocatedShippingPerItem
                }).ToList()
            };

            return View(viewModel);
        }


        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Products = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            var model = new CreateShipmentViewModel
            {
                OrderDate = DateTime.Now,
                Items = new List<CreateShipmentItemViewModel>
                {
                    new CreateShipmentItemViewModel()
                }
            };

            return View(model);
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateShipmentViewModel model)
        {
            model.Items = model.Items.Where(i => !string.IsNullOrWhiteSpace(i.ItemName)).ToList();

            if (model.Items.Count == 0)
            {
                ModelState.AddModelError("", "Please add at least one item to the shipment.");
            }

            if (ModelState.IsValid)
            {
                string shipmentNumber = $"SHP-{DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

                var shipment = new Shipment
                {
                    ShipmentNumber = shipmentNumber,
                    StoreSource = model.StoreSource,
                    OrderDate = model.OrderDate,
                    ExpectedArrival = model.ExpectedArrival,
                    TotalShippingFee = model.TotalShippingFee,
                    Status = "Pending",
                    Notes = model.Notes
                };

                _context.Shipments.Add(shipment);
                await _context.SaveChangesAsync();

                // Create shipment items
                foreach (var item in model.Items)
                {
                    var shipmentItem = new ShipmentItem
                    {
                        ShipmentId = shipment.ShipmentId,
                        ProductId = item.ProductId,
                        ItemName = item.ItemName,
                        ItemCode = item.ItemCode,
                        Category = item.Category,
                        Brand = item.Brand,
                        UnitCost = item.UnitCost,
                        Quantity = item.Quantity,
                        IsReceived = false
                    };
                    _context.ShipmentItems.Add(shipmentItem);
                }

                // Record expense in Cash Flow for logistics
                var totalItemCost = model.Items.Sum(i => i.UnitCost * i.Quantity);
                var cashFlow = new CashFlow
                {
                    TransactionDate = model.OrderDate,
                    TransactionType = "Expense",
                    Category = "Logistics",
                    Description = $"Shipment order from {model.StoreSource} - {shipmentNumber}",
                    Amount = totalItemCost + model.TotalShippingFee,
                    ReferenceNumber = shipmentNumber,
                    ShipmentId = shipment.ShipmentId,
                    Notes = $"Items: {model.Items.Count}, Total Shipping: ?{model.TotalShippingFee:N2}"
                };
                _context.CashFlows.Add(cashFlow);

                await _context.SaveChangesAsync();


                TempData["Success"] = $"Shipment {shipmentNumber} created successfully with {model.Items.Count} items.";
                return RedirectToAction(nameof(Details), new { id = shipment.ShipmentId });
            }

            ViewBag.Products = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            return View(model);
        }

        [HttpGet("receive/{id}")]
        public async Task<IActionResult> Receive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipment = await _context.Shipments
                .Include(s => s.ShipmentItems)
                .FirstOrDefaultAsync(s => s.ShipmentId == id);

            if (shipment == null)
            {
                return NotFound();
            }

            if (shipment.Status == "Received" || shipment.Status == "Cancelled")
            {
                TempData["Error"] = $"This shipment is already {shipment.Status.ToLower()}.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var viewModel = new ReceiveShipmentViewModel
            {
                ShipmentId = shipment.ShipmentId,
                ShipmentNumber = shipment.ShipmentNumber,
                Items = shipment.ShipmentItems.Select(si => new ReceiveItemViewModel
                {
                    ShipmentItemId = si.ShipmentItemId,
                    ItemName = si.ItemName,
                    ItemCode = si.ItemCode,
                    Quantity = si.Quantity,
                    IsReceived = si.IsReceived,
                    MarkAsReceived = !si.IsReceived
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost("receive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Receive(ReceiveShipmentViewModel model)
        {
            var shipment = await _context.Shipments
                .Include(s => s.ShipmentItems)
                .FirstOrDefaultAsync(s => s.ShipmentId == model.ShipmentId);

            if (shipment == null)
            {
                return NotFound();
            }

            int totalItems = shipment.ShipmentItems.Sum(si => si.Quantity);
            decimal allocatedShippingPerItem = totalItems > 0 ? shipment.TotalShippingFee / totalItems : 0;

            int receivedCount = 0;

            foreach (var item in model.Items)
            {
                if (!item.MarkAsReceived) continue;

                var shipmentItem = shipment.ShipmentItems.FirstOrDefault(si => si.ShipmentItemId == item.ShipmentItemId);
                if (shipmentItem == null || shipmentItem.IsReceived) continue;

                // Mark item as received
                shipmentItem.IsReceived = true;
                shipmentItem.ReceivedDate = DateTime.Now;

                // Calculate final cost including allocated shipping
                decimal finalUnitCost = shipmentItem.UnitCost + allocatedShippingPerItem;

                // Check if product exists by item code
                var existingProduct = await _context.Products
                    .Include(p => p.Inventory)
                    .FirstOrDefaultAsync(p => p.ItemCode == shipmentItem.ItemCode);

                if (existingProduct != null)
                {
                    // Update existing inventory
                    if (existingProduct.Inventory != null)
                    {
                        existingProduct.Inventory.QuantityInStock += shipmentItem.Quantity;
                        existingProduct.Inventory.LastUpdated = DateTime.Now;
                    }
                    else
                    {
                        // Create inventory for existing product
                        var inventory = new Inventory
                        {
                            ProductId = existingProduct.ProductId,
                            QuantityInStock = shipmentItem.Quantity,
                            ReorderLevel = 10,
                            LastUpdated = DateTime.Now
                        };
                        _context.Inventories.Add(inventory);
                    }

                    // Link shipment item to product
                    shipmentItem.ProductId = existingProduct.ProductId;
                }
                else if (item.CreateNewProduct)
                {
                    // Create new product from shipment item
                    var newProduct = new Product
                    {
                        ProductName = shipmentItem.ItemName,
                        Brand = shipmentItem.Brand,
                        ItemCode = shipmentItem.ItemCode,
                        Category = shipmentItem.Category,
                        CostPrice = finalUnitCost,
                        SellingPrice = item.SellingPrice ?? finalUnitCost * 1.5m,
                        DateAdded = DateTime.Now,
                        IsActive = true
                    };

                    _context.Products.Add(newProduct);
                    await _context.SaveChangesAsync();

                    // Create inventory for new product
                    var inventory = new Inventory
                    {
                        ProductId = newProduct.ProductId,
                        QuantityInStock = shipmentItem.Quantity,
                        ReorderLevel = 10,
                        LastUpdated = DateTime.Now
                    };
                    _context.Inventories.Add(inventory);

                    // Link shipment item to new product
                    shipmentItem.ProductId = newProduct.ProductId;
                }
                else if (shipmentItem.ProductId.HasValue)
                {
                    // Update existing linked product inventory
                    var linkedProduct = await _context.Products
                        .Include(p => p.Inventory)
                        .FirstOrDefaultAsync(p => p.ProductId == shipmentItem.ProductId);

                    if (linkedProduct?.Inventory != null)
                    {
                        linkedProduct.Inventory.QuantityInStock += shipmentItem.Quantity;
                        linkedProduct.Inventory.LastUpdated = DateTime.Now;
                    }
                }


                receivedCount++;
            }

            var allReceived = shipment.ShipmentItems.All(si => si.IsReceived);
            if (allReceived)
            {
                shipment.Status = "Received";
                shipment.ReceivedDate = DateTime.Now;
            }
            else
            {
                shipment.Status = "In Transit";
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Received {receivedCount} items from shipment {shipment.ShipmentNumber}. {(allReceived ? "Shipment complete!" : "")}";
            return RedirectToAction(nameof(Details), new { id = model.ShipmentId });
        }

        [HttpPost("updatestatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var shipment = await _context.Shipments.FindAsync(id);
            if (shipment == null)
            {
                return NotFound();
            }

            var validStatuses = new[] { "Pending", "In Transit", "Received", "Cancelled" };
            if (!validStatuses.Contains(status))
            {
                TempData["Error"] = "Invalid status.";
                return RedirectToAction(nameof(Details), new { id });
            }

            shipment.Status = status;
            if (status == "Received")
            {
                shipment.ReceivedDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Shipment {shipment.ShipmentNumber} status updated to {status}.";
            return RedirectToAction(nameof(Details), new { id });
        }


        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipment = await _context.Shipments
                .Include(s => s.ShipmentItems)
                .FirstOrDefaultAsync(s => s.ShipmentId == id);

            if (shipment == null)
            {
                return NotFound();
            }

            return View(shipment);
        }

        [HttpPost("delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shipment = await _context.Shipments
                .Include(s => s.ShipmentItems)
                .FirstOrDefaultAsync(s => s.ShipmentId == id);

            if (shipment == null)
            {
                return NotFound();
            }

            if (shipment.Status == "Received")
            {
                TempData["Error"] = "Cannot delete received shipments.";
                return RedirectToAction(nameof(Details), new { id });
            }


            _context.ShipmentItems.RemoveRange(shipment.ShipmentItems);
            _context.Shipments.Remove(shipment);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Shipment {shipment.ShipmentNumber} deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("pending")]
        public async Task<IActionResult> Pending()
        {
            return RedirectToAction(nameof(Index), new { status = "Pending" });
        }
    }
}
