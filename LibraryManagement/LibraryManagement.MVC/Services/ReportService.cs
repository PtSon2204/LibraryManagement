using System.Net.Http.Headers;
using System.Text.Json;
using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Reports;

namespace LibraryManagement.MVC.Services;

public class ReportService : IReportService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ReportService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<LibraryReportViewModel?> GetLibraryReportAsync(ReportFilterViewModel filter)
    {
        AddJwt();

        var query = new List<string>();
        if (filter.FromDate.HasValue)
            query.Add($"fromDate={Uri.EscapeDataString(filter.FromDate.Value.ToString("yyyy-MM-dd"))}");
        if (filter.ToDate.HasValue)
            query.Add($"toDate={Uri.EscapeDataString(filter.ToDate.Value.ToString("yyyy-MM-dd"))}");

        var url = query.Count == 0 ? "api/reports/library" : $"api/reports/library?{string.Join('&', query)}";
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        var report = JsonSerializer.Deserialize<LibraryReportViewModel>(json, JsonOptions);
        if (report != null)
            report.Filter = filter;

        return report;
    }

    private void AddJwt()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var token = httpContext?.User.FindFirst("jwt_token")?.Value;
        if (string.IsNullOrWhiteSpace(token))
            token = httpContext?.Session.GetString("AccessToken");

        if (string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            return;
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
