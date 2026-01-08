using Microsoft.EntityFrameworkCore;
using COMP019_Activity4_4JLCSystems.Models.Entities;

namespace COMP019_Activity4_4JLCSystems.Data
{
    /// ApplicationDbContext - The main database context for 4JLC
    /// Inherits from DbContext and configures EF Core
    /// Uses constructor injection with DbContextOptions
    public class ApplicationDbContext : DbContext
    {
        // Constructor injection - receives options from Program.cs DI container
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet properties - Each represents a table in the database
        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentItem> ShipmentItems { get; set; }
        public DbSet<CashFlow> CashFlows { get; set; }
        public DbSet<User> Users { get; set; }

        /// Configure entity relationships and constraints using Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(p => p.ItemCode).IsUnique();
                entity.Property(p => p.CostPrice).HasPrecision(18, 2);
                entity.Property(p => p.SellingPrice).HasPrecision(18, 2);
            });

            // Inventory configuration - One-to-One with Product
            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasOne(i => i.Product)
                    .WithOne(p => p.Inventory)
                    .HasForeignKey<Inventory>(i => i.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Order configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(o => o.OrderNumber).IsUnique();
                entity.Property(o => o.Subtotal).HasPrecision(18, 2);
                entity.Property(o => o.ShippingFee).HasPrecision(18, 2);
                entity.Property(o => o.TotalAmount).HasPrecision(18, 2);
            });

            // OrderItem configuration - Many-to-One with Order and Product
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(oi => oi.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(oi => oi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(oi => oi.UnitPrice).HasPrecision(18, 2);
                entity.Property(oi => oi.UnitCost).HasPrecision(18, 2);
            });

            // Cart configuration
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasIndex(c => c.SessionId);
            });

            // CartItem configuration - Many-to-One with Cart and Product
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasOne(ci => ci.Cart)
                    .WithMany(c => c.CartItems)
                    .HasForeignKey(ci => ci.CartId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ci => ci.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(ci => ci.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Shipment configuration
            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.HasIndex(s => s.ShipmentNumber).IsUnique();
                entity.Property(s => s.TotalShippingFee).HasPrecision(18, 2);
            });

            // ShipmentItem configuration
            modelBuilder.Entity<ShipmentItem>(entity =>
            {
                entity.HasOne(si => si.Shipment)
                    .WithMany(s => s.ShipmentItems)
                    .HasForeignKey(si => si.ShipmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(si => si.Product)
                    .WithMany(p => p.ShipmentItems)
                    .HasForeignKey(si => si.ProductId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.Property(si => si.UnitCost).HasPrecision(18, 2);
            });

            // CashFlow configuration
            modelBuilder.Entity<CashFlow>(entity =>
            {
                entity.Property(cf => cf.Amount).HasPrecision(18, 2);
                entity.HasIndex(cf => cf.TransactionDate);
                entity.HasIndex(cf => cf.TransactionType);
            });

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                // Email is optional, so we use a filtered unique index
                entity.HasIndex(u => u.Email)
                    .IsUnique()
                    .HasFilter("[Email] IS NOT NULL");
            });

            // Seed initial data for testing
            SeedData(modelBuilder);
        }

        /// Seed sample data for testing and demonstration
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    ProductName = "Wireless Bluetooth Earbuds",
                    Brand = "TechSound",
                    ItemCode = "PROD-001",
                    Category = "Electronics",
                    Description = "High-quality wireless earbuds with noise cancellation",
                    CostPrice = 250.00m,
                    SellingPrice = 450.00m,
                    DateAdded = new DateTime(2024, 1, 15),
                    IsActive = true
                },
                new Product
                {
                    ProductId = 2,
                    ProductName = "USB-C Fast Charging Cable",
                    Brand = "PowerLink",
                    ItemCode = "PROD-002",
                    Category = "Accessories",
                    Description = "1.5m braided USB-C cable with fast charging support",
                    CostPrice = 45.00m,
                    SellingPrice = 99.00m,
                    DateAdded = new DateTime(2024, 1, 15),
                    IsActive = true
                },
                new Product
                {
                    ProductId = 3,
                    ProductName = "Phone Case - Clear",
                    Brand = "ProCase",
                    ItemCode = "PROD-003",
                    Category = "Accessories",
                    Description = "Transparent protective phone case",
                    CostPrice = 35.00m,
                    SellingPrice = 85.00m,
                    DateAdded = new DateTime(2024, 1, 16),
                    IsActive = true
                },
                new Product
                {
                    ProductId = 4,
                    ProductName = "Portable Power Bank 10000mAh",
                    Brand = "PowerMax",
                    ItemCode = "PROD-004",
                    Category = "Electronics",
                    Description = "Compact power bank with dual USB output",
                    CostPrice = 320.00m,
                    SellingPrice = 599.00m,
                    DateAdded = new DateTime(2024, 1, 17),
                    IsActive = true
                },
                new Product
                {
                    ProductId = 5,
                    ProductName = "Screen Protector - Tempered Glass",
                    Brand = "ShieldPro",
                    ItemCode = "PROD-005",
                    Category = "Accessories",
                    Description = "9H hardness tempered glass screen protector",
                    CostPrice = 25.00m,
                    SellingPrice = 65.00m,
                    DateAdded = new DateTime(2024, 1, 17),
                    IsActive = true
                }
            );

            // Seed Inventory
            modelBuilder.Entity<Inventory>().HasData(
                new Inventory { InventoryId = 1, ProductId = 1, QuantityInStock = 50, ReorderLevel = 10, LastUpdated = new DateTime(2024, 1, 15) },
                new Inventory { InventoryId = 2, ProductId = 2, QuantityInStock = 100, ReorderLevel = 20, LastUpdated = new DateTime(2024, 1, 15) },
                new Inventory { InventoryId = 3, ProductId = 3, QuantityInStock = 8, ReorderLevel = 10, LastUpdated = new DateTime(2024, 1, 16) },
                new Inventory { InventoryId = 4, ProductId = 4, QuantityInStock = 25, ReorderLevel = 5, LastUpdated = new DateTime(2024, 1, 17) },
                new Inventory { InventoryId = 5, ProductId = 5, QuantityInStock = 0, ReorderLevel = 15, LastUpdated = new DateTime(2024, 1, 17) }
            );

            // Seed Admin User
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "admin",
                    Password = "admin123",
                    Email = "admin@4jlc.com",
                    FullName = "System Administrator",
                    Role = "Admin",
                    DateCreated = new DateTime(2024, 1, 1),
                    IsActive = true
                }
            );
        }
    }
}
