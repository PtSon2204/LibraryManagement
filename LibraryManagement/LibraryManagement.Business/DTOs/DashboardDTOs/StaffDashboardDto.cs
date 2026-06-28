namespace LibraryManagement.Business.DTOs.DashboardDTOs;

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
