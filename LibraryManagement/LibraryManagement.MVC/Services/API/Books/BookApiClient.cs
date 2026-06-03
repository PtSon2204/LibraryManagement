using System.Globalization;
using LibraryManagement.MVC.Interface.API.Books;
using LibraryManagement.MVC.Services.API.Common;
using LibraryManagement.MVC.ViewModels.Books;

namespace LibraryManagement.MVC.Services.API.Books
{
    public class BookApiClient : IBookApiClient
    {
        private readonly HttpClient _httpClient;

        public BookApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ODataResponse<BookListItemViewModel>> GetBooksAsync(BookSearchViewModel search)
        {
            var url = BuildBooksODataUrl(search, includeCount: true);

            return await _httpClient.GetFromJsonAsync<ODataResponse<BookListItemViewModel>>(url)
                ?? new ODataResponse<BookListItemViewModel>();
        }

        public async Task<List<BookListItemViewModel>> GetLatestBooksAsync(int count = 6)
        {
            var url = $"/odata/books?$top={count}&$orderby=Title asc";
            var response = await _httpClient.GetFromJsonAsync<ODataResponse<BookListItemViewModel>>(url);

            return response?.Value ?? new List<BookListItemViewModel>();
        }

        public async Task<BookDetailViewModel?> GetBookDetailAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"/api/books/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<BookDetailViewModel>();
        }

        public async Task<BookDetailViewModel?> AddBookAsync(BookCreateViewModel book)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/books", book);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<BookDetailViewModel>();
        }

        private static string BuildBooksODataUrl(BookSearchViewModel search, bool includeCount)
        {
            var queryParts = new List<string>();
            var filters = new List<string>();

            if (!string.IsNullOrWhiteSpace(search.Title))
            {
                filters.Add($"contains(Title,'{EscapeODataString(search.Title)}')");
            }

            if (!string.IsNullOrWhiteSpace(search.Language))
            {
                filters.Add($"Language eq '{EscapeODataString(search.Language)}'");
            }

            if (!string.IsNullOrWhiteSpace(search.Publisher))
            {
                filters.Add($"contains(PublisherName,'{EscapeODataString(search.Publisher)}')");
            }

            if (search.AvailableOnly)
            {
                filters.Add("AvailableCopies gt 0");
            }

            if (filters.Count > 0)
            {
                queryParts.Add("$filter=" + Uri.EscapeDataString(string.Join(" and ", filters)));
            }

            var orderBy = NormalizeSort(search.SortBy);
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                queryParts.Add("$orderby=" + Uri.EscapeDataString(orderBy));
            }

            var pageSize = Math.Clamp(search.PageSize, 1, 100);
            var page = Math.Max(search.Page, 1);
            var skip = (page - 1) * pageSize;

            queryParts.Add("$top=" + pageSize.ToString(CultureInfo.InvariantCulture));
            queryParts.Add("$skip=" + skip.ToString(CultureInfo.InvariantCulture));

            if (includeCount)
            {
                queryParts.Add("$count=true");
            }

            return "/odata/books?" + string.Join("&", queryParts);
        }

        private static string EscapeODataString(string value)
        {
            return value.Trim().Replace("'", "''");
        }

        private static string NormalizeSort(string? sortBy)
        {
            return sortBy switch
            {
                "title_desc" => "Title desc",
                "year_asc" => "PublicationYear asc",
                "year_desc" => "PublicationYear desc",
                "available_desc" => "AvailableCopies desc",
                _ => "Title asc"
            };
        }
    }
}
