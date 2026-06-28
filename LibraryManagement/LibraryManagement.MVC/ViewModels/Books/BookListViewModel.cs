using System.Collections.Generic;

namespace LibraryManagement.MVC.ViewModels.Books
{
    /// <summary>ViewModel hiển thị danh sách sách có phân trang và bộ lọc (dùng cho Index)</summary>
    public class BookListViewModel
    {
        public List<BookViewModel> Data { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }

        // Các tham số lọc — dùng để giữ state trên form
        public string? SearchTerm { get; set; }
        public int? PublisherId { get; set; }
        public int? PublicationYear { get; set; }
        public string? Language { get; set; }
    }
}
