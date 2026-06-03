namespace LibraryManagement.MVC.ViewModels.Dashboard;

public class StaffDashboardViewModel
{
    public int TotalBooks { get; set; }

    public int TotalUsers { get; set; }

    public int TotalCopies { get; set; }

    public int AvailableCopies { get; set; }

    public int ActiveLoans { get; set; }

    public int PendingReservations { get; set; }

    public int OverdueLoans { get; set; }

    public int UnpaidFines { get; set; }

    public List<RecentLoanViewModel> RecentLoans { get; set; } = new();

    public List<RecentReservationViewModel> RecentReservations { get; set; } = new();
}
