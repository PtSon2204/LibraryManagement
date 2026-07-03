using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.BookCopies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Services
{
    public class BookCopyService : IBookCopyService
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public BookCopyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ── GET LIST ──────────────────────────────────────────────────────────────

        public async Task<BookCopyListViewModel?> GetBookCopiesAsync(
            Guid bookId, string? searchTerm, string? status,
            string? location, int pageNumber, int pageSize)
        {
            var url = $"api/book-copies?bookId={bookId}&pageNumber={pageNumber}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(searchTerm))
                url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
            if (!string.IsNullOrWhiteSpace(status))
                url += $"&status={Uri.EscapeDataString(status)}";
            if (!string.IsNullOrWhiteSpace(location))
                url += $"&location={Uri.EscapeDataString(location)}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var paged = JsonSerializer.Deserialize<PagedApiResponse<BookCopyApiDto>>(json, _jsonOpts);
            if (paged == null) return null;

            // Lấy BookTitle từ copy đầu tiên (nếu có)
            var firstItem = paged.Data?.FirstOrDefault();

            return new BookCopyListViewModel
            {
                BookId       = bookId,
                BookTitle    = firstItem?.BookTitle ?? string.Empty,
                Data         = paged.Data?.Select(MapToViewModel).ToList() ?? new(),
                TotalRecords = paged.TotalRecords,
                PageNumber   = paged.PageNumber,
                PageSize     = paged.PageSize,
                TotalPages   = paged.TotalPages,
                SearchTerm   = searchTerm,
                StatusFilter = status,
                LocationFilter = location
            };
        }

        // ── GET BY ID ─────────────────────────────────────────────────────────────

        public async Task<BookCopyViewModel?> GetBookCopyByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"api/book-copies/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var dto = JsonSerializer.Deserialize<BookCopyApiDto>(json, _jsonOpts);
            return dto == null ? null : MapToViewModel(dto);
        }

        // ── CREATE SINGLE ─────────────────────────────────────────────────────────

        public async Task<string?> CreateBookCopyAsync(CreateBookCopyViewModel model)
        {
            var payload = new
            {
                model.BookId,
                model.Barcode,
                model.Status,
                model.ShelfSlotId,
                model.AddedDate
            };

            var content  = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/book-copies", content);
            if (response.IsSuccessStatusCode) return null;

            return await ReadErrorMessage(response, "Thêm bản sao thất bại. Vui lòng thử lại.");
        }

        // ── GENERATE MULTIPLE ─────────────────────────────────────────────────────

        public async Task<string?> GenerateBookCopiesAsync(GenerateBookCopiesViewModel model)
        {
            // Parse số bắt đầu, giữ nguyên độ rộng
            if (!int.TryParse(model.StartNumber, out int start))
                return "Số bắt đầu không hợp lệ.";

            int padWidth = model.StartNumber.Length;

            var copies = Enumerable.Range(start, model.Quantity)
                .Select(n => new
                {
                    Barcode   = $"{model.BarcodePrefix}{n.ToString().PadLeft(padWidth, '0')}",
                    Status    = model.Status,
                    ShelfSlotId = model.ShelfSlotId,
                    AddedDate = (DateOnly?)null
                }).ToList();

            var payload  = new { BookId = model.BookId, Copies = copies };
            var content  = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/book-copies/batch", content);
            if (response.IsSuccessStatusCode) return null;

            return await ReadErrorMessage(response, "Tạo bản sao hàng loạt thất bại. Vui lòng thử lại.");
        }

        // ── UPDATE ────────────────────────────────────────────────────────────────

        public async Task<string?> UpdateBookCopyAsync(UpdateBookCopyViewModel model)
        {
            var payload = new
            {
                model.CopyId,
                model.Barcode,
                model.Status,
                model.ShelfSlotId
            };

            var content  = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/book-copies/{model.CopyId}", content);
            if (response.IsSuccessStatusCode) return null;

            return await ReadErrorMessage(response, "Cập nhật bản sao thất bại. Vui lòng thử lại.");
        }

        // ── TOGGLE HIDE ───────────────────────────────────────────────────────────

        public async Task<bool> ToggleHideAsync(Guid id)
        {
            var response = await _httpClient.PutAsync($"api/book-copies/{id}/toggle-hide", null);
            return response.IsSuccessStatusCode;
        }

        // ── DELETE ────────────────────────────────────────────────────────────────

        public async Task<bool> DeleteBookCopyAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/book-copies/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── HELPERS ───────────────────────────────────────────────────────────────

        private static BookCopyViewModel MapToViewModel(BookCopyApiDto dto) => new()
        {
            CopyId    = dto.CopyId,
            BookId    = dto.BookId,
            BookTitle = dto.BookTitle,
            Barcode   = dto.Barcode,
            Status    = dto.Status,
            Location  = dto.SlotLocation,
            ShelfSlotId = dto.ShelfSlotId,
            AddedDate = dto.AddedDate
        };

        private static async Task<string> ReadErrorMessage(HttpResponseMessage response, string fallback)
        {
            try
            {
                var body = await response.Content.ReadAsStringAsync();
                var err  = JsonSerializer.Deserialize<ErrorApiResponse>(body, _jsonOpts);
                return err?.Message ?? fallback;
            }
            catch { return fallback; }
        }

        // ── Private DTOs (map từ API response) ───────────────────────────────────

        private class BookCopyApiDto
        {
            public Guid CopyId { get; set; }
            public Guid BookId { get; set; }
            public string BookTitle { get; set; } = null!;
            public string Barcode { get; set; } = null!;
            public string Status { get; set; } = null!;
            public Guid? ShelfSlotId { get; set; }
            public string? SlotLocation { get; set; }
            public DateOnly AddedDate { get; set; }
        }

        private class PagedApiResponse<T>
        {
            public List<T>? Data { get; set; }
            public int TotalRecords { get; set; }
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int TotalPages { get; set; }
        }

        private class ErrorApiResponse
        {
            public string? Message { get; set; }
        }
    }
}
