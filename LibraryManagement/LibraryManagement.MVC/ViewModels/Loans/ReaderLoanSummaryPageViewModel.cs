namespace LibraryManagement.MVC.ViewModels.Loans;

public class ReaderLoanSummaryPageViewModel
{
    public List<ReaderLoanSummaryViewModel> Readers { get; set; } = new();

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }

    public LoanSearchViewModel Search { get; set; } = new();
}
