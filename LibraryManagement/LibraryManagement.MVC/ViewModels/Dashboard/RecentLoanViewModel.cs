namespace LibraryManagement.MVC.ViewModels.Dashboard;

public class RecentLoanViewModel
{
    public Guid LoanId { get; set; }

    public string BorrowerName { get; set; } = string.Empty;

    public DateTime BorrowedAt { get; set; }

    public DateTime DueAt { get; set; }

    public string Status { get; set; } = string.Empty;
}
