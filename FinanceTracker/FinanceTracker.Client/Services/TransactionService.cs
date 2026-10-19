namespace FinanceTracker.Client.Services
{
    public class TransactionService
    {
        private readonly HttpClient _http;

        public TransactionService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<TransactionDto>> GetAllAsync()
        {
            var response = await _http.GetAsync(ApiRoutes.Transactions);
            return await response.ReadOrThrowAsync<List<TransactionDto>>();
        }

        public async Task<TransactionDto?> GetByIdAsync(int id)
        {
            var response = await _http.GetAsync(ApiRoutes.TransactionById(id));
            return await response.ReadOrDefaultAsync<TransactionDto>();
        }

        public async Task<TransactionDto> CreateAsync(SaveTransactionDto dto)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.Transactions, dto);
            return await response.ReadOrThrowAsync<TransactionDto>();
        }

        public async Task<TransactionDto> UpdateAsync(int id, SaveTransactionDto dto)
        {
            var response = await _http.PutAsJsonAsync(ApiRoutes.TransactionById(id), dto);
            return await response.ReadOrThrowAsync<TransactionDto>();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync(ApiRoutes.TransactionById(id));
            await response.EnsureSuccessOrThrowAsync();
        }
    }
}
