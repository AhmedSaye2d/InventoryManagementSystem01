using System.Security.Claims;
using Application.Extention.Identity;
using Domain.Entities.ActivityTracker;
using Domain.Entities.Inventory;
using Domain.Entities.Order;
using Domain.Entities.Product;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.DataAccess.Seed
{
    public static class AppDbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AppDbSeeder");

            // Ensure DB created / migrated - do not auto-migrate in tests (InMemory)
            try
            {
                if (context.Database.IsRelational())
                    await context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Migrate failed, continuing with seed");
            }

            await SeedIdentityAsync(userManager, roleManager, config, logger);
            await SeedLocationsAsync(context, logger);
            await SeedCategoriesAsync(context, logger);
            await SeedProductsAsync(context, logger);
            await SeedInventoryMovementsAsync(context, userManager, logger);
            await SeedOrdersAsync(context, userManager, logger);
            await SeedActivityTrackersAsync(context, userManager, logger);
        }

        private static async Task SeedIdentityAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config, ILogger logger)
        {
            foreach (var role in new[] { "Admin", "Manager", "User" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    logger.LogInformation("Role created: {Role}", role);
                }
            }

            // Admin from config (env safe)
            var adminEmail = config["SeedAdmin:Email"] ?? "admin@system.com";
            var adminPass = config["SeedAdmin:Password"] ?? "Admin@123456";
            var adminName = config["SeedAdmin:Name"] ?? "System Admin";
            await EnsureUserAsync(userManager, adminEmail, adminPass, adminName, "Admin",
                new Dictionary<string, string> { ["create"] = "true", ["update"] = "true", ["delete"] = "true", ["read"] = "true", ["MangerUser"] = "true" }, logger);

            // Manager
            var managerEmail = config["SeedUsers:Manager:Email"] ?? "manager@system.com";
            var managerPass = config["SeedUsers:Manager:Password"] ?? "Manager@1234";
            var managerName = config["SeedUsers:Manager:Name"] ?? "Inventory Manager";
            await EnsureUserAsync(userManager, managerEmail, managerPass, managerName, "Manager",
                new Dictionary<string, string> { ["create"] = "true", ["update"] = "true", ["delete"] = "false", ["read"] = "true", ["MangerUser"] = "false" }, logger);

            // User
            var userEmail = config["SeedUsers:User:Email"] ?? "user@system.com";
            var userPass = config["SeedUsers:User:Password"] ?? "User@12345";
            var userName = config["SeedUsers:User:Name"] ?? "Store User";
            await EnsureUserAsync(userManager, userEmail, userPass, userName, "User",
                new Dictionary<string, string> { ["create"] = "false", ["update"] = "false", ["delete"] = "false", ["read"] = "true", ["MangerUser"] = "false" }, logger);
        }

        private static async Task EnsureUserAsync(UserManager<ApplicationUser> userManager, string email, string password, string name, string role, Dictionary<string, string> permissions, ILogger logger)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                // Ensure role assigned
                if (!await userManager.IsInRoleAsync(user, role))
                    await userManager.AddToRoleAsync(user, role);
                return;
            }

            user = new ApplicationUser { UserName = email, Email = email, Name = name, EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                logger.LogWarning("Failed to create {Email}: {Errors}", email, string.Join(",", result.Errors.Select(e => e.Description)));
                return;
            }

            await userManager.AddToRoleAsync(user, role);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim("Name", name)
            };
            foreach (var kv in permissions)
                claims.Add(new Claim(kv.Key, kv.Value));

            await userManager.AddClaimsAsync(user, claims);
            logger.LogInformation("Seeded user {Email} as {Role}", email, role);
        }

        private static async Task SeedLocationsAsync(AppDbContext ctx, ILogger logger)
        {
            var names = new[] { "A-01", "A-02", "A-03", "B-01", "B-02", "B-03", "C-01", "C-02" };
            var existing = await ctx.Locations.Select(l => l.Name).ToListAsync();
            var toAdd = new List<Location>();
            foreach (var n in names)
                if (!existing.Contains(n))
                    toAdd.Add(new Location { Id = Guid.NewGuid(), Name = n });

            if (toAdd.Any())
            {
                ctx.Locations.AddRange(toAdd);
                await ctx.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} locations", toAdd.Count);
            }
        }

        private static async Task SeedCategoriesAsync(AppDbContext ctx, ILogger logger)
        {
            var names = new[] { "Computers", "Electronics", "Accessories", "Storage", "Networking", "Peripherals" };
            var existing = await ctx.Categories.Select(c => c.Name).ToListAsync();
            var toAdd = new List<Category>();
            foreach (var n in names)
                if (!existing.Contains(n))
                    toAdd.Add(new Category { Id = Guid.NewGuid(), Name = n });
            if (toAdd.Any())
            {
                ctx.Categories.AddRange(toAdd);
                await ctx.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} categories", toAdd.Count);
            }
        }

        private static async Task SeedProductsAsync(AppDbContext ctx, ILogger logger)
        {
            // Idempotent by SerialNumber
            if (await ctx.Products.AnyAsync()) return;

            var locations = await ctx.Locations.ToDictionaryAsync(l => l.Name, l => l.Id);
            var categories = await ctx.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);

            Guid Loc(string n) => locations[n];
            Guid Cat(string n) => categories[n];

            var products = new List<Product>
            {
                new Product { Id=Guid.NewGuid(), Name="HP Laptop G9", CategoryId=Cat("Computers"), LocationId=Loc("A-01"), Price=18500m, Quantity=45, SerialNumber="SN-HP001", Description="HP Laptop G9 Core i7 16GB", Base64Image="", DateAdd=DateTime.UtcNow.AddDays(-30) },
                new Product { Id=Guid.NewGuid(), Name="Dell XPS 15", CategoryId=Cat("Computers"), LocationId=Loc("A-02"), Price=28900m, Quantity=32, SerialNumber="SN-DELL002", Description="Dell XPS 15 32GB RAM", Base64Image="", DateAdd=DateTime.UtcNow.AddDays(-28) },
                new Product { Id=Guid.NewGuid(), Name="Lenovo ThinkPad X1", CategoryId=Cat("Computers"), LocationId=Loc("A-03"), Price=24500m, Quantity=28, SerialNumber="SN-LEN003", Description="Lenovo ThinkPad X1 Carbon", Base64Image="", DateAdd=DateTime.UtcNow.AddDays(-25) },
                new Product { Id=Guid.NewGuid(), Name="HP LaserJet Printer", CategoryId=Cat("Electronics"), LocationId=Loc("B-01"), Price=5600m, Quantity=18, SerialNumber="SN-HP004", Description="HP LaserJet Pro M404", Base64Image="", DateAdd=DateTime.UtcNow.AddDays(-20) },
                new Product { Id=Guid.NewGuid(), Name="Dell Monitor 27\"", CategoryId=Cat("Peripherals"), LocationId=Loc("B-02"), Price=4200m, Quantity=60, SerialNumber="SN-DELL005", Description="Dell 27 inch 4K Monitor", Base64Image="", DateAdd=DateTime.UtcNow.AddDays(-18) },
                new Product { Id=Guid.NewGuid(), Name="Logitech MX Master Mouse", CategoryId=Cat("Accessories"), LocationId=Loc("B-03"), Price=850m, Quantity=120, SerialNumber="SN-LOG006", Description="Logitech MX Master 3S", Base64Image="", DateAdd=DateTime.UtcNow.AddDays(-15) },
                new Product { Id=Guid.NewGuid(), Name="Mechanical Keyboard RGB", CategoryId=Cat("Accessories"), LocationId=Loc("C-01"), Price=1200m, Quantity=85, SerialNumber="SN-KEY007", Description="Mechanical Keyboard Blue Switch", Base64Image="", DateAdd=DateTime.UtcNow.AddDays(-12) },
                new Product { Id=Guid.NewGuid(), Name="Samsung SSD 1TB NVMe", CategoryId=Cat("Storage"), LocationId=Loc("C-02"), Price=1800m, Quantity=90, SerialNumber="SN-SSD008", Description="Samsung 980 Pro 1TB NVMe", Base64Image="", DateAdd=DateTime.UtcNow.AddDays(-10) },
                new Product { Id=Guid.NewGuid(), Name="Kingston RAM 16GB DDR4", CategoryId=Cat("Storage"), LocationId=Loc("A-01"), Price=950m, Quantity=70, SerialNumber="SN-RAM009", Description="Kingston 16GB 3200MHz", Base64Image="", DateAdd=DateTime.UtcNow.AddDays(-8) },
                new Product { Id=Guid.NewGuid(), Name="Cisco Switch 24-Port", CategoryId=Cat("Networking"), LocationId=Loc("B-01"), Price=7500m, Quantity=12, SerialNumber="SN-CIS010", Description="Cisco Catalyst 24-Port Gigabit", Base64Image="", DateAdd=DateTime.UtcNow.AddDays(-5) },
            };

            // Ensure categories/locations exist (already seeded)
            ctx.Products.AddRange(products);
            await ctx.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} products", products.Count);
        }

        private static async Task SeedInventoryMovementsAsync(AppDbContext ctx, UserManager<ApplicationUser> userManager, ILogger logger)
        {
            if (await ctx.InventoryMovements.AnyAsync()) return;

            var manager = await userManager.FindByEmailAsync("manager@system.com");
            var admin = await userManager.FindByEmailAsync("admin@system.com");
            var managerId = manager?.Id ?? admin?.Id ?? "system";
            var adminId = admin?.Id ?? managerId;

            var products = await ctx.Products.ToListAsync();
            var locById = await ctx.Locations.ToDictionaryAsync(l => l.Id, l => l.Id);
            // Use product.LocationId as primary location
            var movements = new List<InventoryMovement>();
            var now = DateTime.UtcNow;

            foreach (var p in products)
            {
                // StockIn initial
                movements.Add(new InventoryMovement
                {
                    Id = Guid.NewGuid(), ProductId = p.Id, LocationId = p.LocationId, Quantity = 50,
                    MovementType = MovementType.StockIn, Reason = "Initial stock - Purchase Order PO-2025",
                    CreatedAt = now.AddDays(-20), UserId = managerId, QuantityAfter = p.Quantity + 10, ReferenceId = $"PO-{p.SerialNumber}"
                });
                // StockOut sale
                movements.Add(new InventoryMovement
                {
                    Id = Guid.NewGuid(), ProductId = p.Id, LocationId = p.LocationId, Quantity = -5,
                    MovementType = MovementType.StockOut, Reason = "Customer sale - Order",
                    CreatedAt = now.AddDays(-15), UserId = managerId, QuantityAfter = p.Quantity + 5, ReferenceId = $"ORD-{p.SerialNumber}"
                });
                // Adjustment damaged
                if (p.Name.Contains("Mouse") || p.Name.Contains("Keyboard"))
                {
                    movements.Add(new InventoryMovement
                    {
                        Id = Guid.NewGuid(), ProductId = p.Id, LocationId = p.LocationId, Quantity = -3,
                        MovementType = MovementType.Adjustment, Reason = "Damaged during handling",
                        CreatedAt = now.AddDays(-10), UserId = adminId, QuantityAfter = p.Quantity, ReferenceId = $"ADJ-{p.SerialNumber}"
                    });
                }
                // Return
                if (p.Name.Contains("HP Laptop") || p.Name.Contains("SSD"))
                {
                    movements.Add(new InventoryMovement
                    {
                        Id = Guid.NewGuid(), ProductId = p.Id, LocationId = p.LocationId, Quantity = 2,
                        MovementType = MovementType.Return, Reason = "Customer return - undamaged",
                        CreatedAt = now.AddDays(-8), UserId = managerId, QuantityAfter = p.Quantity + 2
                    });
                }
            }

            // Relocation example: HP Laptop from A-01 -> B-02 (pair)
            var hp = products.FirstOrDefault(x => x.SerialNumber == "SN-HP001");
            var b02 = await ctx.Locations.FirstOrDefaultAsync(l => l.Name == "B-02");
            if (hp != null && b02 != null)
            {
                var corr = Guid.NewGuid().ToString();
                movements.Add(new InventoryMovement
                {
                    Id = Guid.NewGuid(), ProductId = hp.Id, LocationId = hp.LocationId, Quantity = -10,
                    MovementType = MovementType.RelocationOut, Reason = "Relocation A-01 -> B-02",
                    CreatedAt = now.AddDays(-5), UserId = managerId, QuantityAfter = hp.Quantity - 10, ReferenceId = corr
                });
                movements.Add(new InventoryMovement
                {
                    Id = Guid.NewGuid(), ProductId = hp.Id, LocationId = b02.Id, Quantity = 10,
                    MovementType = MovementType.RelocationIn, Reason = "Relocation A-01 -> B-02",
                    CreatedAt = now.AddDays(-5).AddMinutes(1), UserId = managerId, QuantityAfter = 10, ReferenceId = corr
                });
            }

            ctx.InventoryMovements.AddRange(movements);
            await ctx.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} inventory movements", movements.Count);
        }

        private static async Task SeedOrdersAsync(AppDbContext ctx, UserManager<ApplicationUser> userManager, ILogger logger)
        {
            if (await ctx.Orders.AnyAsync()) return;

            var products = await ctx.Products.Take(6).ToListAsync();
            var user = await userManager.FindByEmailAsync("user@system.com");
            var manager = await userManager.FindByEmailAsync("manager@system.com");
            var clientId = user?.Id ?? manager?.Id ?? "test-client";

            var statuses = new[] { "Pending", "Processing", "Delivering", "Delivered", "Cancelled" };
            var orders = new List<Order>();
            var now = DateTime.UtcNow;
            for (int i = 0; i < 8; i++)
            {
                var p = products[i % products.Count];
                var qty = (i % 3) + 1;
                orders.Add(new Order
                {
                    Id = Guid.NewGuid(),
                    ProductId = p.Id,
                    ClientId = clientId,
                    Quantity = qty,
                    Price = p.Price,
                    TotalAmount = p.Price * qty,
                    DateOrder = now.AddDays(-(10 - i)),
                    DeliveryDate = now.AddDays(-(10 - i) + 3),
                    OrderStatus = statuses[i % statuses.Length]
                });
            }

            ctx.Orders.AddRange(orders);
            await ctx.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} orders", orders.Count);
        }

        private static async Task SeedActivityTrackersAsync(AppDbContext ctx, UserManager<ApplicationUser> userManager, ILogger logger)
        {
            if (await ctx.ActivityTrackers.AnyAsync()) return;

            var admin = await userManager.FindByEmailAsync("admin@system.com");
            var manager = await userManager.FindByEmailAsync("manager@system.com");
            var adminId = admin?.Id ?? "system";
            var managerId = manager?.Id ?? adminId;

            var trackers = new List<Tracker>
            {
                new Tracker { Title="System Setup", Operation="Setup", Description="Seeded initial data", Date=DateTime.UtcNow.AddDays(-20), UserId=adminId, OperationStatus=true },
                new Tracker { Title="Stock In", Operation="Inventory", Description="Initial StockIn for HP Laptop", Date=DateTime.UtcNow.AddDays(-15), UserId=managerId, OperationStatus=true },
                new Tracker { Title="Login", Operation="Auth", Description="Manager login", Date=DateTime.UtcNow.AddDays(-10), UserId=managerId, OperationStatus=true },
            };
            ctx.ActivityTrackers.AddRange(trackers);
            await ctx.SaveChangesAsync();
            logger.LogInformation("Seeded trackers");
        }
    }
}
