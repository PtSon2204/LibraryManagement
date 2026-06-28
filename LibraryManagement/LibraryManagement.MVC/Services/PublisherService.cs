using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Publisher;
using System.Net.Http.Headers;
using System.Text.Json;

namespace LibraryManagement.MVC.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public PublisherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }



        public async Task<PublisherListViewModel?> GetPublishersAsync(string? search, int pageNumber, int pageSize)
        {

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

            var response = await _httpClient.GetAsync($"api/publishers/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PublisherViewModel>(json, _jsonOptions);
        }
    }
}
