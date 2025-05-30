using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class FinancialDbContext : DbContext
    {
        public DbSet<ConcreteFinancialTransaction> Transactions { get; set; }
        public DbSet<ConcreteUserEvent> Events { get; set; }
        public DbSet<User> Users { get; set; }

        public FinancialDbContext(DbContextOptions<FinancialDbContext> options) : base(options)
        {
        }

        // Parameterless constructor for testing
        public FinancialDbContext() : base()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ConcreteFinancialTransaction
            modelBuilder.Entity<ConcreteFinancialTransaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            });

            // Configure ConcreteUserEvent
            modelBuilder.Entity<ConcreteUserEvent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).HasMaxLength(1000);
            });

            // Configure User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            });
        }

        // Default configuration for development
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Use SQL Server LocalDB for development
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FinancialApp;Trusted_Connection=true;MultipleActiveResultSets=true;");

                // For testing, you can also use In-Memory database:
                // optionsBuilder.UseInMemoryDatabase("FinancialAppTestDb");
            }
        }
    }
}