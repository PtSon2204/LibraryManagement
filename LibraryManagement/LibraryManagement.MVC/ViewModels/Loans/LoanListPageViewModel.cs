namespace LibraryManagement.MVC.ViewModels.Loans;

public class LoanListPageViewModel
{
    public List<LoanListItemViewModel> Loans { get; set; } = new();

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }

    public LoanSearchViewModel Search { get; set; } = new();
}
