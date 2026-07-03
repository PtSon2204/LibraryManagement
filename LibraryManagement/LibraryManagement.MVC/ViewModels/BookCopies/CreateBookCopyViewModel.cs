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

        public Guid? ShelfSlotId { get; set; }

        public DateOnly? AddedDate { get; set; }
    }
}
