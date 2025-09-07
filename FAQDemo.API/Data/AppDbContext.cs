using FAQDemo.API.Models;
using FAQDemo.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FAQDemo.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Embedding> Embeddings { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        // Autosave params with EF Core
        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added: 
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.LastModifiedAt = DateTime.UtcNow;
                        entry.Entity.IsDeleted = false;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified; // Add this line to prevent hard delete / data removal
                        entry.Entity.IsDeleted = true;
                        entry.Entity.LastModifiedAt = DateTime.UtcNow;
                        entry.Entity.DeletedAt = DateTime.UtcNow;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        // global filter to remove isDeleted data from return queries
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Enable pgvector extension in Postgres
            modelBuilder.HasPostgresExtension("vector");

            // Map Embedding.Vector to Postgres vector(1536)
            modelBuilder.Entity<Embedding>()
                .Property(e => e.Vector)
                .HasColumnType("vector(1536)");

            // REFLECTION: Get all entity types in the current model
            foreach (var entityType in modelBuilder.Model.GetEntityTypes()) {

                // REFLECTION: Check if entityType inherits from BaseEntity
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType)) {

                    // EXPRESSION TREE: Build the expression e => e.IsDeleted == false

                    // Create parameter 'e'
                    var parameter = Expression.Parameter(entityType.ClrType, "e");

                    // Access property: e.IsDeleted
                    var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));

                    // Compare: e.IsDeleted == false
                    var comparison = Expression.Equal(property, Expression.Constant(false));

                    // Build lambda: e => e.IsDeleted == false
                    var lambda = Expression.Lambda(comparison, parameter);

                    // Apply it as query filter
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }

            // Order -> User (many orders per user)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId);

            // OrderItem -> Order (many items per order)
            modelBuilder.Entity<OrderItem>()
                .HasOne(i => i.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(i => i.OrderId);

            // OrderItem -> Product (many order items can reference a product)
            modelBuilder.Entity<OrderItem>()
                .HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId);

            // Composite key and relationships for UserRole
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
        }

    }
}