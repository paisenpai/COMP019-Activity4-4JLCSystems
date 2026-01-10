using System.ComponentModel.DataAnnotations;

namespace COMP019_Activity4_4JLCSystems.Models.Entities
{
    /// Cart entity - Session-based shopping cart
    /// Stores temporary cart data before checkout
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Session ID")]
        public string SessionId { get; set; } = string.Empty;

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Last Updated")]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // Navigation property
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
