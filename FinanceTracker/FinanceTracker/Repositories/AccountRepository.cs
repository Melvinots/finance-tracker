using FinanceTracker.Helpers;

namespace FinanceTracker.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;

        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> ExportDataAsync(int userId)
        {
            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();

            return CsvExporter.BuildTransactionsCsv(transactions);
        }

        public async Task DeactivateAccountAsync(int userId)
        {
            var user = await _context.Users.FirstAsync(u => u.Id == userId);
            user.IsActive = false;

            await _context.SaveChangesAsync();
        }
    }
}
