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

        public async Task<string?> CreatePublisherAsync(PublisherViewModel model)
        {
            var payload = new
            {
                model.PublisherName,
                model.Address,
                model.Phone,
                model.Email
            };

            var content = new System.Net.Http.StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/publishers", content);
            if (response.IsSuccessStatusCode) return null;

            return await ReadErrorMessage(response, "Thêm nhà xuất bản thất bại. Vui lòng thử lại.");
        }

        public async Task<string?> UpdatePublisherAsync(PublisherViewModel model)
        {
            var payload = new
            {
                model.PublisherId,
                model.PublisherName,
                model.Address,
                model.Phone,
                model.Email
            };

            var content = new System.Net.Http.StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/publishers/{model.PublisherId}", content);
            if (response.IsSuccessStatusCode) return null;

            return await ReadErrorMessage(response, "Cập nhật nhà xuất bản thất bại. Vui lòng thử lại.");
        }

        public async Task<bool> DeletePublisherAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/publishers/{id}");
            return response.IsSuccessStatusCode;
        }

        private static async Task<string> ReadErrorMessage(HttpResponseMessage response, string fallback)
        {
            try
            {
                var body = await response.Content.ReadAsStringAsync();
                var err = JsonSerializer.Deserialize<PublisherErrorResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return err?.Message ?? fallback;
            }
            catch
            {
                return fallback;
            }
        }
    }

    internal class PublisherErrorResponse
    {
        public string? Message { get; set; }
    }
}
