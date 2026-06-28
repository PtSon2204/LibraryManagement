namespace LibraryManagement.MVC.ViewModels.Loans;

public class BorrowBookResultViewModel
{
    public Guid LoanId { get; set; }

    public Guid LoanDetailId { get; set; }

    public string BookTitle { get; set; } = string.Empty;

    public DateTime DueAt { get; set; }

    public string Status { get; set; } = string.Empty;
}
