# 4JLC - Business Management System
<img width="128" height="58" alt="image" src="https://github.com/user-attachments/assets/f9aef399-a7d7-497f-80e2-87b59a940f0c" />

A complete ASP.NET Core MVC web application for business management using Entity Framework Core (Code First) and SQL Server.

## Admin Credentials:

- Username: admin
- Password: admin123

## System Requirements

- .NET 10 SDK
- SQL Server (LocalDB, SQL Server Express, or full SQL Server)
- Visual Studio 2022 or later (recommended)

## Quick Start Guide

### Step 1: Configure Database Connection String

The connection string is in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=4JLCDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

Modify this if you're using a different SQL Server instance:
- **SQL Server Express:** `Server=.\\SQLEXPRESS;Database=4JLCDB;Trusted_Connection=True;...`
- **Full SQL Server:** `Server=YOUR_SERVER_NAME;Database=4JLCDB;Trusted_Connection=True;...`

### Step 2: Run Database Migrations

Open **Package Manager Console** in Visual Studio:

```powershell
# Create the initial migration
Add-Migration InitialMigration

# Apply migration to database
Update-Database
```

Or using .NET CLI:

```bash
dotnet ef migrations add InitialMigration
dotnet ef database update
```

### Step 3: Verify in SQL Server Management Studio (SSMS)

1. Open SSMS
2. Connect to your SQL Server instance
3. Expand Databases
4. Look for `4JLCDB`
5. Verify tables: Products, Inventories, Orders, OrderItems, Carts, CartItems, Shipments, ShipmentItems, CashFlows

### Step 4: Run the Application

Press F5 in Visual Studio or:

```bash
dotnet run
```

Navigate to: `https://localhost:5001` or `http://localhost:5000`

---
<img width="1455" height="850" alt="image" src="https://github.com/user-attachments/assets/657cd9fd-63c4-4f44-a06c-d8b901f0c140" />

<img width="1701" height="924" alt="image" src="https://github.com/user-attachments/assets/3b04a12e-6fbb-4721-b452-c1caeb9b0b0f" />

<img width="1861" height="997" alt="image" src="https://github.com/user-attachments/assets/7f2bba8a-8a6f-4fe6-8137-dc92fd9fd935" />

---
## Database Schema

### Entity Fields

**Products:**
- ProductId, ProductName, Brand, ItemCode, Category, Description, ImageUrl
- CostPrice (Puhunan), SellingPrice, DateAdded, IsActive

**Inventory:**
- InventoryId, ProductId, QuantityInStock, ReorderLevel, LastUpdated
- Computed: IsLowStock, IsOutOfStock, StockStatus

**Orders:**
- OrderId, OrderNumber, OrderDate, CustomerName, ShippingAddress, ContactNumber
- Subtotal, ShippingFee, TotalAmount, OrderStatus, PaymentDate, PaymentMethod

**OrderItems:**
- OrderItemId, OrderId, ProductId, ProductName, Quantity, UnitPrice, UnitCost

**Shipments:**
- ShipmentId, ShipmentNumber, StoreSource, OrderDate, ExpectedArrival, ReceivedDate
- TotalShippingFee, Status, Notes

**ShipmentItems:**
- ShipmentItemId, ShipmentId, ProductId, ItemName, ItemCode, Category, Brand
- UnitCost, Quantity, IsReceived, ReceivedDate

**CashFlow:**
- CashFlowId, TransactionDate, TransactionType (Income/Expense)
- Category (Sales, Logistics, Shipping, Purchase, Other)
- Description, Amount, ReferenceNumber, OrderId, ShipmentId

---

##  Project Structure

```
4JLC/
 Controllers/
    DashboardController.cs
    ProductsController.cs
    InventoryController.cs
    CartController.cs
    OrdersController.cs
    ShipmentsController.cs
    CashFlowController.cs
 Data/
    ApplicationDbContext.cs
 Models/
    Entities/
        Product.cs
        Inventory.cs
        Order.cs
        OrderItem.cs
        Cart.cs
        CartItem.cs
        Shipment.cs
        ShipmentItem.cs
        CashFlow.cs
    ViewModels/
        DashboardViewModel.cs
        ProductViewModel.cs
        InventoryViewModel.cs
        CartViewModel.cs
        OrderViewModel.cs
        ShipmentViewModel.cs
        CashFlowViewModel.cs
 Views/
    Dashboard/
    Products/
    Inventory/
    Cart/
    Orders/
    Shipments/
    CashFlow/
    Shared/
 appsettings.json
 Program.cs
```

---

##  NuGet Packages Required

- Microsoft.EntityFrameworkCore.SqlServer (9.0.0)
- Microsoft.EntityFrameworkCore.Tools (9.0.0)
- Microsoft.EntityFrameworkCore.Design (9.0.0)

---
