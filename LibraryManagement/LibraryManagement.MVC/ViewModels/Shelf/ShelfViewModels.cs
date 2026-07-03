using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.MVC.ViewModels.Shelf
{
    // ── Response từ API ──────────────────────────────────────────────────────────

    public class FloorViewModel
    {
        public Guid FloorId { get; set; }
        public int FloorNumber { get; set; }
        public string FloorName { get; set; } = null!;
        public string? Description { get; set; }
        public int TotalBookshelves { get; set; }
    }

    // Removed duplicate
    public class FloorFormViewModel
    {
        public Guid FloorId { get; set; }
        public int FloorNumber { get; set; }
        public string FloorName { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class CategoryBriefViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }

    public class BookshelfViewModel
    {
        public Guid BookshelfId { get; set; }
        public Guid FloorId { get; set; }
        public string FloorName { get; set; } = null!;
        public string ShelfCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<CategoryBriefViewModel> Categories { get; set; } = new();
        public int TotalShelves { get; set; }
    }

    public class ShelfSlotViewModel
    {
        public Guid SlotId { get; set; }
        public string ShelfName { get; set; } = null!;
        public string SlotCode { get; set; } = null!;
        public int Capacity { get; set; }
        public int CurrentQuantity { get; set; }
        public double OccupancyRate { get; set; }
        
        public string DisplayName => $"{ShelfName} - {SlotCode} (Trống: {Math.Max(0, Capacity - CurrentQuantity)}/{Capacity})";
    }

    public class ShelfRackViewModel
    {
        public Guid ShelfId { get; set; }
        public int ShelfNumber { get; set; }
        public string Name { get; set; } = null!;
        public List<ShelfSlotViewModel> Slots { get; set; } = new();

        public int TotalCapacity => Slots.Sum(s => s.Capacity);
        public int TotalUsed => Slots.Sum(s => s.CurrentQuantity);
        public int TotalFree => Math.Max(0, TotalCapacity - TotalUsed);
        public bool HasAvailableSpace => TotalFree > 0;
    }

    public class BookshelfDetailViewModel
    {
        public BookshelfViewModel Bookshelf { get; set; } = null!;
        public List<ShelfRackViewModel> Racks { get; set; } = new();

        public int TotalSlots => Racks.Sum(r => r.Slots.Count);
        public int TotalCapacity => Racks.Sum(r => r.TotalCapacity);
        public int TotalUsed => Racks.Sum(r => r.TotalUsed);
        public int TotalFree => Math.Max(0, TotalCapacity - TotalUsed);
        public bool HasAvailableSpace => TotalFree > 0;
        public double OccupancyRate => TotalCapacity == 0 ? 0
            : Math.Round((double)TotalUsed / TotalCapacity * 100, 1);
    }

    // ── Trang Index: groupBy Floor ────────────────────────────────────────────────

    public class ShelfIndexViewModel
    {
        public List<FloorWithBookshelvesViewModel> Floors { get; set; } = new();
        public List<FloorViewModel> AllFloors { get; set; } = new();     // for dropdown
        public List<CategoryBriefViewModel> AllCategories { get; set; } = new();

        // Bộ lọc
        public Guid? FilterFloorId { get; set; }
        public string? FilterAvailability { get; set; }  // "available" | "full" | ""
    }

    public class FloorWithBookshelvesViewModel
    {
        public Guid FloorId { get; set; }
        public int FloorNumber { get; set; }
        public string FloorName { get; set; } = null!;
        public string? Description { get; set; }
        public List<BookshelfCardViewModel> Bookshelves { get; set; } = new();
    }

    public class BookshelfCardViewModel
    {
        public Guid BookshelfId { get; set; }
        public string ShelfCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<CategoryBriefViewModel> Categories { get; set; } = new();
        public int TotalShelves { get; set; }
        public int TotalCapacity { get; set; }
        public int TotalUsed { get; set; }
        public int TotalFree => Math.Max(0, TotalCapacity - TotalUsed);
        public bool HasAvailableSpace => TotalFree > 0;
        public double OccupancyRate => TotalCapacity == 0 ? 0
            : Math.Round((double)TotalUsed / TotalCapacity * 100, 1);

        /// "success" | "warning" | "danger"
        public string StatusColor => OccupancyRate >= 90 ? "danger"
            : OccupancyRate >= 60 ? "warning" : "success";

        public string StatusLabel => OccupancyRate >= 90 ? "Gần đầy"
            : OccupancyRate >= 60 ? "Còn ít chỗ" : "Còn trống";
    }

    // ── Form Create / Edit ────────────────────────────────────────────────────────

    public class BookshelfFormViewModel
    {
        public Guid BookshelfId { get; set; }          // Guid.Empty khi tạo mới
        public Guid FloorId { get; set; }
        public string ShelfCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<int> CategoryIds { get; set; } = new();

        // Dữ liệu cho dropdown
        public List<FloorViewModel> Floors { get; set; } = new();
        public List<CategoryBriefViewModel> AllCategories { get; set; } = new();
    }

    // ── Tree từ API (dùng để tính availability) ───────────────────────────────────

    public class ShelfTreeDto
    {
        public Guid FloorId { get; set; }
        public int FloorNumber { get; set; }
        public string FloorName { get; set; } = null!;
        public string? Description { get; set; }
        public List<BookshelfTreeDto> Bookshelves { get; set; } = new();
    }

    public class BookshelfTreeDto
    {
        public Guid BookshelfId { get; set; }
        public string ShelfCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<CategoryBriefViewModel> Categories { get; set; } = new();
        public List<ShelfRackTreeDto> Shelves { get; set; } = new();
    }

    public class ShelfRackTreeDto
    {
        public Guid ShelfId { get; set; }
        public int ShelfNumber { get; set; }
        public string Name { get; set; } = null!;
        public List<SlotTreeDto> Slots { get; set; } = new();
    }

    public class SlotTreeDto
    {
        public Guid SlotId { get; set; }
        public string SlotCode { get; set; } = null!;
        public int Capacity { get; set; }
        public int CurrentQuantity { get; set; }
        public double OccupancyRate { get; set; }
    }

    public class ShelfFormViewModel
    {
        public Guid ShelfId { get; set; }
        public Guid BookshelfId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Số tầng/kệ phải lớn hơn 0.")]
        public int ShelfNumber { get; set; }
        [Required(ErrorMessage = "Tên tầng/kệ không được để trống.")]
        public string Name { get; set; } = null!;
    }

    public class ShelfSlotFormViewModel
    {
        public Guid SlotId { get; set; }
        public Guid ShelfId { get; set; }
        [Required(ErrorMessage = "Mã vị trí không được để trống.")]
        public string SlotCode { get; set; } = null!;
        [Range(1, int.MaxValue, ErrorMessage = "Sức chứa phải lớn hơn 0.")]
        public int Capacity { get; set; } = 10;
        public string? Description { get; set; }
    }
}
