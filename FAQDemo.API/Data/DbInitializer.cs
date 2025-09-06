using FAQDemo.API.Helpers;
using FAQDemo.API.Models;
using FAQDemo.API.Services;
using FAQDemo.API.Services.Interfaces; // for IEmbeddingService

namespace FAQDemo.API.Data
{
    public static class DbInitializer
    {
        public async static Task Seed(IServiceProvider service)
        {
            using var scope = service.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var embeddingService = scope.ServiceProvider.GetRequiredService<IEmbeddingService>();

            await SeedRoles(context);
            await SeedUsers(context);
            await SeedProducts(context, embeddingService);
        }

        public async static Task SeedRoles(AppDbContext context)
        {
            if (!context.Roles.Any())
            {
                var roles = new List<Role>()
                {
                    new Role { Name = "Admin" },
                    new Role {Name = "User"}
                };

                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }
        }

        public async static Task SeedUsers(AppDbContext context)
        {
            if (!context.Users.Any())
            {
                // ---- Admin user ----
                var (adminHash, adminSalt) = HashHelper.HashPassword("ahmed123#");

                var adminUser = new AppUser
                {
                    Email = "mahmedvilla@gmail.com",
                    PasswordHash = adminHash,
                    PasswordStamp = adminSalt,
                    UserRoles = new List<UserRole>()
                };

                Role? adminRole = context.Roles.FirstOrDefault(x => x.Name == "Admin");
                if (adminRole != null)
                {
                    adminUser.UserRoles.Add(new UserRole { Role = adminRole });
                }

                await context.Users.AddAsync(adminUser);

                // ---- Normal user ----
                var (userHash, userSalt) = HashHelper.HashPassword("user123#");

                var normalUser = new AppUser
                {
                    Email = "normaluser@test.com",
                    PasswordHash = userHash,
                    PasswordStamp = userSalt,
                    UserRoles = new List<UserRole>()
                };

                Role? userRole = context.Roles.FirstOrDefault(x => x.Name == "User");
                if (userRole != null)
                {
                    normalUser.UserRoles.Add(new UserRole { Role = userRole });
                }

                await context.Users.AddAsync(normalUser);

                // Save both users
                await context.SaveChangesAsync();
            }
        }

        public async static Task SeedProducts(AppDbContext context, IEmbeddingService embeddingService)
        {
            if (!context.Products.Any())
            {
                var products = new List<Product> {
                    // ---- Soft Drinks / Energy ----
                    new Product { Name = "Coca Cola", Price = 1.5, Quantity = 100 },
                    new Product { Name = "Pepsi", Price = 1.4, Quantity = 80 },
                    new Product { Name = "Sprite", Price = 1.3, Quantity = 70 },
                    new Product { Name = "Fanta", Price = 1.2, Quantity = 60 },
                    new Product { Name = "Mountain Dew", Price = 1.6, Quantity = 90 },
                    new Product { Name = "Red Bull", Price = 2.5, Quantity = 40 },
                    new Product { Name = "Monster Energy", Price = 2.3, Quantity = 50 },

                    // ---- Snacks / Chocolates ----
                    new Product { Name = "Snickers", Price = 1.0, Quantity = 120 },
                    new Product { Name = "KitKat", Price = 0.9, Quantity = 140 },
                    new Product { Name = "Twix", Price = 1.1, Quantity = 110 },
                    new Product { Name = "M&Ms", Price = 1.0, Quantity = 130 },
                    new Product { Name = "Oreo Cookies", Price = 1.2, Quantity = 160 },

                    // ---- Chips / Crisps ----
                    new Product { Name = "Lays Chips", Price = 1.3, Quantity = 200 },
                    new Product { Name = "Doritos", Price = 1.4, Quantity = 180 },
                    new Product { Name = "Pringles", Price = 2.0, Quantity = 70 },

                    // ---- Healthy Options ----
                    new Product { Name = "Almonds", Price = 3.5, Quantity = 60 },
                    new Product { Name = "Granola Bar", Price = 2.5, Quantity = 90 },
                    new Product { Name = "Dried Apricots", Price = 3.0, Quantity = 50 },
                    new Product { Name = "Trail Mix", Price = 3.8, Quantity = 40 }
                };

                // Save products first
                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();

                // Generate embeddings for each product via the service (calls OpenAI + repository)
                foreach (var product in products) {
                    await embeddingService.CreateProductEmbeddingAsync(product);
                    // Console.WriteLine($"[Seed] Created embedding for {product.Name}");
                }
            }
        }

    }
}
