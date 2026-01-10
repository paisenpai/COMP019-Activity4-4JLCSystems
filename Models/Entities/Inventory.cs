using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP019_Activity4_4JLCSystems.Models.Entities
{
    /// <summary>
    /// Inventory entity - Tracks stock levels for each product
    /// Linked to Product via one-to-one relationship
    /// </summary>
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }

        [Required]
        [ForeignKey("Product")]
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        [Required]
        [Display(Name = "Quantity in Stock")]
        public int QuantityInStock { get; set; }

        [Display(Name = "Reorder Level")]
        public int ReorderLevel { get; set; } = 10; // Low stock threshold

        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // Computed property - Check if low stock (less than half of reorder level)
        [NotMapped]
        public bool IsLowStock => QuantityInStock > 0 && QuantityInStock < ReorderLevel / 2;

        // Computed property - Check if out of stock
        [NotMapped]
        public bool IsOutOfStock => QuantityInStock <= 0;

        // Computed property - Stock status display
        [NotMapped]
        public string StockStatus
        {
            get
            {
                if (IsOutOfStock) return "Out of Stock";
                if (IsLowStock) return "Low Stock";
                return "In Stock";
            }
        }

        // Navigation property
        public virtual Product Product { get; set; } = null!;
    }
}
