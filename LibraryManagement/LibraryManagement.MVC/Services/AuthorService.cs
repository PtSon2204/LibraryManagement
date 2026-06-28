using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Books;

namespace LibraryManagement.MVC.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly HttpClient _httpClient;

        public AuthorService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<AuthorOption>?> GetAllAuthorsAsync()
        {
            var response = await _httpClient.GetAsync("api/Author");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<AuthorOption>>();
            }
            return new List<AuthorOption>();
        }
    }
}
