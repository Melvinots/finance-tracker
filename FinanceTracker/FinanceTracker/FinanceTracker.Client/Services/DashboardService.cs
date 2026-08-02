namespace FinanceTracker.Client.Services
{
    public class DashboardService
    {
        private readonly HttpClient _http;

        public DashboardService(HttpClient http)
        {
            _http = http;
        }

        public async Task<DashboardDto?> GetDashboardAsync(int month, int year)
        {
            return await _http.GetFromJsonAsync<DashboardDto>($"api/Dashboard/GetDashboard?month={month}&year={year}");
        }
    }
}
