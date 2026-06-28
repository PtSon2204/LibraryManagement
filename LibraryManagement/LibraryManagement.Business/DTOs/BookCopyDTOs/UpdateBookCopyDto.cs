using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Business.DTOs.BookCopyDTOs
{
    /// <summary>
    /// DTO cập nhật thông tin bản sao sách
    /// </summary>
    public class UpdateBookCopyDto
    {
        [Required(ErrorMessage = "CopyId là bắt buộc.")]
        public Guid CopyId { get; set; }

        [Required(ErrorMessage = "Barcode là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Barcode không được vượt quá 100 ký tự.")]
        public string Barcode { get; set; } = null!;

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        [RegularExpression("^(Available|Borrowed|Lost|Damaged)$",
            ErrorMessage = "Trạng thái phải là Available, Borrowed, Lost hoặc Damaged.")]
        public string Status { get; set; } = null!;

        [StringLength(200, ErrorMessage = "Vị trí không được vượt quá 200 ký tự.")]
        public string? Location { get; set; }
    }
}
