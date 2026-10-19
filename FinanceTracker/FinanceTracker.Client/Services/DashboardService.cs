namespace FinanceTracker.Client.Services
{
    public class DashboardService
    {
        private readonly HttpClient _http;

        public DashboardService(HttpClient http)
        {
            _http = http;
        }

        public async Task<DashboardDto> GetDashboardAsync(int month, int year)
        {
            var response = await _http.GetAsync(ApiRoutes.DashboardByMonth(month, year));
            return await response.ReadOrThrowAsync<DashboardDto>();
        }
    }
}
