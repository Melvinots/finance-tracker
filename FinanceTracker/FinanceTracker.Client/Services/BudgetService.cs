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
            var response = await _http.GetAsync(ApiRoutes.BudgetsByMonth(month, year));
            return await response.ReadOrThrowAsync<List<BudgetDto>>();
        }

        public async Task<BudgetDto?> GetByIdAsync(int id)
        {
            var response = await _http.GetAsync(ApiRoutes.BudgetById(id));
            return await response.ReadOrDefaultAsync<BudgetDto>();
        }

        public async Task<BudgetDto> CreateAsync(SaveBudgetDto dto)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.Budgets, dto);
            return await response.ReadOrThrowAsync<BudgetDto>();
        }

        public async Task<BudgetDto> UpdateAsync(int id, SaveBudgetDto dto)
        {
            var response = await _http.PutAsJsonAsync(ApiRoutes.BudgetById(id), dto);
            return await response.ReadOrThrowAsync<BudgetDto>();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync(ApiRoutes.BudgetById(id));
            await response.EnsureSuccessOrThrowAsync();
        }
    }
}
