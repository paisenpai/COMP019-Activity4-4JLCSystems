using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP019_Activity4_4JLCSystems.Models.Entities
{
    /// ShipmentItem entity - Individual items in a shipment delivery
    /// Contains cost and quantity information for inventory tracking
    public class ShipmentItem
    {
        [Key]
        public int ShipmentItemId { get; set; }

        [Required]
        [ForeignKey("Shipment")]
        public int ShipmentId { get; set; }

        [ForeignKey("Product")]
        [Display(Name = "Existing Product")]
        public int? ProductId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Item Code")]
        public string ItemCode { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Category")]
        public string? Category { get; set; }

        [StringLength(100)]
        [Display(Name = "Brand")]
        public string? Brand { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Unit Cost (Puhunan)")]
        public decimal UnitCost { get; set; }

        [Required]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Is Received")]
        public bool IsReceived { get; set; } = false;

        [Display(Name = "Received Date")]
        public DateTime? ReceivedDate { get; set; }

        // Navigation properties
        public virtual Shipment Shipment { get; set; } = null!;
        public virtual Product? Product { get; set; }

        // Computed properties
        [NotMapped]
        public decimal LineTotalCost => UnitCost * Quantity;

        // Allocated shipping fee per unit (calculated from parent shipment)
        [NotMapped]
        public decimal AllocatedShippingFeePerUnit => Shipment?.TotalItems > 0 
            ? (Shipment.TotalShippingFee / Shipment.TotalItems) 
            : 0;

        // Final cost per unit including allocated shipping
        [NotMapped]
        public decimal FinalUnitCost => UnitCost + AllocatedShippingFeePerUnit;
    }
}
