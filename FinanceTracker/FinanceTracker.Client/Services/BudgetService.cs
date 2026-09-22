using FinanceTracker.Shared.DTOs.Budgets;

namespace FinanceTracker.Client.Services
{
    public class BudgetService
    {
        private readonly HttpClient _http;

        public BudgetService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<BudgetDto>> GetAllAsync(int month, int year)
        {
            return await _http.GetFromJsonAsync<List<BudgetDto>>($"api/Budgets/GetAll?month={month}&year={year}")
                ?? new List<BudgetDto>();
        }

        public async Task<BudgetDto?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<BudgetDto>($"api/Budgets/GetById/{id}");
        }

        public async Task<HttpResponseMessage> CreateAsync(SaveBudgetDto dto)
        {
            return await _http.PostAsJsonAsync("api/Budgets/Create", dto);
        }

        public async Task<HttpResponseMessage> UpdateAsync(int id, SaveBudgetDto dto)
        {
            return await _http.PutAsJsonAsync($"api/Budgets/Update/{id}", dto);
        }

        public async Task<HttpResponseMessage> DeleteAsync(int id)
        {
            return await _http.DeleteAsync($"api/Budgets/Delete/{id}");
        }
    }
}
