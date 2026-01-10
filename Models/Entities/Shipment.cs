using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP019_Activity4_4JLCSystems.Models.Entities
{
    /// Shipment entity - Logistics module for incoming stock deliveries
    /// Tracks supplier deliveries before they are added to inventory
    public class Shipment
    {
        [Key]
        public int ShipmentId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Shipment Number")]
        public string ShipmentNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Store/Shop Source")]
        public string StoreSource { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Display(Name = "Expected Arrival")]
        public DateTime? ExpectedArrival { get; set; }

        [Display(Name = "Received Date")]
        public DateTime? ReceivedDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Shipping Fee")]
        public decimal TotalShippingFee { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending"; // Pending, In Transit, Received, Cancelled

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        // Navigation property
        public virtual ICollection<ShipmentItem> ShipmentItems { get; set; } = new List<ShipmentItem>();

        // Computed properties
        [NotMapped]
        public int TotalItems => ShipmentItems.Sum(si => si.Quantity);

        [NotMapped]
        public decimal TotalItemCost => ShipmentItems.Sum(si => si.UnitCost * si.Quantity);

        [NotMapped]
        public decimal AllocatedShippingPerItem => TotalItems > 0 ? TotalShippingFee / TotalItems : 0;

        [NotMapped]
        public decimal TotalCostWithShipping => TotalItemCost + TotalShippingFee;
    }
}
