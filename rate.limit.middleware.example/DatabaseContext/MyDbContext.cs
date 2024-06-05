using Microsoft.EntityFrameworkCore;
using rate.limit.middleware.example.Models;
using rate.limit.middleware.example.RateLimitingMiddleware;

namespace rate.limit.middleware.example.DatabaseContext
{
    public class MyDatabaseContext : DbContext
    {
        public MyDatabaseContext(DbContextOptions<MyDatabaseContext> options) : base(options)
        {
        }

        // DbSet for RateLimitRecord entity
        public DbSet<RateLimitRecord> RateLimitRecords { get; set; }

        // Add other DbSet properties for additional entities if needed

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure RateLimitRecord entity properties and relationships if needed

            // Example configuration:
            modelBuilder.Entity<RateLimitRecord>(entity =>
            {
                entity.HasKey(e => e.Identifier);
                entity.Property(e => e.Identifier).IsRequired();
                entity.Property(e => e.Count).IsRequired();
                entity.Property(e => e.LastReset).IsRequired();
                entity.Property(e => e.ResetInterval).IsRequired();
                // Add other configurations as needed
            });

            // Call base method
            base.OnModelCreating(modelBuilder);
        }
    }
}
