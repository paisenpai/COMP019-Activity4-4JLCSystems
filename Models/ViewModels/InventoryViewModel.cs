using System.ComponentModel.DataAnnotations;

namespace COMP019_Activity4_4JLCSystems.Models.ViewModels
{
    /// InventoryListViewModel - For displaying inventory list with filters
    public class InventoryListViewModel
    {
        public List<InventoryItemViewModel> Items { get; set; } = new List<InventoryItemViewModel>();
        public string? CategoryFilter { get; set; }
        public string? StockFilter { get; set; } // All, InStock, LowStock, OutOfStock
        public string? SearchTerm { get; set; }
        public List<string> AvailableCategories { get; set; } = new List<string>();

        // Summary statistics
        public int TotalProducts { get; set; }
        public int InStockCount { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public decimal TotalInventoryCost { get; set; }
    }

    /// InventoryItemViewModel - For displaying individual inventory item

    public class InventoryItemViewModel
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? ImageUrl { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsLowStock { get; set; }
        public bool IsOutOfStock { get; set; }
        public string StockStatus { get; set; } = string.Empty;

        // Computed values
        public decimal TotalValue => SellingPrice * QuantityInStock;
        public decimal TotalCost => CostPrice * QuantityInStock;
    }

    /// UpdateInventoryViewModel - For adjusting stock quantities
    public class UpdateInventoryViewModel
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public int CurrentQuantity { get; set; }

        [Required(ErrorMessage = "Please select an adjustment type")]
        [Display(Name = "Adjustment Type")]
        public string AdjustmentType { get; set; } = string.Empty; // Add, Remove, Set

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 100000, ErrorMessage = "Quantity must be at least 1")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [StringLength(500)]
        [Display(Name = "Reason")]
        public string? Reason { get; set; }

        [Range(0, 10000, ErrorMessage = "Reorder level must be between 0 and 10,000")]
        [Display(Name = "Reorder Level")]
        public int ReorderLevel { get; set; }
    }
}
