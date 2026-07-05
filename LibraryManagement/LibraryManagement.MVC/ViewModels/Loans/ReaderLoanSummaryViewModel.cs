namespace LibraryManagement.MVC.ViewModels.Loans;

public class ReaderLoanSummaryViewModel
{
    public Guid ReaderId { get; set; }

    public string ReaderName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string ReaderStatus { get; set; } = string.Empty;

    public int PendingCount { get; set; }

    public int BorrowedCount { get; set; }

    public int OverdueCount { get; set; }

    public int UnpaidFineCount { get; set; }

    public decimal UnpaidFineAmount { get; set; }
}
