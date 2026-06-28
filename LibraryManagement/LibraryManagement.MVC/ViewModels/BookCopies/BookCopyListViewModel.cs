using System;
using System.Collections.Generic;

namespace LibraryManagement.MVC.ViewModels.BookCopies
{
    /// <summary>
    /// ViewModel cho trang quản lý bản sao của 1 cuốn sách
    /// </summary>
    public class BookCopyListViewModel
    {
        // Thông tin sách cha
        public Guid BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public string? BookISBN { get; set; }

        // Danh sách bản sao (phân trang)
        public List<BookCopyViewModel> Data { get; set; } = new();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }

        // Filter
        public string? SearchTerm { get; set; }
        public string? StatusFilter { get; set; }
        public string? LocationFilter { get; set; }
    }
}
