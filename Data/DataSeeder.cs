using Microsoft.EntityFrameworkCore;
using COMP019_Activity4_4JLCSystems.Models.Entities;

namespace COMP019_Activity4_4JLCSystems.Data
{
    /// This is just a data seeder for test situations.
    public static class DataSeeder
    {
        public static async Task SeedProductsAsync(ApplicationDbContext context)
        {
            // Delete all existing related data first (respecting foreign key constraints)
            context.CartItems.RemoveRange(context.CartItems);
            context.OrderItems.RemoveRange(context.OrderItems);
            context.ShipmentItems.RemoveRange(context.ShipmentItems);
            context.Inventories.RemoveRange(context.Inventories);
            context.Products.RemoveRange(context.Products);
            await context.SaveChangesAsync();

            // Reset identity seed for SQL Server
            await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Products', RESEED, 0)");
            await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Inventories', RESEED, 0)");

            var products = new List<Product>
            {
                // ==================== EXTERIOR ====================
                new Product { ItemCode = "EXT-001", ProductName = "Headlight", Brand = "JMC", Category = "EXTERIOR", CostPrice = 12500, SellingPrice = 16000, Description = "JMC Headlight - ON THE WAY" },
                new Product { ItemCode = "EXT-002", ProductName = "Tail Light", Brand = "JMC", Category = "EXTERIOR", CostPrice = 6500, SellingPrice = 7000, Description = "JMC Tail Light - ON THE WAY" },
                new Product { ItemCode = "EXT-003", ProductName = "Signal Light", Brand = "JMC", Category = "EXTERIOR", CostPrice = 1200, SellingPrice = 1500, Description = "JMC Signal Light - ON THE WAY" },
                new Product { ItemCode = "EXT-004", ProductName = "Marker Light", Brand = "JMC", Category = "EXTERIOR", CostPrice = 900, SellingPrice = 1000, Description = "JMC Marker Light - ON THE WAY" },
                new Product { ItemCode = "EXT-005", ProductName = "Inside Door Handle", Brand = "JMC", Category = "EXTERIOR", CostPrice = 650, SellingPrice = 800, Description = "JMC Inside Door Handle - ON THE WAY" },
                new Product { ItemCode = "EXT-006", ProductName = "Outside Door Handle", Brand = "JMC", Category = "EXTERIOR", CostPrice = 650, SellingPrice = 800, Description = "JMC Outside Door Handle - ON THE WAY" },
                new Product { ItemCode = "EXT-007", ProductName = "Climb Handle", Brand = "JMC", Category = "EXTERIOR", CostPrice = 550, SellingPrice = 700, Description = "JMC Climb Handle - ON THE WAY" },

                // ==================== ENGINE ====================
                new Product { ItemCode = "DP1-9J459-AA", ProductName = "One way valve", Brand = "ISUZU", Category = "ENGINE", CostPrice = 3500, SellingPrice = 4600, Description = "ISUZU One way valve - FOR DELIVERY" },
                new Product { ItemCode = "DN1-9J459-AA", ProductName = "One way valve", Brand = "JMC", Category = "ENGINE", CostPrice = 3500, SellingPrice = 4600, Description = "JMC One way valve - PENDING APPROVAL" },
                new Product { ItemCode = "200C18", ProductName = "Fuel Nozzle Valve", Brand = "BOSCH", Category = "ENGINE", CostPrice = 1800, SellingPrice = 2500, Description = "BOSCH Fuel Nozzle Valve - FOR DELIVERY" },
                new Product { ItemCode = "1008080ABA", ProductName = "Air Temperature sensor", Brand = "BOSCH", Category = "ENGINE", CostPrice = 1250, SellingPrice = 1800, Description = "BOSCH Air Temperature sensor - ON THE WAY" },
                new Product { ItemCode = "1207100AAJ", ProductName = "EGR Valve", Brand = "JMC", Category = "ENGINE", CostPrice = 0, SellingPrice = 0, Description = "JMC EGR Valve - PENDING APPROVAL" },
                new Product { ItemCode = "V13X850", ProductName = "Power Steering Fan Belt", Brand = "GATES", Category = "ENGINE", CostPrice = 300, SellingPrice = 350, Description = "GATES Power Steering Fan Belt - FOR DELIVERY" },
                new Product { ItemCode = "6PK1050", ProductName = "Alternator Fan Belt", Brand = "GATES", Category = "ENGINE", CostPrice = 800, SellingPrice = 1000, Description = "GATES Alternator Fan Belt - FOR DELIVERY" },
                new Product { ItemCode = "6PK1480", ProductName = "Aircon Fan Belt", Brand = "GATES", Category = "ENGINE", CostPrice = 950, SellingPrice = 1100, Description = "GATES Aircon Fan Belt - FOR DELIVERY" },
                new Product { ItemCode = "110902109", ProductName = "Turbo Intake Hose", Brand = "JMC", Category = "ENGINE", CostPrice = 2500, SellingPrice = 2800, Description = "JMC Turbo Intake Hose - FOR DELIVERY" },
                new Product { ItemCode = "1306200SBJD1", ProductName = "Upper Thermostat Housing", Brand = "JMC", Category = "ENGINE", CostPrice = 0, SellingPrice = 0, Description = "JMC Upper Thermostat Housing - FOR DELIVERY" },
                new Product { ItemCode = "BC1-6300-AA", ProductName = "Tensioner Bearing", Brand = "Foton", Category = "ENGINE", CostPrice = 2400, SellingPrice = 0, Description = "Foton Tensioner Bearing - FOR DELIVERY" },
                new Product { ItemCode = "BC1-6A013-AA", ProductName = "Idler Bearing", Brand = "Foton", Category = "ENGINE", CostPrice = 2200, SellingPrice = 2500, Description = "Foton Idler Bearing - ON THE WAY" },
                new Product { ItemCode = "1006060TARD1", ProductName = "Timing Belt", Brand = "JMC", Category = "ENGINE", CostPrice = 4400, SellingPrice = 5000, Description = "JMC Timing Belt - FOR DELIVERY" },
                new Product { ItemCode = "1118300ABY", ProductName = "Turbo Kit", Brand = "GARRETT", Category = "ENGINE", CostPrice = 0, SellingPrice = 0, Description = "GARRETT Turbo Kit - ON THE WAY" },

                // ==================== TRANSMISSION ====================
                new Product { ItemCode = "CN6C157750A", ProductName = "Clutch Disc", Brand = "JMC/HASCO", Category = "TRANSMISSION", CostPrice = 3300, SellingPrice = 3500, Description = "JMC/HASCO Clutch Disc - ON THE WAY" },
                new Product { ItemCode = "160110014", ProductName = "Pressure Plate", Brand = "JMC/HASCO", Category = "TRANSMISSION", CostPrice = 4000, SellingPrice = 4500, Description = "JMC/HASCO Pressure Plate - FOR DELIVERY" },
                new Product { ItemCode = "038J-1601307", ProductName = "Release Bearing", Brand = "FOTON", Category = "TRANSMISSION", CostPrice = 2000, SellingPrice = 2300, Description = "FOTON Release Bearing - ON THE WAY" },
                new Product { ItemCode = "62RCT3530F2", ProductName = "Release Bearing", Brand = "JMC", Category = "TRANSMISSION", CostPrice = 2000, SellingPrice = 2400, Description = "JMC Release Bearing - INSTOCK" },
                new Product { ItemCode = "112267400046", ProductName = "Fuel water positioning sensor", Brand = "BOSCH", Category = "TRANSMISSION", CostPrice = 1100, SellingPrice = 1700, Description = "BOSCH Fuel water positioning sensor - ON THE WAY" },
                new Product { ItemCode = "3729100A", ProductName = "Reverse Sensor", Brand = "JMC", Category = "TRANSMISSION", CostPrice = 0, SellingPrice = 0, Description = "JMC Reverse Sensor - PENDING APPROVAL" },
                new Product { ItemCode = "GN1-9E731-AA", ProductName = "Odometer Speed Sensor", Brand = "JMC", Category = "TRANSMISSION", CostPrice = 900, SellingPrice = 1100, Description = "JMC Odometer Speed Sensor - ON THE WAY" },
                new Product { ItemCode = "281002315", ProductName = "Crank position sensor", Brand = "BOSCH", Category = "TRANSMISSION", CostPrice = 1300, SellingPrice = 2000, Description = "BOSCH Crank position sensor - ON THE WAY" },
                new Product { ItemCode = "354920001", ProductName = "Exhaust Brake Solenoid", Brand = "BOSCH", Category = "TRANSMISSION", CostPrice = 2700, SellingPrice = 4100, Description = "BOSCH Exhaust Brake Solenoid - PENDING APPROVAL" },
                new Product { ItemCode = "281006207", ProductName = "Exhaust Air Pressure Sensor", Brand = "BOSCH", Category = "TRANSMISSION", CostPrice = 2400, SellingPrice = 3700, Description = "BOSCH Exhaust Air Pressure Sensor - ON THE WAY" },
                new Product { ItemCode = "63/32YA1", ProductName = "Pilot Bearing", Brand = "", Category = "TRANSMISSION", CostPrice = 0, SellingPrice = 0, Description = "Pilot Bearing - NEED ASSESSMENT" },

                // ==================== UNDERCHASIS ====================
                new Product { ItemCode = "2900024A1", ProductName = "Leaf Spring Pin", Brand = "JMC", Category = "UNDERCHASIS", CostPrice = 500, SellingPrice = 0, Description = "JMC Leaf Spring Pin - ON THE WAY" },
                new Product { ItemCode = "2900024A", ProductName = "Leaf Spring Bushing", Brand = "JMC", Category = "UNDERCHASIS", CostPrice = 0, SellingPrice = 0, Description = "JMC Leaf Spring Bushing - ON THE WAY" },
                new Product { ItemCode = "SE-5171R", ProductName = "Tire Rod Ends", Brand = "555 Japan", Category = "UNDERCHASIS", CostPrice = 2000, SellingPrice = 2500, Description = "555 Japan Tire Rod Ends - FOR DELIVERY" },
                new Product { ItemCode = "MI-08", ProductName = "King Pin", Brand = "MRK", Category = "UNDERCHASIS", CostPrice = 1300, SellingPrice = 1500, Description = "MRK King Pin - FOR DELIVERY" },
                new Product { ItemCode = "32210", ProductName = "Wheel Bearing (Front) Inner", Brand = "NSK", Category = "UNDERCHASIS", CostPrice = 900, SellingPrice = 1300, Description = "NSK Wheel Bearing (Front) Inner - FOR DELIVERY" },
                new Product { ItemCode = "32207", ProductName = "Wheel Bearing (Front) Outer", Brand = "NSK", Category = "UNDERCHASIS", CostPrice = 600, SellingPrice = 900, Description = "NSK Wheel Bearing (Front) Outer - FOR DELIVERY" },
                new Product { ItemCode = "29587/20", ProductName = "Wheel Bearing (Back) Inner", Brand = "NSK", Category = "UNDERCHASIS", CostPrice = 1200, SellingPrice = 1300, Description = "NSK Wheel Bearing (Back) Inner - ON THE WAY" },
                new Product { ItemCode = "28680/22", ProductName = "Wheel Bearing (Back) Outer", Brand = "NSK", Category = "UNDERCHASIS", CostPrice = 1000, SellingPrice = 1200, Description = "NSK Wheel Bearing (Back) Outer - ON THE WAY" },
                new Product { ItemCode = "73-90", ProductName = "Wheel Bearing Oil Seal Front", Brand = "ISUZU", Category = "UNDERCHASIS", CostPrice = 0, SellingPrice = 0, Description = "ISUZU Wheel Bearing Oil Seal Front - ON THE WAY" },
                new Product { ItemCode = "80-113-12/22", ProductName = "Wheel Bearing Oil Seal Rear", Brand = "ISUZU", Category = "UNDERCHASIS", CostPrice = 0, SellingPrice = 0, Description = "ISUZU Wheel Bearing Oil Seal Rear - ON THE WAY" },
                new Product { ItemCode = "8cm/10cm", ProductName = "Brake Shoe", Brand = "", Category = "UNDERCHASIS", CostPrice = 0, SellingPrice = 0, Description = "Brake Shoe - NEED ASSESSMENT" },
                new Product { ItemCode = "1104916200039", ProductName = "Clutch Slave Cylinder", Brand = "FOTON", Category = "UNDERCHASIS", CostPrice = 1800, SellingPrice = 2300, Description = "FOTON Clutch Slave Cylinder - ON THE WAY" },
                new Product { ItemCode = "UC-SEAL-001", ProductName = "Axle Oil Seal 49x94x8/9.5", Brand = "", Category = "UNDERCHASIS", CostPrice = 0, SellingPrice = 0, Description = "Axle Oil Seal 49x94x8/9.5 - PENDING APPROVAL" },

                // ==================== FILTER ====================
                new Product { ItemCode = "C-512", ProductName = "Oil Filter", Brand = "VIC", Category = "FILTER", CostPrice = 400, SellingPrice = 550, Description = "VIC Oil Filter - FOR DELIVERY" },
                new Product { ItemCode = "MFC-016", ProductName = "Fuel Filter (Primary)", Brand = "POWER MAX", Category = "FILTER", CostPrice = 580, SellingPrice = 690, Description = "POWER MAX Fuel Filter (Primary) - FOR DELIVERY" },
                new Product { ItemCode = "MFC-017", ProductName = "Fuel Filter (Secondary)", Brand = "POWER MAX", Category = "FILTER", CostPrice = 400, SellingPrice = 510, Description = "POWER MAX Fuel Filter (Secondary) - FOR DELIVERY" },
                new Product { ItemCode = "FC-016", ProductName = "Fuel Filter (Primary)", Brand = "VIC", Category = "FILTER", CostPrice = 900, SellingPrice = 950, Description = "VIC Fuel Filter (Primary) - FOR DELIVERY" },
                new Product { ItemCode = "FC-017", ProductName = "Fuel Filter (Secondary)", Brand = "VIC", Category = "FILTER", CostPrice = 900, SellingPrice = 950, Description = "VIC Fuel Filter (Secondary) - FOR DELIVERY" },
                new Product { ItemCode = "110923009", ProductName = "Air Filter", Brand = "JMC", Category = "FILTER", CostPrice = 1300, SellingPrice = 0, Description = "JMC Air Filter - FOR DELIVERY" }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            // Create inventory records for all products with random stock
            var random = new Random();
            var inventories = products.Select(p => new Inventory
            {
                ProductId = p.ProductId,
                QuantityInStock = random.Next(5, 50), // Random stock between 5 and 50
                ReorderLevel = 5,
                LastUpdated = DateTime.Now
            }).ToList();

            await context.Inventories.AddRangeAsync(inventories);
            await context.SaveChangesAsync();
        }
    }
}
