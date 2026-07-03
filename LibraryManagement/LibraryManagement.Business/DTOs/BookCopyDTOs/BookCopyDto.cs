using System;

namespace LibraryManagement.Business.DTOs.BookCopyDTOs
{
    /// <summary>
    /// DTO trả về thông tin 1 bản sao
    /// </summary>
    public class BookCopyDto
    {
        public Guid CopyId { get; set; }
        public Guid BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public string Barcode { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateOnly AddedDate { get; set; }

        /// <summary>ID ô kệ (null nếu chưa xếp kệ)</summary>
        public Guid? ShelfSlotId { get; set; }

        /// <summary>Thông tin vị trí đầy đủ: "Tầng 1 > Giá A > Kệ 2 > S01"</summary>
        public string? SlotLocation { get; set; }
    }
}
