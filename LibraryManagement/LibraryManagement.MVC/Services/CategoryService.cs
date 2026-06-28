using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Books;
using LibraryManagement.MVC.ViewModels.Category;

namespace LibraryManagement.MVC.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;

        public CategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CategoryOption>?> GetAllCategoriesAsync()
        {
            var response = await _httpClient.GetAsync("api/categories/all");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<CategoryOption>>();
            }
            return new List<CategoryOption>();
        }

        public async Task<CategoryListViewModel?> GetCategoriesAsync(string? search, int pageNumber, int pageSize)
        {
            try
            {
                var query = $"api/categories?search={Uri.EscapeDataString(search ?? "")}&pageNumber={pageNumber}&pageSize={pageSize}";
                return await _httpClient.GetFromJsonAsync<CategoryListViewModel>(query);
            }
            catch
            {
                return null;
            }
        }

        public async Task<CategoryViewModel?> GetCategoryByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<CategoryViewModel>($"api/categories/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<string?> CreateCategoryAsync(CategoryViewModel model)
        {
            var payload = new
            {
                model.CategoryName
            };

            var content = new System.Net.Http.StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/categories", content);
            if (response.IsSuccessStatusCode) return null;

            return await ReadErrorMessage(response, "Thêm thể loại thất bại. Vui lòng thử lại.");
        }

        public async Task<string?> UpdateCategoryAsync(CategoryViewModel model)
        {
            var payload = new
            {
                model.CategoryId,
                model.CategoryName
            };

            var content = new System.Net.Http.StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/categories/{model.CategoryId}", content);
            if (response.IsSuccessStatusCode) return null;

            return await ReadErrorMessage(response, "Cập nhật thể loại thất bại. Vui lòng thử lại.");
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/categories/{id}");
            return response.IsSuccessStatusCode;
        }

        private static async Task<string> ReadErrorMessage(HttpResponseMessage response, string fallback)
        {
            try
            {
                var body = await response.Content.ReadAsStringAsync();
                var err = JsonSerializer.Deserialize<CategoryErrorResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return err?.Message ?? fallback;
            }
            catch
            {
                return fallback;
            }
        }
    }

    internal class CategoryErrorResponse
    {
        public string? Message { get; set; }
    }
}
