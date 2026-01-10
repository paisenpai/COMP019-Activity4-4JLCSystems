using System.ComponentModel.DataAnnotations;

namespace COMP019_Activity4_4JLCSystems.Models.ViewModels
{
    /// DashboardViewModel - Displays computed summary values for the dashboard
    /// Shows Total Sales, Pending Orders, Total Profit, Total Shipments
    public class DashboardViewModel
    {
        // Sales Metrics
        [Display(Name = "Total Sales")]
        public decimal TotalSales { get; set; }

        [Display(Name = "Sales This Month")]
        public decimal SalesThisMonth { get; set; }

        [Display(Name = "Sales Today")]
        public decimal SalesToday { get; set; }

        // Order Metrics
        [Display(Name = "Total Orders")]
        public int TotalOrders { get; set; }

        [Display(Name = "Pending Orders")]
        public int PendingOrders { get; set; }

        [Display(Name = "Paid Orders")]
        public int PaidOrders { get; set; }

        [Display(Name = "Shipped Orders")]
        public int ShippedOrders { get; set; }

        [Display(Name = "Delivered Orders")]
        public int DeliveredOrders { get; set; }

        // Profit Metrics (Profit = Total Sales - Item Cost - Shipping Fees)
        [Display(Name = "Total Profit")]
        public decimal TotalProfit { get; set; }

        [Display(Name = "Profit This Month")]
        public decimal ProfitThisMonth { get; set; }

        [Display(Name = "Total Item Costs")]
        public decimal TotalItemCosts { get; set; }

        [Display(Name = "Total Shipping Fees Deducted")]
        public decimal TotalShippingFeesDeducted { get; set; }

        // Shipment/Logistics Metrics
        [Display(Name = "Total Shipments")]
        public int TotalShipments { get; set; }

        [Display(Name = "Pending Shipments")]
        public int PendingShipments { get; set; }

        [Display(Name = "Received Shipments")]
        public int ReceivedShipments { get; set; }

        // Inventory Metrics
        [Display(Name = "Total Products")]
        public int TotalProducts { get; set; }

        [Display(Name = "Low Stock Items")]
        public int LowStockItems { get; set; }

        [Display(Name = "Out of Stock Items")]
        public int OutOfStockItems { get; set; }

        [Display(Name = "Total Inventory Value")]
        public decimal TotalInventoryValue { get; set; }

        // Cash Flow Metrics
        [Display(Name = "Total Money In")]
        public decimal TotalMoneyIn { get; set; }

        [Display(Name = "Total Money Out")]
        public decimal TotalMoneyOut { get; set; }

        [Display(Name = "Net Cash Flow")]
        public decimal NetCashFlow => TotalMoneyIn - TotalMoneyOut;

        // Recent Activity
        public List<RecentOrderViewModel> RecentOrders { get; set; } = new List<RecentOrderViewModel>();
        public List<RecentShipmentViewModel> RecentShipments { get; set; } = new List<RecentShipmentViewModel>();
        public List<LowStockItemViewModel> LowStockProducts { get; set; } = new List<LowStockItemViewModel>();
    }

    /// Recent Order display for Dashboard
    public class RecentOrderViewModel
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string? CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    /// Recent Shipment display for Dashboard
    public class RecentShipmentViewModel
    {
        public int ShipmentId { get; set; }
        public string ShipmentNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string StoreSource { get; set; } = string.Empty;
        public int TotalItems { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    /// Low Stock Item display for Dashboard
    public class LowStockItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public int QuantityInStock { get; set; }
        public int ReorderLevel { get; set; }
        public string StockStatus { get; set; } = string.Empty;
    }
}
