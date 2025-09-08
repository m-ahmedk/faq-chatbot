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
            await SeedFaqs(service);
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

        public async static Task SeedFaqs(IServiceProvider service)
        {
            using var scope = service.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var faqService = scope.ServiceProvider.GetRequiredService<IFaqService>();

            if (!context.Faqs.Any())
            {
                var faqs = new List<Faq>
                {
                    new Faq
                    {
                        Question = "How do I register?",
                        Answer = "Use the /api/auth/register endpoint with email and password to create an account."
                    },
                    new Faq
                    {
                        Question = "How do I log in?",
                        Answer = "Use the /api/auth/login endpoint to receive a JWT token for authentication."
                    },
                    new Faq
                    {
                        Question = "How can I place an order?",
                        Answer = "Send a POST request to /api/orders with product IDs and quantities to place an order."
                    },
                    new Faq
                    {
                        Question = "Can I order multiple products at once?",
                        Answer = "Yes, you can include multiple items in the same order request to /api/orders."
                    },
                    new Faq
                    {
                        Question = "Do you ship internationally?",
                        Answer = "Yes, worldwide shipping is supported. Delivery typically takes 3–5 business days."
                    },
                    new Faq
                    {
                        Question = "How long does shipping take?",
                        Answer = "Shipping usually takes 3–5 business days, depending on your location."
                    },
                    new Faq
                    {
                        Question = "What payment methods do you support?",
                        Answer = "We currently support credit card and PayPal payments."
                    },
                    new Faq
                    {
                        Question = "What is your return policy?",
                        Answer = "Items can be returned within 30 days of purchase for a full refund."
                    },
                    new Faq
                    {
                        Question = "How do I reset my password?",
                        Answer = "Click 'Forgot Password' on the login page to reset your password."
                    },
                    new Faq
                    {
                        Question = "Is my account secure?",
                        Answer = "Yes, your data is protected with JWT authentication and secure password hashing."
                    },
                    new Faq
                    {
                        Question = "Can I track my orders?",
                        Answer = "Yes, use GET /api/orders/{id} to check the status of your order."
                    },
                    new Faq
                    {
                        Question = "Can I cancel my order?",
                        Answer = "Yes, orders can be cancelled before they are shipped using the /api/orders/cancel endpoint."
                    },
                    new Faq
                    {
                        Question = "How do I contact support?",
                        Answer = "You can email support@example.com or use the /api/support endpoint."
                    }
                };

                foreach (var faq in faqs)
                {
                    await faqService.AddAsync(faq); // generates embeddings internally
                }
            }
        }

    }
}
