using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP019_Activity4_4JLCSystems.Models.Entities
{
    /// <summary>
    /// OrderItem entity - Line items within an order
    /// Links Orders to Products with quantity and pricing
    /// </summary>
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required]
        [ForeignKey("Order")]
        public int OrderId { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Unit Cost")]
        public decimal UnitCost { get; set; }

        // Computed property - Line total
        [NotMapped]
        public decimal LineTotal => UnitPrice * Quantity;

        // Computed property - Line profit
        [NotMapped]
        public decimal LineProfit => (UnitPrice - UnitCost) * Quantity;

        // Navigation properties
        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
