namespace LibraryManagement.MVC.ViewModels.Fines;

public class PaymentQrViewModel
{
    public string QrUrl { get; set; } = string.Empty;
    public string TransferContent { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
