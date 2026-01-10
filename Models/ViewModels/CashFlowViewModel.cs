using System.ComponentModel.DataAnnotations;

namespace COMP019_Activity4_4JLCSystems.Models.ViewModels
{
    /// CashFlowSummaryViewModel - For displaying cash flow overview
    public class CashFlowSummaryViewModel
    {
        // Summary totals
        [Display(Name = "Total Inventory Value (Capital)")]
        public decimal TotalInventoryValue { get; set; }

        [Display(Name = "Total Inventory Cost")]
        public decimal TotalInventoryCost { get; set; }

        [Display(Name = "Pending Payments")]
        public decimal PendingPayments { get; set; }

        [Display(Name = "Pending Order Count")]
        public int PendingOrderCount { get; set; }

        [Display(Name = "Total Money In (Sales)")]
        public decimal TotalMoneyIn { get; set; }

        [Display(Name = "Total Money Out")]
        public decimal TotalMoneyOut { get; set; }

        [Display(Name = "Net Cash Flow")]
        public decimal NetCashFlow => TotalMoneyIn - TotalMoneyOut;

        // Breakdown by category
        public decimal SalesIncome { get; set; }
        public decimal OtherIncome { get; set; }

        public decimal LogisticsExpense { get; set; }
        public decimal ShippingExpense { get; set; }
        public decimal PurchaseExpense { get; set; }
        public decimal OtherExpense { get; set; }

        // Recent transactions
        public List<CashFlowItemViewModel> RecentTransactions { get; set; } = new List<CashFlowItemViewModel>();

        // Filter options
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? TransactionType { get; set; }
        public string? Category { get; set; }
    }

    /// CashFlowItemViewModel - For displaying individual cash flow entry
    public class CashFlowItemViewModel
    {
        public int CashFlowId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
        public int? OrderId { get; set; }
        public int? ShipmentId { get; set; }

        // Computed
        public bool IsIncome => TransactionType == "Income";
        public bool IsExpense => TransactionType == "Expense";
    }

    /// CreateCashFlowViewModel - For adding manual cash flow entries
    public class CreateCashFlowViewModel
    {
        [Required(ErrorMessage = "Transaction date is required")]
        [Display(Name = "Transaction Date")]
        [DataType(DataType.Date)]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Transaction type is required")]
        [Display(Name = "Transaction Type")]
        public string TransactionType { get; set; } = string.Empty; // Income, Expense

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(300)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, 10000000, ErrorMessage = "Amount must be greater than 0")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [StringLength(100)]
        [Display(Name = "Reference Number")]
        public string? ReferenceNumber { get; set; }

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }
    }
}
