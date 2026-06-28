namespace LibraryManagement.MVC.ViewModels.Loans;

public class LoanListItemViewModel
{
    public Guid LoanId { get; set; }

    public Guid LoanDetailId { get; set; }

    public Guid BookId { get; set; }

    public string BookTitle { get; set; } = string.Empty;

    public string Barcode { get; set; } = string.Empty;

    public string BorrowerName { get; set; } = string.Empty;

    public string BorrowerEmail { get; set; } = string.Empty;

    public DateTime BorrowedAt { get; set; }

    public DateTime DueAt { get; set; }

    public DateTime? ReturnedAt { get; set; }

    public string Status { get; set; } = string.Empty;

    public bool IsOverdue { get; set; }

    public List<LoanCopyOptionViewModel> CopyOptions { get; set; } = new();
}
