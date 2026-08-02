using FinanceTracker.Shared.DTOs.Transactions;

namespace FinanceTracker.Client.Services
{
    public class TransactionService
    {
        private readonly HttpClient _http;

        public TransactionService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<TransactionDto>>GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<TransactionDto>>("api/Transactions/GetAll")
                ?? new List<TransactionDto>();
        }

        public async Task<TransactionDto?>GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<TransactionDto>($"api/Transactions/GetById/{id}")
                ?? new TransactionDto();
        }

        public async Task<HttpResponseMessage>CreateAsync(SaveTransactionDto dto)
        {
            return await _http.PostAsJsonAsync("api/Transactions/Create", dto);
        }

        public async Task<HttpResponseMessage>UpdateAsync(int id, SaveTransactionDto dto)
        {
            return await _http.PutAsJsonAsync($"api/Transactions/Update/{id}", dto);
        }

        public async Task<HttpResponseMessage>DeleteAsync(int id)
        {
            return await _http.DeleteAsync($"api/Transactions/Delete/{id}");
        }
    }
}
