using System.Net.Http.Headers;
using System.Text.Json;
using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Fines;

namespace LibraryManagement.MVC.Services;

public class FineService : IFineService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public FineService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<FineTemplateViewModel>> GetActiveTemplatesAsync()
    {
        AddJwt();
        var response = await _httpClient.GetAsync("api/fines/templates");
        if (!response.IsSuccessStatusCode) return new List<FineTemplateViewModel>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<FineTemplateViewModel>>(json, _jsonOptions) ?? new();
    }

    public async Task<List<FineTemplateViewModel>> GetAllTemplatesAsync()
    {
        AddJwt();
        var response = await _httpClient.GetAsync("api/fines/templates/all");
        if (!response.IsSuccessStatusCode) return new List<FineTemplateViewModel>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<FineTemplateViewModel>>(json, _jsonOptions) ?? new();
    }

    public async Task<string?> CreateFineAsync(CreateFineViewModel model)
    {
        AddJwt();
        var response = await _httpClient.PostAsJsonAsync("api/fines", model);
        if (response.IsSuccessStatusCode) return null;

        var error = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(error) ? "Không thể tạo khoản phạt. Vui lòng thử lại." : error;
    }

    public async Task<FineListPageViewModel?> GetFinesAsync(string? search, string? status, int page, int pageSize)
    {
        AddJwt();
        var query = new List<string>
        {
            $"page={Math.Max(page, 1)}",
            $"pageSize={(pageSize <= 0 ? 15 : pageSize)}"
        };

        if (!string.IsNullOrWhiteSpace(search)) query.Add($"search={Uri.EscapeDataString(search)}");
        if (!string.IsNullOrWhiteSpace(status)) query.Add($"status={Uri.EscapeDataString(status)}");

        var response = await _httpClient.GetAsync($"api/fines?{string.Join('&', query)}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<FineListPageViewModel>(json, _jsonOptions);
    }

    public async Task<string?> CreateTemplateAsync(UpsertFineTemplateViewModel model)
    {
        AddJwt();
        var response = await _httpClient.PostAsJsonAsync("api/fines/templates", model);
        if (response.IsSuccessStatusCode) return null;

        var error = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(error) ? "Lỗi khi tạo loại khoản phạt." : error;
    }

    public async Task<string?> UpdateTemplateAsync(Guid id, UpsertFineTemplateViewModel model)
    {
        AddJwt();
        var response = await _httpClient.PutAsJsonAsync($"api/fines/templates/{id}", model);
        if (response.IsSuccessStatusCode) return null;

        var error = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(error) ? "Lỗi khi cập nhật loại khoản phạt." : error;
    }

    public async Task<string?> DeleteTemplateAsync(Guid id)
    {
        AddJwt();
        var response = await _httpClient.DeleteAsync($"api/fines/templates/{id}");
        if (response.IsSuccessStatusCode) return null;

        var error = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(error) ? "Lỗi khi xóa loại khoản phạt." : error;
    }

    public async Task<PaymentQrViewModel?> GenerateQrAsync(decimal amount, Guid loanDetailId)
    {
        AddJwt();
        var response = await _httpClient.GetAsync($"api/fines/generate-qr?amount={amount}&loanDetailId={loanDetailId}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PaymentQrViewModel>(json, _jsonOptions);
    }

    private void AddJwt()
    {
        var token = _httpContextAccessor.HttpContext!.Session.GetString("AccessToken");
        _httpClient.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token)
            ? null
            : new AuthenticationHeaderValue("Bearer", token);
    }
}
