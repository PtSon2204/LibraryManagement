namespace LibraryManagement.MVC.ViewModels.Loans;

public class LoanSearchViewModel
{
    public string? Status { get; set; }

    public string? Search { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
