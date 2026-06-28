using System;
using LibraryManagement.Data.Common;

namespace LibraryManagement.Models.Queries
{
    public class BookCopyQuery : PaginationParams
    {
        public string? SearchTerm { get; set; }   // tìm theo Barcode
        public Guid? BookId { get; set; }          // lọc theo sách
        public string? Status { get; set; }        // Available, Borrowed, Lost, Damaged
        public string? Location { get; set; }      // lọc theo vị trí
        public bool? IncludeHidden { get; set; }   // ẩn/hiện (dùng IsActive)
    }
}
