using FinanceTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Users.AnyAsync())
            {
                return;
            }

            var user = new User
            {
                FullName = "Melvin Bonde",
                Email = "melvin@example.com",
                PasswordHash = "$2a$11$0JgO9iC/QM6z.QKae1Euc.S85p9OmQ9KyRwfo9tfxoNveoYavxHo2",
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(user);

            await context.SaveChangesAsync();

            var categories = new List<Category>
        {
            new()
            {
                Name = "Food",
                Icon = "🛒",
                Color = "#10B981",
                UserId = user.Id
            },

            new()
            {
                Name = "Transport",
                Icon = "🚌",
                Color = "#8B5CF6",
                UserId = user.Id
            },

            new()
            {
                Name = "Utilities",
                Icon = "⚡",
                Color = "#F59E0B",
                UserId = user.Id
            },

            new()
            {
                Name = "Entertainment",
                Icon = "🎬",
                Color = "#F43F5E",
                UserId = user.Id
            },

            new()
            {
                Name = "Shopping",
                Icon = "🛍️",
                Color = "#3B82F6",
                UserId = user.Id
            },

            new()
            {
                Name = "Health",
                Icon = "💊",
                Color = "#14B8A6",
                UserId = user.Id
            },

            new()
            {
                Name = "Income",
                Icon = "💼",
                Color = "#22C55E",
                UserId = user.Id
            }
        };

            context.Categories.AddRange(categories);

            await context.SaveChangesAsync();

            var food = categories.First(c => c.Name == "Food");
            var transport = categories.First(c => c.Name == "Transport");
            var utilities = categories.First(c => c.Name == "Utilities");
            var entertainment = categories.First(c => c.Name == "Entertainment");
            var shopping = categories.First(c => c.Name == "Shopping");
            var health = categories.First(c => c.Name == "Health");
            var income = categories.First(c => c.Name == "Income");

            var transactions = new List<Transaction>
        {
            new()
            {
                Description = "Monthly salary",
                Amount = 48500m,
                Date = new DateTime(2026, 7, 1),
                IsExpense = false,
                UserId = user.Id,
                CategoryId = income.Id
            },

            new()
            {
                Description = "Grocery shopping",
                Amount = 4200m,
                Date = new DateTime(2026, 7, 3),
                IsExpense = true,
                UserId = user.Id,
                CategoryId = food.Id
            },

            new()
            {
                Description = "Electric bill",
                Amount = 2400m,
                Date = new DateTime(2026, 7, 5),
                IsExpense = true,
                UserId = user.Id,
                CategoryId = utilities.Id
            },

            new()
            {
                Description = "Grab rides",
                Amount = 1800m,
                Date = new DateTime(2026, 7, 7),
                IsExpense = true,
                UserId = user.Id,
                CategoryId = transport.Id
            },

            new()
            {
                Description = "Netflix subscription",
                Amount = 599m,
                Date = new DateTime(2026, 7, 8),
                IsExpense = true,
                UserId = user.Id,
                CategoryId = entertainment.Id
            },

            new()
            {
                Description = "New headphones",
                Amount = 3500m,
                Date = new DateTime(2026, 7, 10),
                IsExpense = true,
                UserId = user.Id,
                CategoryId = shopping.Id
            },

            new()
            {
                Description = "Freelance payment",
                Amount = 8000m,
                Date = new DateTime(2026, 7, 12),
                IsExpense = false,
                UserId = user.Id,
                CategoryId = income.Id
            },

            new()
            {
                Description = "Pharmacy purchase",
                Amount = 850m,
                Date = new DateTime(2026, 7, 14),
                IsExpense = true,
                UserId = user.Id,
                CategoryId = health.Id
            },

            new()
            {
                Description = "Restaurant dinner",
                Amount = 1800m,
                Date = new DateTime(2026, 7, 15),
                IsExpense = true,
                UserId = user.Id,
                CategoryId = food.Id
            }
        };

            context.Transactions.AddRange(transactions);

            var budgets = new List<Budget>
        {
            new()
            {
                LimitAmount = 12000m,
                Month = 7,
                Year = 2026,
                UserId = user.Id,
                CategoryId = food.Id
            },

            new()
            {
                LimitAmount = 5000m,
                Month = 7,
                Year = 2026,
                UserId = user.Id,
                CategoryId = transport.Id
            },

            new()
            {
                LimitAmount = 6000m,
                Month = 7,
                Year = 2026,
                UserId = user.Id,
                CategoryId = utilities.Id
            },

            new()
            {
                LimitAmount = 2000m,
                Month = 7,
                Year = 2026,
                UserId = user.Id,
                CategoryId = entertainment.Id
            },

            new()
            {
                LimitAmount = 5000m,
                Month = 7,
                Year = 2026,
                UserId = user.Id,
                CategoryId = shopping.Id
            },

            new()
            {
                LimitAmount = 3000m,
                Month = 7,
                Year = 2026,
                UserId = user.Id,
                CategoryId = health.Id
            }
        };

            context.Budgets.AddRange(budgets);

            await context.SaveChangesAsync();
        }

        public static async Task ResetAndSeedAsync(AppDbContext context)
        {
            await context.Database.EnsureDeletedAsync();

            await context.Database.MigrateAsync();

            await SeedAsync(context);
        }
    }
}
