using System.ComponentModel.DataAnnotations;

namespace COMP019_Activity4_4JLCSystems.Models.ViewModels
{
    /// ProductListViewModel - For displaying products with inventory info
    public class ProductListViewModel
    {
        public List<ProductDisplayViewModel> Products { get; set; } = new List<ProductDisplayViewModel>();
        public string? CategoryFilter { get; set; }
        public string? StockFilter { get; set; } // All, InStock, LowStock, OutOfStock
        public string? SearchTerm { get; set; }
        public List<string> AvailableCategories { get; set; } = new List<string>();
    }

    /// ProductDisplayViewModel - For displaying product with stock info
    public class ProductDisplayViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateAdded { get; set; }

        // Inventory info
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }
        public bool IsLowStock { get; set; }
        public bool IsOutOfStock { get; set; }
        public string StockStatus { get; set; } = string.Empty;

        // Computed
        public decimal ProfitMargin => SellingPrice - CostPrice;
        public decimal ProfitPercentage => CostPrice > 0 ? ((SellingPrice - CostPrice) / CostPrice) * 100 : 0;
    }

    /// CreateProductViewModel - For creating a new product
    public class CreateProductViewModel
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200)]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Brand")]
        public string? Brand { get; set; }

        [Required(ErrorMessage = "Item code is required")]
        [StringLength(50)]
        [Display(Name = "Item Code")]
        public string ItemCode { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Category")]
        public string? Category { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Image URL")]
        [StringLength(500)]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Cost price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Cost price must be greater than 0")]
        [Display(Name = "Cost Price (Puhunan)")]
        public decimal CostPrice { get; set; }

        [Required(ErrorMessage = "Selling price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Selling price must be greater than 0")]
        [Display(Name = "Selling Price")]
        public decimal SellingPrice { get; set; }

        [Required(ErrorMessage = "Initial stock quantity is required")]
        [Range(0, 100000, ErrorMessage = "Quantity must be between 0 and 100,000")]
        [Display(Name = "Initial Stock Quantity")]
        public int InitialStock { get; set; }

        [Range(0, 10000, ErrorMessage = "Reorder level must be between 0 and 10,000")]
        [Display(Name = "Reorder Level (Low Stock Warning)")]
        public int ReorderLevel { get; set; } = 10;
    }

    /// EditProductViewModel - For editing an existing product
    public class EditProductViewModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200)]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Brand")]
        public string? Brand { get; set; }

        [Required(ErrorMessage = "Item code is required")]
        [StringLength(50)]
        [Display(Name = "Item Code")]
        public string ItemCode { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Category")]
        public string? Category { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Image URL")]
        [StringLength(500)]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Cost price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Cost price must be greater than 0")]
        [Display(Name = "Cost Price (Puhunan)")]
        public decimal CostPrice { get; set; }

        [Required(ErrorMessage = "Selling price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Selling price must be greater than 0")]
        [Display(Name = "Selling Price")]
        public decimal SellingPrice { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}
