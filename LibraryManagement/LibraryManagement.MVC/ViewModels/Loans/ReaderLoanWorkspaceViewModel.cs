namespace LibraryManagement.MVC.ViewModels.Loans;

public class ReaderLoanWorkspaceViewModel
{
    public Guid ReaderId { get; set; }

    public string ReaderName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string ReaderStatus { get; set; } = string.Empty;

    public List<LoanListItemViewModel> PendingLoans { get; set; } = new();

    public List<LoanListItemViewModel> BorrowedLoans { get; set; } = new();

    public List<LoanListItemViewModel> OverdueLoans { get; set; } = new();
}
