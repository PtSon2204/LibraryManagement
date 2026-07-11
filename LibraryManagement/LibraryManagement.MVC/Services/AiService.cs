using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Ai;

namespace LibraryManagement.MVC.Services
{
    public class AiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public AiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> ChatAsync(ChatRequestViewModel request)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(request, _jsonOptions),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("api/ai/chat", content);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("response", out var responseProperty))
            {
                return responseProperty.GetString();
            }

            return null;
        }
    }
}
