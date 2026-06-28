using System;

namespace LibraryManagement.MVC.ViewModels.Books
{
    /// <summary>ViewModel hiển thị thông tin 1 cuốn sách (dùng cho Details)</summary>
    public class BookViewModel
    {
        public Guid BookId { get; set; }
        public string Title { get; set; } = null!;
        public string? ISBN { get; set; }
        public int? PublisherId { get; set; }
        public string? PublisherName { get; set; }
        public int? PublicationYear { get; set; }
        public string? Language { get; set; }
        public string? Edition { get; set; }
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsHidden { get; set; }
    }
}
