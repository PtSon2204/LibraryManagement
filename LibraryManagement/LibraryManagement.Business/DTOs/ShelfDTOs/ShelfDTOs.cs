using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Business.DTOs.ShelfDTOs
{
    // ─── Response DTOs ───────────────────────────────────────────────────────────

    public class FloorDto
    {
        public Guid FloorId { get; set; }
        public int FloorNumber { get; set; }
        public string FloorName { get; set; } = null!;
        public string? Description { get; set; }
        public int TotalBookshelves { get; set; }
    }

    public class BookshelfDto
    {
        public Guid BookshelfId { get; set; }
        public Guid FloorId { get; set; }
        public string FloorName { get; set; } = null!;
        public string ShelfCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<CategoryBriefDto> Categories { get; set; } = new();
        public int TotalShelves { get; set; }
    }

    public class CategoryBriefDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }

    public class ShelfDto
    {
        public Guid ShelfId { get; set; }
        public Guid BookshelfId { get; set; }
        public string BookshelfName { get; set; } = null!;
        public int ShelfNumber { get; set; }
        public string Name { get; set; } = null!;
        public int TotalSlots { get; set; }
    }

    public class ShelfSlotDto
    {
        public Guid SlotId { get; set; }
        public Guid ShelfId { get; set; }
        public string ShelfName { get; set; } = null!;
        public string SlotCode { get; set; } = null!;
        public int Capacity { get; set; }
        public int CurrentQuantity { get; set; }
        public string? Description { get; set; }
        /// <summary>Tỉ lệ lấp đầy (%)</summary>
        public double OccupancyRate => Capacity == 0 ? 0 : Math.Round((double)CurrentQuantity / Capacity * 100, 1);
    }

    // ─── Tree DTO (cây phân cấp đầy đủ) ─────────────────────────────────────────

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
        public List<CategoryBriefDto> Categories { get; set; } = new();
        public List<ShelfTreeNodeDto> Shelves { get; set; } = new();
    }

    public class ShelfTreeNodeDto
    {
        public Guid ShelfId { get; set; }
        public int ShelfNumber { get; set; }
        public string Name { get; set; } = null!;
        public List<ShelfSlotDto> Slots { get; set; } = new();
    }

    // ─── Create DTOs ─────────────────────────────────────────────────────────────

    public class CreateFloorDto
    {
        public int FloorNumber { get; set; }
        public string FloorName { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class CreateBookshelfDto
    {
        public Guid FloorId { get; set; }
        public string ShelfCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        /// <summary>Danh sách CategoryId gán cho giá sách này</summary>
        public List<int> CategoryIds { get; set; } = new();
    }

    public class CreateShelfDto
    {
        public Guid BookshelfId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Số tầng/kệ phải lớn hơn 0.")]
        public int ShelfNumber { get; set; }
        [Required(ErrorMessage = "Tên tầng/kệ không được để trống.")]
        public string Name { get; set; } = null!;
    }

    public class CreateShelfSlotDto
    {
        public Guid ShelfId { get; set; }
        [Required(ErrorMessage = "Mã vị trí không được để trống.")]
        public string SlotCode { get; set; } = null!;
        [Range(1, int.MaxValue, ErrorMessage = "Sức chứa phải lớn hơn 0.")]
        public int Capacity { get; set; } = 10;
        public string? Description { get; set; }
    }

    // ─── Update DTOs ─────────────────────────────────────────────────────────────

    public class UpdateFloorDto
    {
        public Guid FloorId { get; set; }
        public int FloorNumber { get; set; }
        public string FloorName { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class UpdateBookshelfDto
    {
        public Guid BookshelfId { get; set; }
        public string ShelfCode { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        /// <summary>Ghi đè toàn bộ danh sách thể loại của giá sách</summary>
        public List<int> CategoryIds { get; set; } = new();
    }

    public class UpdateShelfDto
    {
        public Guid ShelfId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Số tầng/kệ phải lớn hơn 0.")]
        public int ShelfNumber { get; set; }
        [Required(ErrorMessage = "Tên tầng/kệ không được để trống.")]
        public string Name { get; set; } = null!;
    }

    public class UpdateShelfSlotDto
    {
        public Guid SlotId { get; set; }
        [Required(ErrorMessage = "Mã vị trí không được để trống.")]
        public string SlotCode { get; set; } = null!;
        [Range(1, int.MaxValue, ErrorMessage = "Sức chứa phải lớn hơn 0.")]
        public int Capacity { get; set; }
        public string? Description { get; set; }
    }
}
