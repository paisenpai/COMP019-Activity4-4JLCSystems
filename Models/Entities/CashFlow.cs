using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP019_Activity4_4JLCSystems.Models.Entities
{
    /// CashFlow entity - Financial tracking for money in/out
    /// Records all financial transactions in the system
    public class CashFlow
    {
        [Key]
        public int CashFlowId { get; set; }

        [Required]
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        [Display(Name = "Transaction Type")]
        public string TransactionType { get; set; } = string.Empty; // Income, Expense

        [Required]
        [StringLength(100)]
        [Display(Name = "Category")]
        public string Category { get; set; } = string.Empty; // Sales, Logistics, Shipping, Purchase, Other

        [Required]
        [StringLength(300)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [StringLength(100)]
        [Display(Name = "Reference Number")]
        public string? ReferenceNumber { get; set; }

        // Foreign key references (optional for linking to specific orders/shipments)
        [Display(Name = "Related Order")]
        public int? OrderId { get; set; }

        [Display(Name = "Related Shipment")]
        public int? ShipmentId { get; set; }

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Computed property - Is this income or expense?
        [NotMapped]
        public bool IsIncome => TransactionType == "Income";

        [NotMapped]
        public bool IsExpense => TransactionType == "Expense";

        // Navigation properties (optional)
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [ForeignKey("ShipmentId")]
        public virtual Shipment? Shipment { get; set; }
    }
}
