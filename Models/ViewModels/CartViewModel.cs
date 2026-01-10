using System.ComponentModel.DataAnnotations;
using COMP019_Activity4_4JLCSystems.Models.Entities;

namespace COMP019_Activity4_4JLCSystems.Models.ViewModels
{
    /// CartViewModel - Displays the shopping cart with all items
    public class CartViewModel
    {
        public int CartId { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();

        // Computed totals
        public decimal Subtotal => Items.Sum(i => i.LineTotal);
        public int TotalItems => Items.Sum(i => i.Quantity);
    }

    /// CartItemViewModel - Individual cart item display
    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int AvailableStock { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }

    /// AddToCartViewModel - Input model for adding items to cart
    public class AddToCartViewModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int Quantity { get; set; } = 1;
    }

    /// CheckoutViewModel - Input model for checkout process
    public class CheckoutViewModel
    {
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();

        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(200)]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Shipping address is required")]
        [StringLength(500)]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Contact Number")]
        public string? ContactNumber { get; set; }

        [Display(Name = "Shipping Fee")]
        public decimal ShippingFee { get; set; } = 50.00m;

        [StringLength(500)]
        [Display(Name = "Order Notes")]
        public string? Notes { get; set; }

        public decimal Subtotal { get; set; }
        public decimal TotalAmount => Subtotal + ShippingFee;
    }
}
