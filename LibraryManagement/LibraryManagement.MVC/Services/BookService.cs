using System.Text.Json;
using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Books;

namespace LibraryManagement.MVC.Services;

public class BookService : IBookService
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public BookService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BookListPageViewModel?> GetBooksAsync(BookSearchViewModel search)
    {
        var query = new List<string>
        {
            $"page={search.Page}",
            $"pageSize={search.PageSize}",
            $"availableOnly={search.AvailableOnly.ToString().ToLowerInvariant()}"
        };

        AddQuery(query, "title", search.Title);
        AddQuery(query, "language", search.Language);
        AddQuery(query, "publisher", search.Publisher);
        AddQuery(query, "sortBy", search.SortBy);

        var response = await _httpClient.GetAsync($"api/books?{string.Join('&', query)}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<BookListPageViewModel>(json, _jsonOptions);

        if (result != null) result.Search = search;

        return result;
    }

    public async Task<List<BookListItemViewModel>> GetLatestBooksAsync(int count)
    {
        var response = await _httpClient.GetAsync($"api/books/latest?count={count}");
        if (!response.IsSuccessStatusCode) return new List<BookListItemViewModel>();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<BookListItemViewModel>>(json, _jsonOptions) ?? new List<BookListItemViewModel>();
    }

    public async Task<BookDetailViewModel?> GetBookDetailAsync(Guid bookId)
    {
        var response = await _httpClient.GetAsync($"api/books/{bookId}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<BookDetailViewModel>(json, _jsonOptions);
    }

    private static void AddQuery(List<string> query, string name, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            query.Add($"{name}={Uri.EscapeDataString(value)}");
    }
}
