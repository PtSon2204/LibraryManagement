namespace LibraryManagement.MVC.ViewModels.Books;

public class BookCopyViewModel
{
    public Guid CopyId { get; set; }

    public string Barcode { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? Location { get; set; }

    public DateOnly AddedDate { get; set; }
}
