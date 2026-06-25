using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Publisher;
using System.Net.Http.Headers;
using System.Text.Json;

namespace LibraryManagement.MVC.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public PublisherService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddJwt()
        {
            var token = _httpContextAccessor.HttpContext!.Session.GetString("AccessToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<PublisherListViewModel?> GetPublishersAsync(string? search, int pageNumber, int pageSize)
        {
            AddJwt();

            var url = $"api/publishers?pageNumber={pageNumber}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(search))
                url += $"&search={Uri.EscapeDataString(search)}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PublisherListViewModel>(json, _jsonOptions);
            if (result != null) result.Search = search;

            return result;
        }

        public async Task<PublisherViewModel?> GetPublisherByIdAsync(int id)
        {
            AddJwt();

            var response = await _httpClient.GetAsync($"api/publishers/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PublisherViewModel>(json, _jsonOptions);
        }
    }
}
