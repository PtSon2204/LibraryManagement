using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Shelf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Services
{
    public class ShelfService : IShelfService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

        public ShelfService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private async Task<List<FloorViewModel>> FetchFloorsAsync()
        {
            try { return await _httpClient.GetFromJsonAsync<List<FloorViewModel>>("api/shelves/floors", _opts) ?? new(); }
            catch { return new(); }
        }

        private async Task<List<CategoryBriefViewModel>> FetchCategoriesAsync()
        {
            try
            {
                // Gọi API lấy toàn bộ thể loại (không phân trang)
                var raw = await _httpClient.GetFromJsonAsync<List<CategoryItem>>("api/categories/all", _opts);
                return raw?.Select(c => new CategoryBriefViewModel
                {
                    CategoryId   = c.CategoryId,
                    CategoryName = c.CategoryName
                }).ToList() ?? new();
            }
            catch { return new(); }
        }

        private async Task<List<ShelfTreeDto>?> FetchTreeAsync()
        {
            try { return await _httpClient.GetFromJsonAsync<List<ShelfTreeDto>>("api/shelves/tree", _opts); }
            catch { return null; }
        }

        private async Task<BookshelfViewModel?> FetchBookshelfAsync(Guid id)
        {
            try { return await _httpClient.GetFromJsonAsync<BookshelfViewModel>($"api/shelves/bookshelves/{id}", _opts); }
            catch { return null; }
        }

        private static string? ReadError(HttpResponseMessage response, string fallback)
        {
            try
            {
                var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var err  = JsonSerializer.Deserialize<ApiError>(body, _opts);
                return err?.Message ?? fallback;
            }
            catch { return fallback; }
        }

        // ── Index ─────────────────────────────────────────────────────────────────

        public async Task<ShelfIndexViewModel?> GetIndexViewModelAsync(Guid? floorId, string? availability)
        {
            var treeTask       = FetchTreeAsync();
            var floorsTask     = FetchFloorsAsync();
            var categoriesTask = FetchCategoriesAsync();

            await Task.WhenAll(treeTask, floorsTask, categoriesTask);

            var tree       = treeTask.Result;
            var allFloors  = floorsTask.Result;
            var allCats    = categoriesTask.Result;

            if (tree == null) return null;

            // Build card models từ tree (có đủ slot/capacity info)
            var floorGroups = tree.Select(f => new FloorWithBookshelvesViewModel
            {
                FloorId     = f.FloorId,
                FloorNumber = f.FloorNumber,
                FloorName   = f.FloorName,
                Description = f.Description,
                Bookshelves = f.Bookshelves.Select(b =>
                {
                    var totalCap  = b.Shelves.SelectMany(s => s.Slots).Sum(sl => sl.Capacity);
                    var totalUsed = b.Shelves.SelectMany(s => s.Slots).Sum(sl => sl.CurrentQuantity);
                    return new BookshelfCardViewModel
                    {
                        BookshelfId   = b.BookshelfId,
                        ShelfCode     = b.ShelfCode,
                        Name          = b.Name,
                        Description   = b.Description,
                        Categories    = b.Categories,
                        TotalShelves  = b.Shelves.Count,
                        TotalCapacity = totalCap,
                        TotalUsed     = totalUsed
                    };
                }).ToList()
            }).ToList();

            // Lọc theo tầng
            if (floorId.HasValue)
                floorGroups = floorGroups.Where(f => f.FloorId == floorId.Value).ToList();

            // Lọc theo availability
            if (availability == "available")
                floorGroups.ForEach(f => f.Bookshelves = f.Bookshelves.Where(b => b.HasAvailableSpace).ToList());
            else if (availability == "full")
                floorGroups.ForEach(f => f.Bookshelves = f.Bookshelves.Where(b => !b.HasAvailableSpace).ToList());

            // Xóa floor trống sau filter
            floorGroups = floorGroups.Where(f => f.Bookshelves.Count > 0 || !floorId.HasValue).ToList();

            return new ShelfIndexViewModel
            {
                Floors            = floorGroups,
                AllFloors         = allFloors,
                AllCategories     = allCats,
                FilterFloorId     = floorId,
                FilterAvailability = availability
            };
        }

        // ── Form data ─────────────────────────────────────────────────────────────

        public async Task<BookshelfFormViewModel?> GetCreateFormDataAsync()
        {
            var floors = await FetchFloorsAsync();
            var cats   = await FetchCategoriesAsync();
            return new BookshelfFormViewModel
            {
                Floors        = floors,
                AllCategories = cats
            };
        }

        public async Task<BookshelfFormViewModel?> GetEditFormDataAsync(Guid bookshelfId)
        {
            var bsTask     = FetchBookshelfAsync(bookshelfId);
            var floorsTask = FetchFloorsAsync();
            var catsTask   = FetchCategoriesAsync();

            await Task.WhenAll(bsTask, floorsTask, catsTask);

            var bs = bsTask.Result;
            if (bs == null) return null;

            return new BookshelfFormViewModel
            {
                BookshelfId   = bs.BookshelfId,
                FloorId       = bs.FloorId,
                ShelfCode     = bs.ShelfCode,
                Name          = bs.Name,
                Description   = bs.Description,
                CategoryIds   = bs.Categories.Select(c => c.CategoryId).ToList(),
                Floors        = floorsTask.Result,
                AllCategories = catsTask.Result
            };
        }

        // ── CRUD ─────────────────────────────────────────────────────────────────

        public async Task<string?> CreateBookshelfAsync(BookshelfFormViewModel model)
        {
            var payload = new
            {
                model.FloorId,
                model.ShelfCode,
                model.Name,
                model.Description,
                model.CategoryIds
            };
            var content  = Serialize(payload);
            var response = await _httpClient.PostAsync("api/shelves/bookshelves", content);
            if (response.IsSuccessStatusCode) return null;
            return ReadError(response, "Thêm giá sách thất bại. Vui lòng thử lại.");
        }

        public async Task<string?> UpdateBookshelfAsync(BookshelfFormViewModel model)
        {
            var payload = new
            {
                model.BookshelfId,
                model.ShelfCode,
                model.Name,
                model.Description,
                model.CategoryIds
            };
            var content  = Serialize(payload);
            var response = await _httpClient.PutAsync($"api/shelves/bookshelves/{model.BookshelfId}", content);
            if (response.IsSuccessStatusCode) return null;
            return ReadError(response, "Cập nhật giá sách thất bại. Vui lòng thử lại.");
        }

        public async Task<bool> DeleteBookshelfAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/shelves/bookshelves/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Floor CRUD ─────────────────────────────────────────────────────────────

        public async Task<List<FloorViewModel>> GetAllFloorsAsync()
        {
            return await FetchFloorsAsync();
        }

        public async Task<FloorFormViewModel?> GetFloorEditFormDataAsync(Guid id)
        {
            var floors = await FetchFloorsAsync();
            var floor = floors.FirstOrDefault(f => f.FloorId == id);
            if (floor == null) return null;

            return new FloorFormViewModel
            {
                FloorId     = floor.FloorId,
                FloorNumber = floor.FloorNumber,
                FloorName   = floor.FloorName,
                Description = floor.Description
            };
        }

        public async Task<string?> CreateFloorAsync(FloorFormViewModel model)
        {
            var payload = new { model.FloorNumber, model.FloorName, model.Description };
            var content  = Serialize(payload);
            var response = await _httpClient.PostAsync("api/shelves/floors", content);
            if (response.IsSuccessStatusCode) return null;
            return ReadError(response, "Thêm tầng thất bại. Vui lòng kiểm tra lại số tầng (có thể đã tồn tại).");
        }

        public async Task<string?> UpdateFloorAsync(FloorFormViewModel model)
        {
            var payload = new { model.FloorId, model.FloorNumber, model.FloorName, model.Description };
            var content  = Serialize(payload);
            var response = await _httpClient.PutAsync($"api/shelves/floors/{model.FloorId}", content);
            if (response.IsSuccessStatusCode) return null;
            return ReadError(response, "Cập nhật tầng thất bại.");
        }

        public async Task<bool> DeleteFloorAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/shelves/floors/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Shelf (Kệ) CRUD ────────────────────────────────────────────────────────

        public async Task<BookshelfTreeDto?> GetBookshelfDetailsAsync(Guid id)
        {
            var tree = await FetchTreeAsync();
            return tree.SelectMany(f => f.Bookshelves).FirstOrDefault(b => b.BookshelfId == id);
        }

        public async Task<string?> CreateShelfAsync(ShelfFormViewModel model)
        {
            var payload = new { model.BookshelfId, model.ShelfNumber, model.Name };
            var content  = Serialize(payload);
            var response = await _httpClient.PostAsync("api/shelves/racks", content);
            if (response.IsSuccessStatusCode) return null;
            return ReadError(response, "Thêm kệ thất bại.");
        }

        public async Task<string?> UpdateShelfAsync(ShelfFormViewModel model)
        {
            var payload = new { model.ShelfId, model.ShelfNumber, model.Name };
            var content  = Serialize(payload);
            var response = await _httpClient.PutAsync($"api/shelves/racks/{model.ShelfId}", content);
            if (response.IsSuccessStatusCode) return null;
            return ReadError(response, "Cập nhật kệ thất bại.");
        }

        public async Task<bool> DeleteShelfAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/shelves/racks/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── ShelfSlot (Vị trí chứa) CRUD ──────────────────────────────────────────

        public async Task<List<ShelfSlotViewModel>> GetAllSlotsAsync()
        {
            try
            {
                var slots = await _httpClient.GetFromJsonAsync<List<ShelfSlotViewModel>>("api/shelves/slots", _opts);
                return slots ?? new List<ShelfSlotViewModel>();
            }
            catch { return new List<ShelfSlotViewModel>(); }
        }

        public async Task<string?> CreateSlotAsync(ShelfSlotFormViewModel model)
        {
            var payload = new { model.ShelfId, model.SlotCode, model.Capacity, model.Description };
            var content  = Serialize(payload);
            var response = await _httpClient.PostAsync("api/shelves/slots", content);
            if (response.IsSuccessStatusCode) return null;
            return ReadError(response, "Thêm slot thất bại.");
        }

        public async Task<string?> UpdateSlotAsync(ShelfSlotFormViewModel model)
        {
            var payload = new { model.SlotId, model.SlotCode, model.Capacity, model.Description };
            var content  = Serialize(payload);
            var response = await _httpClient.PutAsync($"api/shelves/slots/{model.SlotId}", content);
            if (response.IsSuccessStatusCode) return null;
            return ReadError(response, "Cập nhật slot thất bại.");
        }

        public async Task<bool> DeleteSlotAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/shelves/slots/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Serialize helper ──────────────────────────────────────────────────────

        private static StringContent Serialize(object obj) =>
            new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
    }

    // ── Internal response helpers ─────────────────────────────────────────────────

    internal class ApiError
    {
        public string? Message { get; set; }
    }

    internal class CategoryListResponse
    {
        public List<CategoryItem>? Data { get; set; }
    }

    internal class CategoryItem
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
