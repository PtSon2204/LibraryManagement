using System.Net.Http.Headers;
using System.Text.Json;
using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Dashboard;

namespace LibraryManagement.MVC.Services;

public class StaffDashboardService : IStaffDashboardService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public StaffDashboardService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<StaffDashboardViewModel?> GetDashboardAsync()
    {
        AddJwt();

        var response = await _httpClient.GetAsync("api/staff-dashboard");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<StaffDashboardViewModel>(json, _jsonOptions);
    }

    private void AddJwt()
    {
        var token = _httpContextAccessor.HttpContext!.Session.GetString("AccessToken");
        if (string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            return;
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
