using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Business.DTOs.BookCopyDTOs
{
    /// <summary>
    /// DTO tạo 1 bản sao sách
    /// </summary>
    public class CreateBookCopyDto
    {
        [Required(ErrorMessage = "BookId là bắt buộc.")]
        public Guid BookId { get; set; }

        [Required(ErrorMessage = "Barcode là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Barcode không được vượt quá 100 ký tự.")]
        public string Barcode { get; set; } = null!;

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        [RegularExpression("^(Available|Borrowed|Lost|Damaged)$",
            ErrorMessage = "Trạng thái phải là Available, Borrowed, Lost hoặc Damaged.")]
        public string Status { get; set; } = "Available";

        [Range(typeof(decimal), "1", "9999999999999999.99", ParseLimitsInInvariantCulture = true, ErrorMessage = "Giá thay thế phải lớn hơn 0.")]
        public decimal ReplacementPrice { get; set; }

        /// <summary>Ô kệ chứa bản sao này (nullable — có thể thêm trước rồi xếp kệ sau)</summary>
        public Guid? ShelfSlotId { get; set; }

        public DateOnly? AddedDate { get; set; }
    }
}
