using LibraryManagement.MVC.Interface;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Services
{
    public class SlotTemplateService : ISlotTemplateService
    {
        private readonly HttpClient _httpClient;

        public SlotTemplateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetAllActiveTemplatesAsync()
        {
            var response = await _httpClient.GetAsync("/api/SlotTemplates/active");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetAllTemplatesAsync()
        {
            var response = await _httpClient.GetAsync("/api/SlotTemplates");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> CreateTemplateAsync(object payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/SlotTemplates", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ToggleTemplateStatusAsync(int id)
        {
            var content = new StringContent("", Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/SlotTemplates/{id}/toggle", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTemplateAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/SlotTemplates/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
