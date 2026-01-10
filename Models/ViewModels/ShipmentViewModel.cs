using System.ComponentModel.DataAnnotations;

namespace COMP019_Activity4_4JLCSystems.Models.ViewModels
{
    /// ShipmentListViewModel - For displaying list of shipments
    public class ShipmentListViewModel
    {
        public List<ShipmentDisplayViewModel> Shipments { get; set; } = new List<ShipmentDisplayViewModel>();
        public string? StatusFilter { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SearchTerm { get; set; }
    }

    /// ShipmentDisplayViewModel - For displaying shipment in list
    public class ShipmentDisplayViewModel
    {
        public int ShipmentId { get; set; }
        public string ShipmentNumber { get; set; } = string.Empty;
        public string StoreSource { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedArrival { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public decimal TotalShippingFee { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalItems { get; set; }
        public decimal TotalCost { get; set; }
    }

    /// CreateShipmentViewModel - For creating a new shipment with items
    public class CreateShipmentViewModel
    {
        [Required(ErrorMessage = "Store/Shop source is required")]
        [StringLength(200)]
        [Display(Name = "Store/Shop Source")]
        public string StoreSource { get; set; } = string.Empty;

        [Required(ErrorMessage = "Order date is required")]
        [Display(Name = "Order Date")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Display(Name = "Expected Arrival Date")]
        [DataType(DataType.Date)]
        public DateTime? ExpectedArrival { get; set; }

        [Required(ErrorMessage = "Total shipping fee is required")]
        [Range(0, 100000, ErrorMessage = "Shipping fee must be between 0 and 100,000")]
        [Display(Name = "Total Shipping Fee")]
        public decimal TotalShippingFee { get; set; }

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        // Shipment items to add
        public List<CreateShipmentItemViewModel> Items { get; set; } = new List<CreateShipmentItemViewModel>();
    }

    /// CreateShipmentItemViewModel - For adding items to a shipment
    public class CreateShipmentItemViewModel
    {
        [Display(Name = "Link to Existing Product")]
        public int? ProductId { get; set; }

        [Required(ErrorMessage = "Item name is required")]
        [StringLength(200)]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Item code is required")]
        [StringLength(50)]
        [Display(Name = "Item Code")]
        public string ItemCode { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Category")]
        public string? Category { get; set; }

        [StringLength(100)]
        [Display(Name = "Brand")]
        public string? Brand { get; set; }

        [Required(ErrorMessage = "Unit cost is required")]
        [Range(0.01, 1000000, ErrorMessage = "Unit cost must be greater than 0")]
        [Display(Name = "Unit Cost (Puhunan)")]
        public decimal UnitCost { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 10000, ErrorMessage = "Quantity must be at least 1")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
    }

    /// ShipmentDetailsViewModel - For viewing shipment details
    public class ShipmentDetailsViewModel
    {
        public int ShipmentId { get; set; }
        public string ShipmentNumber { get; set; } = string.Empty;
        public string StoreSource { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime? ExpectedArrival { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public decimal TotalShippingFee { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }

        public List<ShipmentItemDetailViewModel> Items { get; set; } = new List<ShipmentItemDetailViewModel>();

        // Computed properties
        public int TotalItems => Items.Sum(i => i.Quantity);
        public decimal TotalItemCost => Items.Sum(i => i.LineTotalCost);
        public decimal AllocatedShippingPerItem => TotalItems > 0 ? TotalShippingFee / TotalItems : 0;
        public decimal TotalCostWithShipping => TotalItemCost + TotalShippingFee;
    }

    /// ShipmentItemDetailViewModel - For displaying shipment item details
    public class ShipmentItemDetailViewModel
    {
        public int ShipmentItemId { get; set; }
        public int? ProductId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public decimal UnitCost { get; set; }
        public int Quantity { get; set; }
        public bool IsReceived { get; set; }
        public DateTime? ReceivedDate { get; set; }

        // Computed properties
        public decimal LineTotalCost => UnitCost * Quantity;
        public decimal AllocatedShippingFee { get; set; }
        public decimal FinalUnitCost => UnitCost + AllocatedShippingFee;
    }

    /// ReceiveShipmentViewModel - For receiving shipment items into inventory
    public class ReceiveShipmentViewModel
    {
        public int ShipmentId { get; set; }
        public string ShipmentNumber { get; set; } = string.Empty;
        public List<ReceiveItemViewModel> Items { get; set; } = new List<ReceiveItemViewModel>();
    }

    /// ReceiveItemViewModel - For marking individual items as received
    public class ReceiveItemViewModel
    {
        public int ShipmentItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public bool IsReceived { get; set; }
        public bool MarkAsReceived { get; set; }

        // For creating new product if needed
        public bool CreateNewProduct { get; set; }
        public decimal? SellingPrice { get; set; }
    }
}
