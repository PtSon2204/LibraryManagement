using System;
using System.Collections.Generic;

namespace LibraryManagement.MVC.ViewModels.Loans
{
    public class LoanListViewModel
    {
        public List<LoanViewModel> Data { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }

        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
