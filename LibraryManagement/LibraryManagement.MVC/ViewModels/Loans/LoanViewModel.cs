using System;
using System.Collections.Generic;

namespace LibraryManagement.MVC.ViewModels.Loans
{
    public class LoanViewModel
    {
        public Guid LoanId { get; set; }
        public DateTime BorrowedAt { get; set; }
        public DateTime DueAt { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string? ProcessedByLibrarian { get; set; }
        
        public List<LoanDetailViewModel> LoanDetails { get; set; } = new();
    }
}
