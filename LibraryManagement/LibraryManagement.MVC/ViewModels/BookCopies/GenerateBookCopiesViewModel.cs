using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.MVC.ViewModels.BookCopies
{
    /// <summary>
    /// ViewModel dùng để generate nhiều bản sao tự động
    /// </summary>
    public class GenerateBookCopiesViewModel
    {
        public Guid BookId { get; set; }
        public string? BookTitle { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc.")]
        [Range(1, 100, ErrorMessage = "Số lượng phải từ 1 đến 100.")]
        public int Quantity { get; set; } = 1;

        [Required(ErrorMessage = "Tiền tố barcode là bắt buộc.")]
        [StringLength(20, ErrorMessage = "Tiền tố không được quá 20 ký tự.")]
        public string BarcodePrefix { get; set; } = null!;

        [Required(ErrorMessage = "Số bắt đầu là bắt buộc.")]
        [StringLength(10)]
        public string StartNumber { get; set; } = "0001";

        [StringLength(200)]
        public string? Location { get; set; }

        public string Status { get; set; } = "Available";
    }
}
