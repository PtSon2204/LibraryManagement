using System;
using LibraryManagement.Data.Common;

namespace LibraryManagement.Models.Queries
{
    public class LoanQuery : PaginationParams
    {
        public string? SearchTerm { get; set; } // Can search by BookTitle, Barcode
        public string? Status { get; set; } // e.g. "Borrowed", "Returned"
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
