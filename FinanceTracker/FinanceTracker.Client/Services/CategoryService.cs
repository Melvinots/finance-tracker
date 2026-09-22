using FinanceTracker.Shared.DTOs.Categories;
using static System.Net.WebRequestMethods;

namespace FinanceTracker.Client.Services
{
    public class CategoryService
    {
        private readonly HttpClient _http;

        public CategoryService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<CategoryDto>>("api/Categories/GetAll") 
                ?? new List<CategoryDto>();
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<CategoryDto>($"api/Categories/GetById/{id}")
                ?? new CategoryDto();
        }

        public async Task<int> GetCountByCategoryAsync(int categoryId)
        {
            return await _http.GetFromJsonAsync<int>($"api/Categories/GetTransactionCount/{categoryId}");
        }

        public async Task<HttpResponseMessage> CreateAsync(SaveCategoryDto dto)
        {
            return await _http.PostAsJsonAsync("api/Categories/Create", dto);
        }

        public async Task<HttpResponseMessage> UpdateAsync(int id, SaveCategoryDto dto)
        {
            return await _http.PutAsJsonAsync($"api/Categories/Update/{id}", dto);
        }

        public async Task<HttpResponseMessage> DeleteAsync(int id)
        {
            return await _http.DeleteAsync($"api/Categories/Delete/{id}");
        }
    }
}
