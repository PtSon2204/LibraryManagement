namespace LibraryManagement.MVC.Services
{
    public class LoanService : Interface.ILoanService
    {
        private readonly HttpClient _httpClient;
        private static readonly System.Text.Json.JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public LoanService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ViewModels.Loans.LoanListViewModel?> GetMyLoanHistoryAsync(string? searchTerm, string? status, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize)
        {
            var url = $"api/Loan/history?pageNumber={pageNumber}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(searchTerm))
                url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
            if (!string.IsNullOrWhiteSpace(status))
                url += $"&status={Uri.EscapeDataString(status)}";
            if (fromDate.HasValue)
                url += $"&fromDate={fromDate.Value:yyyy-MM-dd}";
            if (toDate.HasValue)
                url += $"&toDate={toDate.Value:yyyy-MM-dd}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<ViewModels.Loans.LoanListViewModel>(json, _jsonOptions);
            
            if (result != null)
            {
                result.SearchTerm = searchTerm;
                result.Status = status;
                result.FromDate = fromDate;
                result.ToDate = toDate;
            }

            return result;
        }

        public async Task<ViewModels.Loans.LoanViewModel?> GetLoanDetailAsync(Guid loanId)
        {
            var response = await _httpClient.GetAsync($"api/Loan/{loanId}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<ViewModels.Loans.LoanViewModel>(json, _jsonOptions);
        }
    }
}
