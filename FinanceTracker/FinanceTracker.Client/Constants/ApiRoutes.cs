namespace FinanceTracker.Client.Constants
{
    public static class ApiRoutes
    {
        public const string Auth = "api/auth";
        public const string AuthRegister = $"{Auth}/register";
        public const string AuthLogin = $"{Auth}/login";
        public const string AuthRefresh = $"{Auth}/refresh";
        public const string AuthForgotPassword = $"{Auth}/forgot-password";
        public const string AuthRevoke = $"{Auth}/revoke";

        public const string Budgets = "api/budgets";
        public const string Categories = "api/categories";
        public const string Transactions = "api/transactions";
        public const string Dashboard = "api/dashboard";
        public const string UserSettings = "api/user-settings";
        public const string Account = "api/account";

        // ── Route builders ───────────────────────────────────────

        public static string BudgetById(int id) => $"{Budgets}/{id}";
        public static string BudgetsByMonth(int month, int year) => WithMonthYear(Budgets, month, year);

        public static string CategoryById(int id) => $"{Categories}/{id}";
        public static string CategoryTransactionCount(int id) => $"{Categories}/{id}/transaction-count";

        public static string TransactionById(int id) => $"{Transactions}/{id}";

        public static string DashboardByMonth(int month, int year) => WithMonthYear(Dashboard, month, year);

        public static string AccountExport => $"{Account}/export";

        // ── Private helpers ──────────────────────────────────────

        private static string WithMonthYear(string path, int month, int year) =>
            $"{path}?month={month}&year={year}";
    }
}