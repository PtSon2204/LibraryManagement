using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Books;

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
            var response = await _httpClient.GetAsync("api/Category");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<CategoryOption>>();
            }
            return new List<CategoryOption>();
        }
    }
}
