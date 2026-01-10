using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP019_Activity4_4JLCSystems.Models.Entities
{
    /// Order entity - Represents customer orders (Shopee-like flow)
    /// Status flow: Pending -> Paid -> Shipped -> Delivered
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Order Number")]
        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [StringLength(200)]
        [Display(Name = "Customer Name")]
        public string? CustomerName { get; set; }

        [StringLength(500)]
        [Display(Name = "Shipping Address")]
        public string? ShippingAddress { get; set; }

        [StringLength(20)]
        [Display(Name = "Contact Number")]
        public string? ContactNumber { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Subtotal")]
        public decimal Subtotal { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Shipping Fee")]
        public decimal ShippingFee { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Order Status")]
        public string OrderStatus { get; set; } = "Pending"; // Pending, Paid, Shipped, Delivered, Cancelled

        [Display(Name = "Payment Date")]
        public DateTime? PaymentDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Payment Method")]
        public string? PaymentMethod { get; set; }

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        // Navigation property
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Computed properties
        [NotMapped]
        public bool IsPaid => OrderStatus == "Paid" || OrderStatus == "Shipped" || OrderStatus == "Delivered";

        [NotMapped]
        public decimal TotalCost => OrderItems.Sum(oi => oi.UnitCost * oi.Quantity);

        [NotMapped]
        public decimal Profit => Subtotal - TotalCost - ShippingFee;
    }
}
