namespace LibraryManagement.Business.DTOs.LoanDTOs;

public class LoanCopyOptionDto
{
    public Guid CopyId { get; set; }

    public string Barcode { get; set; } = string.Empty;

    public string? SlotLocation { get; set; }

    public string Status { get; set; } = string.Empty;
}
