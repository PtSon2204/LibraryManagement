using System;

namespace LibraryManagement.MVC.ViewModels.BookCopies
{
    public class BookCopyViewModel
    {
        public Guid CopyId { get; set; }
        public Guid BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public string Barcode { get; set; } = null!;
        public string Status { get; set; } = null!;
        public decimal ReplacementPrice { get; set; }
        public string? Location { get; set; }
        public Guid? ShelfSlotId { get; set; }
        public DateOnly AddedDate { get; set; }
    }
}
