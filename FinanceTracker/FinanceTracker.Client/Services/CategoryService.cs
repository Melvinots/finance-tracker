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
            var response = await _http.GetAsync(ApiRoutes.Categories);
            return await response.ReadOrThrowAsync<List<CategoryDto>>();
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var response = await _http.GetAsync(ApiRoutes.CategoryById(id));
            return await response.ReadOrDefaultAsync<CategoryDto>();
        }

        public async Task<int> GetCountByCategoryAsync(int categoryId)
        {
            var response = await _http.GetAsync(ApiRoutes.CategoryTransactionCount(categoryId));
            return await response.ReadOrThrowAsync<int>();
        }

        public async Task<CategoryDto> CreateAsync(SaveCategoryDto dto)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.Categories, dto);
            return await response.ReadOrThrowAsync<CategoryDto>();
        }

        public async Task<CategoryDto> UpdateAsync(int id, SaveCategoryDto dto)
        {
            var response = await _http.PutAsJsonAsync(ApiRoutes.CategoryById(id), dto);
            return await response.ReadOrThrowAsync<CategoryDto>();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync(ApiRoutes.CategoryById(id));
            await response.EnsureSuccessOrThrowAsync();
        }
    }
}
