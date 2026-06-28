using System;
using System.ComponentModel.DataAnnotations;

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
        public string? Location { get; set; }
        public DateOnly AddedDate { get; set; }
    }
}
