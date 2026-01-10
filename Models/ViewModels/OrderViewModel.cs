using System.ComponentModel.DataAnnotations;
using COMP019_Activity4_4JLCSystems.Models.Entities;

namespace COMP019_Activity4_4JLCSystems.Models.ViewModels
{
    /// OrderListViewModel - For displaying list of orders
    public class OrderListViewModel
    {
        public List<Order> Orders { get; set; } = new List<Order>();
        public string? StatusFilter { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SearchTerm { get; set; }
        
        // Pagination
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
    }

    /// OrderDetailsViewModel - For displaying order details with items
    public class OrderDetailsViewModel
    {
        public Order Order { get; set; } = null!;
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // Profit calculation
        public decimal TotalCost => OrderItems.Sum(oi => oi.UnitCost * oi.Quantity);
        public decimal Profit => Order.Subtotal - TotalCost - Order.ShippingFee;
    }

    /// ProcessPaymentViewModel - For marking order as paid
    public class ProcessPaymentViewModel
    {
        [Required]
        public int OrderId { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Payment method is required")]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Display(Name = "Payment Notes")]
        [StringLength(500)]
        public string? PaymentNotes { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
