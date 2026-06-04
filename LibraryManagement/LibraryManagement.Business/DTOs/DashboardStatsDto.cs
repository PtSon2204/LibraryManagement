using System;
using System.Collections.Generic;

namespace LibraryManagement.Business.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalBooks { get; set; }
        public int TotalBookCopies { get; set; }
        public int TotalMembers { get; set; }
        public int ActiveLoans { get; set; }
        public int OverdueLoans { get; set; }
        public decimal TotalFinesAmount { get; set; }
        public decimal PendingFinesAmount { get; set; }
        public List<RecentLoanDto> RecentLoans { get; set; } = new();
        public List<MonthlyLoanStatsDto> MonthlyLoanStats { get; set; } = new();
    }

    public class RecentLoanDto
    {
        public Guid LoanId { get; set; }
        public string BookTitle { get; set; } = null!;
        public string BorrowerName { get; set; } = null!;
        public DateTime BorrowedAt { get; set; }
        public string Status { get; set; } = null!;
    }

    public class MonthlyLoanStatsDto
    {
        public string MonthName { get; set; } = null!;
        public int LoanCount { get; set; }
    }
}
