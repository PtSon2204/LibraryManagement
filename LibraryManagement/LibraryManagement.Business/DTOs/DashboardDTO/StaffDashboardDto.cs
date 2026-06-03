using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Business.DTOs.DashboardDTO
{
    public class StaffDashboardDto
    {
        public int TotalBooks { get; set; }
        public int TotalUsers { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public int ActiveLoans { get; set; }
        public int PendingReservations { get; set; }
        public int OverdueLoans { get; set; }
        public int UnpaidFines { get; set; }
        public List<RecentLoanDto> RecentLoans { get; set; } = new();
        public List<RecentReservationDto> RecentReservations { get; set; } = new();
    }

    public class RecentLoanDto
    {
        public Guid LoanId { get; set; }

        public string BorrowerName { get; set; } = string.Empty;

        public DateTime BorrowedAt { get; set; }

        public DateTime DueAt { get; set; }

        public string Status { get; set; } = string.Empty;
    }

    public class RecentReservationDto
    {
        public Guid ReservationId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string BookTitle { get; set; } = string.Empty;

        public DateTime ReservationDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
