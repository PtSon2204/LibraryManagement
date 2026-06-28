using System.Text;
using System.Text.Json;
using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Books;

namespace LibraryManagement.MVC.Services;

internal class ErrorResponse
{
    public string? Message { get; set; }
}

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

    public async Task<BookListViewModel?> GetBooksAsync(string? searchTerm, int? publisherId, int? publicationYear, string? language, int pageNumber, int pageSize)
    {
        var query = new List<string>
        {
            $"pageNumber={pageNumber}",
            $"pageSize={pageSize}"
        };

        AddQuery(query, "searchTerm", searchTerm);
        if (publisherId.HasValue) query.Add($"publisherId={publisherId.Value}");
        if (publicationYear.HasValue) query.Add($"publicationYear={publicationYear.Value}");
        AddQuery(query, "language", language);

        var response = await _httpClient.GetAsync($"api/books?{string.Join('&', query)}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<BookListViewModel>(json, _jsonOptions);
        if (result != null)
        {
            result.SearchTerm = searchTerm;
            result.PublisherId = publisherId;
            result.PublicationYear = publicationYear;
            result.Language = language;
        }

        return result;
    }

    public async Task<BookViewModel?> GetBookByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"api/books/{id}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<BookViewModel>(json, _jsonOptions);
    }

    public async Task<string?> CreateBookAsync(CreateBookViewModel model)
    {
        var payload = new
        {
            model.Title,
            model.ISBN,
            model.PublisherId,
            model.PublicationYear,
            model.Language,
            model.Edition,
            model.Description,
            model.CoverImageUrl
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("api/books", content);
        if (response.IsSuccessStatusCode) return null;

        return await ReadErrorMessage(response, "Thêm sách thất bại. Vui lòng thử lại.");
    }

    public async Task<string?> UpdateBookAsync(UpdateBookViewModel model)
    {
        var payload = new
        {
            model.BookId,
            model.Title,
            model.ISBN,
            model.PublisherId,
            model.PublicationYear,
            model.Language,
            model.Edition,
            model.Description,
            model.CoverImageUrl
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"api/books/{model.BookId}", content);
        if (response.IsSuccessStatusCode) return null;

        return await ReadErrorMessage(response, "Cập nhật sách thất bại. Vui lòng thử lại.");
    }

    public async Task<bool> ToggleHideBookAsync(Guid id)
    {
        var response = await _httpClient.PutAsync($"api/books/{id}/toggle-hide", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteBookAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/books/{id}");
        return response.IsSuccessStatusCode;
    }

    private static void AddQuery(List<string> query, string name, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            query.Add($"{name}={Uri.EscapeDataString(value)}");
    }

    private static async Task<string> ReadErrorMessage(HttpResponseMessage response, string fallback)
    {
        try
        {
            var body = await response.Content.ReadAsStringAsync();
            var err = JsonSerializer.Deserialize<ErrorResponse>(body, _jsonOptions);
            return err?.Message ?? fallback;
        }
        catch
        {
            return fallback;
        }
    }
}
