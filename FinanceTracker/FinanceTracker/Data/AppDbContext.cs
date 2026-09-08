using FinanceTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Category>()
                .HasIndex(c => new { c.UserId, c.Name })
                .IsUnique();

            modelBuilder.Entity<Category>()
                .Property(c => c.Name)
                .HasMaxLength(50);

            modelBuilder.Entity<Category>()
                .Property(c => c.Color)
                .HasMaxLength(7);

            modelBuilder.Entity<Category>()
                .Property(c => c.Icon)
                .HasMaxLength(50);

            modelBuilder.Entity<Budget>()
                .Property(b => b.LimitAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Budget>()
                .HasIndex(b => new { b.UserId, b.CategoryId, b.Month, b.Year })
                .IsUnique();

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(r => r.Token)
                .IsUnique();

            modelBuilder.Entity<UserSettings>()
                .HasOne(settings => settings.User)
                .WithOne(user => user.Settings)
                .HasForeignKey<UserSettings>(settings => settings.UserId);
        }
    }
}
