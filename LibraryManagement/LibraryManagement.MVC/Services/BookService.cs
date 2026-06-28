using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Books;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace LibraryManagement.MVC.Services
{
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



        public async Task<BookListViewModel?> GetBooksAsync(string? searchTerm, int? publisherId, int? publicationYear, string? language, int pageNumber, int pageSize)
        {

            var url = $"api/books?pageNumber={pageNumber}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(searchTerm))
                url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
            if (publisherId.HasValue)
                url += $"&publisherId={publisherId}";
            if (publicationYear.HasValue)
                url += $"&publicationYear={publicationYear}";
            if (!string.IsNullOrWhiteSpace(language))
                url += $"&language={Uri.EscapeDataString(language)}";

            var response = await _httpClient.GetAsync(url);
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

        // ── Helper ────────────────────────────────────────────────────────────────

        private static async Task<string> ReadErrorMessage(HttpResponseMessage response, string fallback)
        {
            try
            {
                var body = await response.Content.ReadAsStringAsync();
                var err = JsonSerializer.Deserialize<ErrorResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return err?.Message ?? fallback;
            }
            catch
            {
                return fallback;
            }
        }
    }
}
