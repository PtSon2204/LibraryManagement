using System;

namespace LibraryManagement.MVC.ViewModels.Loans
{
    public class LoanDetailViewModel
    {
        public Guid LoanDetailId { get; set; }
        public Guid CopyId { get; set; }
        public string BookTitle { get; set; } = null!;
        public string Barcode { get; set; } = null!;
        public string? CoverImageUrl { get; set; }
        public DateTime? ReturnedAt { get; set; }
        public string Status { get; set; } = null!;
    }
}
