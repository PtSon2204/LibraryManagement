using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace LibraryManagement.MVC.ViewModels.Books
{
    /// <summary>ViewModel dùng cho form Chỉnh sửa sách (popup Edit)</summary>
    public class UpdateBookViewModel
    {
        /// <summary>ID sách cần cập nhật (bắt buộc)</summary>
        public Guid BookId { get; set; }

        public string Title { get; set; } = null!;
        public string? ISBN { get; set; }
        public int? PublisherId { get; set; }
        public int? PublicationYear { get; set; }
        public string? Language { get; set; }
        public string? Edition { get; set; }
        public string? Description { get; set; }

        /// <summary>File ảnh bìa mới upload từ máy. Nếu null → giữ ảnh cũ</summary>
        public IFormFile? CoverImageFile { get; set; }

        /// <summary>URL ảnh bìa hiện tại hoặc URL mới sau khi upload</summary>
        public string? CoverImageUrl { get; set; }

        // Dữ liệu phụ trợ cho dropdown — không submit lên server
        public List<PublisherOption> Publishers { get; set; } = new();
    }
}
