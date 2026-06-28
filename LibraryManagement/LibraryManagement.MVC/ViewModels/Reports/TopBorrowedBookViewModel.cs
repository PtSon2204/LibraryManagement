namespace LibraryManagement.MVC.ViewModels.Reports;

public class TopBorrowedBookViewModel
{
    public Guid BookId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? ISBN { get; set; }

    public int BorrowCount { get; set; }
}
