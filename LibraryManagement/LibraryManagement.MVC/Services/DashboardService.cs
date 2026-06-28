using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Dashboard;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public DashboardService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DashboardViewModel?> GetDashboardStatsAsync()
        {
            var response = await _httpClient.GetAsync("api/dashboard");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DashboardViewModel>(json, _jsonOptions);
        }
    }
}
