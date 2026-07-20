using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.MVC.ViewModels.BookCopies
{
    public class CreateBookCopyViewModel
    {
        public Guid BookId { get; set; }
        public string? BookTitle { get; set; }

        [Required(ErrorMessage = "Barcode là bắt buộc.")]
        [StringLength(100)]
        public string Barcode { get; set; } = null!;

        [Required]
        public string Status { get; set; } = "Available";

        [Range(typeof(decimal), "1", "9999999999999999.99", ParseLimitsInInvariantCulture = true, ErrorMessage = "Giá thay thế phải lớn hơn 0.")]
        public decimal ReplacementPrice { get; set; }

        public Guid? ShelfSlotId { get; set; }

        public DateOnly? AddedDate { get; set; }
    }
}
