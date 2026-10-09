
using FinanceTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data
{
    public static class DbSeeder
    {
        private const int Year = 2026;
        private const int LastMonth = 12;

        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Users.AnyAsync())
            {
                return;
            }

            var user = new User
            {
                FullName = "Test User",
                Email = "test@example.com",
                PasswordHash = "$2a$11$Y3xTvcE6xInG/x5zEyvfBOkYE7Z.Z4GIU1fZQ6qgqjV2vuYFFcjSm",
                CreatedAt = DateTime.UtcNow
            };

            var settings = new UserSettings
            {
                User = user,
                Currency = "USD",
                Appearance = "light"
            };

            context.Users.Add(user);
            context.UserSettings.Add(settings);

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
            ("Work",          "Work",          "#0EA5E9"),
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

            var cat = categories.ToDictionary(c => c.Name);

            var transactions = new List<Transaction>();

            // Keyed by (category, month) so a one-off budget replaces the baseline
            // instead of creating a duplicate row for the same category and month.
            var budgetMap = new Dictionary<(string Category, int Month), decimal>();

            void AddExpense(string categoryName, int month, int day, decimal amount, string description, string? notes = null)
            {
                transactions.Add(new Transaction
                {
                    Description = description,
                    Amount = amount,
                    Date = new DateTime(Year, month, day),
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
                    Date = new DateTime(Year, month, day),
                    IsExpense = false,
                    Notes = notes,
                    UserId = user.Id,
                    CategoryId = cat["Work"].Id
                });
            }

            void SetBudget(string categoryName, int month, decimal limit)
                => budgetMap[(categoryName, month)] = limit;

            // ---- Recurring monthly activity (January - LastMonth) ---------------

            var baselineBudgets = new (string Category, decimal Limit)[]
            {
            ("Food",          10000m),
            ("Transport",      3000m),
            ("Utilities",      5000m),
            ("Entertainment",  2000m),
            ("Health",         1500m),
            ("Shopping",       5000m),
            };

            for (var month = 1; month <= LastMonth; month++)
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

                // Shopping (baseline; August and November get extra sprees below)
                AddExpense("Shopping", month, 12, 2500m, "Online shopping");

                // Health
                AddExpense("Health", month, 14, 850m, "Pharmacy purchase");

                foreach (var (category, limit) in baselineBudgets)
                {
                    SetBudget(category, month, limit);
                }
            }

            // ---- One-off / seasonal activity to exercise every category ---------

            // January - new year course + gift exchange (Education goes over budget)
            AddExpense("Education", 1, 20, 5000m, "New Year online course enrollment");
            AddExpense("Gifts", 1, 2, 1500m, "New Year gift exchange");
            SetBudget("Education", 1, 4000m); // over budget
            SetBudget("Gifts", 1, 2000m);

            // February - Valentine's spending (Gifts goes slightly over budget)
            AddExpense("Gifts", 2, 13, 2200m, "Valentine's Day gift");
            AddExpense("Clothing", 2, 13, 1800m, "Valentine's outfit");
            SetBudget("Gifts", 2, 2000m); // over budget
            SetBudget("Clothing", 2, 2500m);

            // March - doctor visit + aircon repair (Medicine goes over budget)
            AddExpense("Medicine", 3, 18, 3200m, "Doctor consultation + prescription meds");
            AddExpense("Home", 3, 22, 4500m, "Aircon repair service");
            SetBudget("Medicine", 3, 2000m); // over budget
            SetBudget("Home", 3, 5000m);

            // April - weekend getaway + vet visit
            AddExpense("Travel", 4, 25, 6500m, "Weekend trip to Baguio");
            AddExpense("Pets", 4, 10, 1200m, "Vet checkup and vaccines");
            SetBudget("Travel", 4, 7000m);
            SetBudget("Pets", 4, 1500m);

            // May - new game + new furniture (both slightly over budget)
            AddExpense("Gaming", 5, 16, 2999m, "New game purchase");
            AddExpense("Home", 5, 9, 5200m, "New furniture");
            SetBudget("Gaming", 5, 2500m);  // over budget
            SetBudget("Home", 5, 5000m);    // over budget

            // June - summer vacation + a freelance project (Travel goes well over budget)
            AddExpense("Travel", 6, 5, 9500m, "Summer vacation - flights");
            AddExpense("Travel", 6, 7, 8500m, "Summer vacation - hotel");
            AddIncome(6, 20, 12000m, "Freelance web development project");
            SetBudget("Travel", 6, 15000m); // over budget

            // July - wardrobe refresh + misc + freelance payment (Clothing over budget)
            AddExpense("Clothing", 7, 19, 3500m, "New wardrobe essentials");
            AddExpense("Other", 7, 27, 700m, "Miscellaneous expenses");
            AddIncome(7, 12, 8000m, "Freelance payment");
            SetBudget("Clothing", 7, 3000m); // over budget
            SetBudget("Other", 7, 1000m);

            // August - back-to-school shopping spree (Shopping goes well over budget)
            AddExpense("Shopping", 8, 6, 6000m, "Back-to-school shopping spree");
            AddExpense("Education", 8, 7, 2500m, "School supplies and books");
            AddExpense("Pets", 8, 21, 950m, "Pet food and grooming");
            SetBudget("Education", 8, 3000m);
            SetBudget("Pets", 8, 1500m);

            // September - big-ticket console purchase + birthday gift + freelance payment
            AddExpense("Gaming", 9, 11, 15000m, "Gaming console purchase");
            AddExpense("Gifts", 9, 24, 1800m, "Birthday gift for a friend");
            AddExpense("Other", 9, 28, 500m, "Charity donation");
            AddIncome(9, 15, 9000m, "Freelance payment - September project");
            SetBudget("Gaming", 9, 5000m); // way over budget
            SetBudget("Gifts", 9, 2000m);
            SetBudget("Other", 9, 1000m);

            // October - family birthday dinner + phone repair + water filter
            if (LastMonth >= 10)
            {
                AddExpense("Food", 10, 12, 3500m, "Birthday dinner for Mom");        // pushes Food over budget
                AddExpense("Other", 10, 16, 3200m, "Phone screen repair");           // over budget
                AddExpense("Home", 10, 19, 1800m, "Replacement water filter");
                SetBudget("Other", 10, 2000m);
                SetBudget("Home", 10, 2500m);
            }

            // November - Undas trip + 11.11 sale haul + flu + game sale + freelance payment
            if (LastMonth >= 11)
            {
                AddExpense("Travel", 11, 1, 4200m, "Undas trip to the province");
                AddExpense("Transport", 11, 1, 1600m, "Bus tickets for the province trip"); // pushes Transport over budget
                AddExpense("Shopping", 11, 11, 7800m, "11.11 sale haul");                   // Shopping well over budget
                AddExpense("Medicine", 11, 14, 1100m, "Flu medicine");
                AddExpense("Gaming", 11, 25, 1500m, "Seasonal game sale");
                AddIncome(11, 18, 7000m, "Freelance payment - November project");
                SetBudget("Travel", 11, 5000m);
                SetBudget("Medicine", 11, 1500m);
                SetBudget("Gaming", 11, 2000m);
            }

            // December - Christmas: 13th month pay, gifts, Noche Buena, holiday trip
            if (LastMonth >= 12)
            {
                AddExpense("Education", 12, 3, 3000m, "Online course renewal");
                AddExpense("Clothing", 12, 10, 2400m, "Christmas outfit");
                AddExpense("Entertainment", 12, 12, 1500m, "Christmas party contribution");
                AddIncome(12, 15, 51000m, "13th month pay");
                AddExpense("Gifts", 12, 18, 4500m, "Christmas gifts for family");
                AddExpense("Gifts", 12, 19, 900m, "Secret Santa");                          // Gifts over budget
                AddExpense("Other", 12, 20, 1000m, "Year-end donation");
                AddExpense("Food", 12, 23, 6500m, "Noche Buena groceries");                 // Food over budget
                AddExpense("Travel", 12, 26, 8200m, "Holiday trip - flights");
                AddExpense("Travel", 12, 27, 7000m, "Holiday trip - hotel");
                SetBudget("Education", 12, 3500m);
                SetBudget("Clothing", 12, 2500m);
                SetBudget("Entertainment", 12, 3500m); // raised for the holidays
                SetBudget("Gifts", 12, 4000m);         // over budget
                SetBudget("Other", 12, 1000m);
                SetBudget("Travel", 12, 16000m);
            }

            var budgets = budgetMap
                .Select(b => new Budget
                {
                    LimitAmount = b.Value,
                    Month = b.Key.Month,
                    Year = Year,
                    UserId = user.Id,
                    CategoryId = cat[b.Key.Category].Id
                })
                .ToList();

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
