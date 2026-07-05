namespace LibraryManagement.MVC.ViewModels.Loans;

public class LoanCopyOptionViewModel
{
    public Guid CopyId { get; set; }

    public string Barcode { get; set; } = string.Empty;

    public string? SlotLocation { get; set; }

    public string Status { get; set; } = string.Empty;
}
