using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace LibraryManagement.MVC.ViewModels.Books
{
    /// <summary>ViewModel dùng cho form Tạo mới sách (popup Create)</summary>
    public class CreateBookViewModel
    {
        public string Title { get; set; } = null!;
        public string? ISBN { get; set; }
        public int? PublisherId { get; set; }
        public int? PublicationYear { get; set; }
        public string? Language { get; set; }
        public string? Edition { get; set; }
        public string? Description { get; set; }

        /// <summary>File ảnh bìa upload từ máy (ưu tiên hơn CoverImageUrl)</summary>
        public IFormFile? CoverImageFile { get; set; }

        /// <summary>URL ảnh bìa — được set sau khi lưu file upload thành công</summary>
        public string? CoverImageUrl { get; set; }

        // Dữ liệu phụ trợ cho dropdown — không submit lên server
        public List<PublisherOption> Publishers { get; set; } = new();

        public List<int> AuthorIds { get; set; } = new();
        public List<AuthorOption> Authors { get; set; } = new();

        public List<int> CategoryIds { get; set; } = new();
        public List<CategoryOption> Categories { get; set; } = new();
    }
}
