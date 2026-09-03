
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

            // One category per icon supported by Icons.Category, so every icon
            // is exercised somewhere in the seeded data.
            var categoryDefs = new (string Name, string Icon, string Color)[]
            {
                ("Food",          "Food",          "#10B981"),
                ("Transport",     "Transport",     "#8B5CF6"),
                ("Utilities",     "Utilities",     "#F59E0B"),
                ("Entertainment", "Entertainment", "#F43F5E"),
                ("Shopping",      "Shopping",      "#3B82F6"),
                ("Health",        "Health",        "#14B8A6"),
                ("Home",          "Home",          "#F97316"),
                ("Clothing",      "Clothing",      "#EC4899"),
                ("Education",     "Education",     "#6366F1"),
                ("Travel",        "Travel",        "#06B6D4"),
                ("Work",          "Work",          "#0EA5E9"), // salary / freelance income
                ("Gaming",        "Gaming",        "#A855F7"),
                ("Medicine",      "Medicine",      "#EF4444"),
                ("Pets",          "Pets",          "#84CC16"),
                ("Gifts",         "Gifts",         "#D946EF"),
                ("Other",         "Other",         "#94A3B8"),
            };

            var categories = categoryDefs
                .Select(c => new Category
                {
                    Name = c.Name,
                    Icon = c.Icon,
                    Color = c.Color,
                    UserId = user.Id
                })
                .ToList();

            context.Categories.AddRange(categories);

            await context.SaveChangesAsync();

            var cat = categories.ToDictionary(c => c.Name, c => c);

            var transactions = new List<Transaction>();
            var budgets = new List<Budget>();

            void AddExpense(string categoryName, int month, int day, decimal amount, string description, string? notes = null)
            {
                transactions.Add(new Transaction
                {
                    Description = description,
                    Amount = amount,
                    Date = new DateTime(2026, month, day),
                    IsExpense = true,
                    Notes = notes,
                    UserId = user.Id,
                    CategoryId = cat[categoryName].Id
                });
            }

            void AddIncome(int month, int day, decimal amount, string description, string? notes = null)
            {
                transactions.Add(new Transaction
                {
                    Description = description,
                    Amount = amount,
                    Date = new DateTime(2026, month, day),
                    IsExpense = false,
                    Notes = notes,
                    UserId = user.Id,
                    CategoryId = cat["Work"].Id
                });
            }

            void AddBudget(string categoryName, int month, decimal limit)
            {
                budgets.Add(new Budget
                {
                    LimitAmount = limit,
                    Month = month,
                    Year = 2026,
                    UserId = user.Id,
                    CategoryId = cat[categoryName].Id
                });
            }

            // ---- Recurring monthly activity (Jan - Sep 2026) --------------------
            for (var month = 1; month <= 9; month++)
            {
                var growth = month - 1;

                // Income
                AddIncome(month, 1, 48000m + growth * 300m, "Monthly salary");

                // Food
                AddExpense("Food", month, 3, 3800m + growth * 50m, "Grocery shopping - week 1");
                AddExpense("Food", month, 10, 1500m + growth * 20m, "Grocery top-up");
                AddExpense("Food", month, 17, 2100m, "Restaurant dinner");
                AddExpense("Food", month, 24, 1200m, "Weekend brunch");

                // Transport
                AddExpense("Transport", month, 5, 1800m, "Grab rides");
                AddExpense("Transport", month, 20, 900m, "Gas refill");

                // Utilities
                AddExpense("Utilities", month, 5, 2400m + growth * 15m, "Electric bill");
                AddExpense("Utilities", month, 6, 800m, "Water bill");
                AddExpense("Utilities", month, 7, 1200m, "Internet & phone");

                // Entertainment
                AddExpense("Entertainment", month, 8, 599m, "Netflix subscription");
                AddExpense("Entertainment", month, 15, 1200m, "Movie night out");

                // Shopping (baseline; August gets an extra spree below)
                AddExpense("Shopping", month, 12, 2500m, "Online shopping");

                // Health
                AddExpense("Health", month, 14, 850m, "Pharmacy purchase");

                // Core budgets every month
                AddBudget("Food", month, 10000m);
                AddBudget("Transport", month, 3000m);
                AddBudget("Utilities", month, 5000m);
                AddBudget("Entertainment", month, 2000m);
                AddBudget("Health", month, 1500m);

                // Shopping budget: baseline every month except August, which is
                // overridden below to intentionally show an over-budget month.
                if (month != 8)
                {
                    AddBudget("Shopping", month, 5000m);
                }
            }

            // ---- One-off / seasonal activity to exercise every category ---------

            // January - new year course + gift exchange (Education goes over budget)
            AddExpense("Education", 1, 20, 5000m, "New Year online course enrollment");
            AddExpense("Gifts", 1, 2, 1500m, "New Year gift exchange");
            AddBudget("Education", 1, 4000m); // over budget
            AddBudget("Gifts", 1, 2000m);

            // February - Valentine's spending (Gifts goes slightly over budget)
            AddExpense("Gifts", 2, 13, 2200m, "Valentine's Day gift");
            AddExpense("Clothing", 2, 13, 1800m, "Valentine's outfit");
            AddBudget("Gifts", 2, 2000m); // over budget
            AddBudget("Clothing", 2, 2500m);

            // March - doctor visit + aircon repair (Medicine goes over budget)
            AddExpense("Medicine", 3, 18, 3200m, "Doctor consultation + prescription meds");
            AddExpense("Home", 3, 22, 4500m, "Aircon repair service");
            AddBudget("Medicine", 3, 2000m); // over budget
            AddBudget("Home", 3, 5000m);

            // April - weekend getaway + vet visit
            AddExpense("Travel", 4, 25, 6500m, "Weekend trip to Baguio");
            AddExpense("Pets", 4, 10, 1200m, "Vet checkup and vaccines");
            AddBudget("Travel", 4, 7000m);
            AddBudget("Pets", 4, 1500m);

            // May - new game + new furniture (both slightly over budget)
            AddExpense("Gaming", 5, 16, 2999m, "New game purchase");
            AddExpense("Home", 5, 9, 5200m, "New furniture");
            AddBudget("Gaming", 5, 2500m);  // over budget
            AddBudget("Home", 5, 5000m);    // over budget

            // June - summer vacation + a freelance project (Travel goes well over budget)
            AddExpense("Travel", 6, 5, 9500m, "Summer vacation - flights");
            AddExpense("Travel", 6, 7, 8500m, "Summer vacation - hotel");
            AddIncome(6, 20, 12000m, "Freelance web development project");
            AddBudget("Travel", 6, 15000m); // over budget

            // July - wardrobe refresh + misc + freelance payment (baseline month, Clothing over)
            AddExpense("Clothing", 7, 19, 3500m, "New wardrobe essentials");
            AddExpense("Other", 7, 27, 700m, "Miscellaneous expenses");
            AddIncome(7, 12, 8000m, "Freelance payment");
            AddBudget("Clothing", 7, 3000m); // over budget
            AddBudget("Other", 7, 1000m);

            // August - back-to-school shopping spree (Shopping goes well over budget)
            AddExpense("Shopping", 8, 6, 6000m, "Back-to-school shopping spree");
            AddExpense("Education", 8, 7, 2500m, "School supplies and books");
            AddExpense("Pets", 8, 21, 950m, "Pet food and grooming");
            AddBudget("Shopping", 8, 5000m); // over budget (baseline 2500 + 6000 spree)
            AddBudget("Education", 8, 3000m);
            AddBudget("Pets", 8, 1500m);

            // September - big-ticket console purchase + birthday gift + freelance payment
            AddExpense("Gaming", 9, 11, 15000m, "Gaming console purchase");
            AddExpense("Gifts", 9, 24, 1800m, "Birthday gift for a friend");
            AddExpense("Other", 9, 28, 500m, "Charity donation");
            AddIncome(9, 15, 9000m, "Freelance payment - September project");
            AddBudget("Gaming", 9, 5000m); // way over budget
            AddBudget("Gifts", 9, 2000m);
            AddBudget("Other", 9, 1000m);

            context.Transactions.AddRange(transactions);
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
