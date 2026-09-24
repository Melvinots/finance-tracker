using FinanceTracker.Data;
using FinanceTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Repositories
{
    public class UserSettingsRepository : IUserSettingsRepository
    {
        private readonly AppDbContext _context;

        public UserSettingsRepository(AppDbContext context) 
        {
            _context = context;
        }

        public async Task<UserSettings> GetAsync(int userId)
        {
            return await _context.UserSettings
                .Include(us => us.User)
                .FirstAsync(us => us.UserId == userId);
        }

        public async Task<UserSettings> GetByIdAsync(int id, int userId)
        {
            return await _context.UserSettings
                .Include(us => us.User)
                .FirstAsync(us => us.Id == id && us.UserId == userId);
        }

        public async Task<UserSettings> UpdateAsync(UserSettings settings)
        {
            _context.UserSettings.Update(settings);
            await _context.SaveChangesAsync();

            return settings;
        }

        public async Task<string> ExportDataAsync()
        {
            throw new NotImplementedException();
        }

        public async Task DeactivateAccountAsync()
        {
            throw new NotImplementedException();
        }
    }
}
