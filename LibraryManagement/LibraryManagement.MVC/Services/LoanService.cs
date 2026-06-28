using System.Net.Http.Headers;
using System.Text.Json;
using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Loans;

namespace LibraryManagement.MVC.Services;

public class LoanService : ILoanService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public LoanService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<LoanListPageViewModel?> GetStaffLoansAsync(LoanSearchViewModel search)
    {
        AddJwt();
        var query = BuildQuery(search, includeStaffFilters: true);
        var response = await _httpClient.GetAsync($"api/loans?{query}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoanListPageViewModel>(json, _jsonOptions);
        if (result != null) result.Search = search;
        return result;
    }

    public async Task<LoanListPageViewModel?> GetMyLoansAsync(LoanSearchViewModel search)
    {
        AddJwt();
        var query = BuildQuery(search, includeStaffFilters: false);
        var response = await _httpClient.GetAsync($"api/loans/my?{query}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoanListPageViewModel>(json, _jsonOptions);
        if (result != null) result.Search = search;
        return result;
    }

    public async Task<BorrowBookResultViewModel?> BorrowBookAsync(Guid bookId)
    {
        AddJwt();
        var response = await _httpClient.PostAsJsonAsync("api/loans/borrow", new { BookId = bookId });
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<BorrowBookResultViewModel>(_jsonOptions);
    }

    public async Task<string?> ReturnBookAsync(Guid loanDetailId)
    {
        AddJwt();
        var response = await _httpClient.PostAsync($"api/loans/{loanDetailId}/return", null);
        if (response.IsSuccessStatusCode) return null;

        var error = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(error) ? "Không thể trả sách. Vui lòng thử lại." : error;
    }

    private static string BuildQuery(LoanSearchViewModel search, bool includeStaffFilters)
    {
        var query = new List<string>
        {
            $"page={Math.Max(search.Page, 1)}",
            $"pageSize={(search.PageSize <= 0 ? 10 : search.PageSize)}"
        };

        if (includeStaffFilters)
        {
            AddQuery(query, "status", search.Status);
            AddQuery(query, "search", search.Search);
        }

        return string.Join('&', query);
    }

    private static void AddQuery(List<string> query, string name, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            query.Add($"{name}={Uri.EscapeDataString(value)}");
    }

    private void AddJwt()
    {
        var token = _httpContextAccessor.HttpContext!.Session.GetString("AccessToken");
        _httpClient.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token)
            ? null
            : new AuthenticationHeaderValue("Bearer", token);
    }
}
