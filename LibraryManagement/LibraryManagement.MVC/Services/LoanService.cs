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
        var query = BuildCurrentLoanQuery(search, includeStaffFilters: true);
        var response = await _httpClient.GetAsync($"api/loans?{query}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoanListPageViewModel>(json, _jsonOptions);
        if (result != null) result.Search = search;
        return result;
    }

    public async Task<ReaderLoanSummaryPageViewModel?> GetStaffReaderLoanSummariesAsync(LoanSearchViewModel search)
    {
        AddJwt();
        var query = BuildReaderLoanQuery(search);
        var response = await _httpClient.GetAsync($"api/loans/readers?{query}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ReaderLoanSummaryPageViewModel>(json, _jsonOptions);
        if (result != null) result.Search = search;
        return result;
    }

    public async Task<ReaderLoanWorkspaceViewModel?> GetStaffReaderLoanWorkspaceAsync(Guid readerId)
    {
        AddJwt();
        var response = await _httpClient.GetAsync($"api/loans/readers/{readerId}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ReaderLoanWorkspaceViewModel>(json, _jsonOptions);
    }

    public async Task<LoanListPageViewModel?> GetMyLoansAsync(LoanSearchViewModel search)
    {
        AddJwt();
        var query = BuildCurrentLoanQuery(search, includeStaffFilters: false);
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

    public async Task<string?> ConfirmBorrowRequestAsync(Guid loanDetailId, Guid copyId)
    {
        AddJwt();
        var response = await _httpClient.PostAsJsonAsync($"api/loans/{loanDetailId}/confirm", new { CopyId = copyId });
        if (response.IsSuccessStatusCode) return null;

        var error = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(error) ? "Không thể xác nhận yêu cầu mượn. Vui lòng thử lại." : error;
    }

    public async Task<string?> ConfirmBorrowRequestsAsync(Guid readerId, List<ConfirmLoanDetailItemViewModel> items)
    {
        AddJwt();
        var response = await _httpClient.PostAsJsonAsync($"api/loans/readers/{readerId}/confirm-batch", new { Items = items });
        if (response.IsSuccessStatusCode) return null;

        var error = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(error) ? "Không thể xác nhận các yêu cầu mượn. Vui lòng thử lại." : error;
    }

    public async Task<string?> ReturnBookAsync(Guid loanDetailId)
    {
        AddJwt();
        var response = await _httpClient.PostAsync($"api/loans/{loanDetailId}/return", null);
        if (response.IsSuccessStatusCode) return null;

        var error = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(error) ? "Không thể trả sách. Vui lòng thử lại." : error;
    }

    public async Task<LoanListViewModel?> GetMyLoanHistoryAsync(string? searchTerm, string? status, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize)
    {
        AddJwt();
        var query = new List<string>
        {
            $"pageNumber={pageNumber}",
            $"pageSize={pageSize}"
        };

        AddQuery(query, "searchTerm", searchTerm);
        AddQuery(query, "status", status);
        if (fromDate.HasValue) query.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");
        if (toDate.HasValue) query.Add($"toDate={toDate.Value:yyyy-MM-dd}");

        var response = await _httpClient.GetAsync($"api/loans/history?{string.Join('&', query)}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoanListViewModel>(json, _jsonOptions);

        if (result != null)
        {
            result.SearchTerm = searchTerm;
            result.Status = status;
            result.FromDate = fromDate;
            result.ToDate = toDate;
        }

        return result;
    }

    public async Task<LoanViewModel?> GetLoanDetailAsync(Guid loanId)
    {
        AddJwt();
        var response = await _httpClient.GetAsync($"api/loans/{loanId}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<LoanViewModel>(json, _jsonOptions);
    }

    private static string BuildCurrentLoanQuery(LoanSearchViewModel search, bool includeStaffFilters)
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

    private static string BuildReaderLoanQuery(LoanSearchViewModel search)
    {
        var query = new List<string>
        {
            $"page={Math.Max(search.Page, 1)}",
            $"pageSize={(search.PageSize <= 0 ? 10 : search.PageSize)}"
        };

        AddQuery(query, "search", search.Search);
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
