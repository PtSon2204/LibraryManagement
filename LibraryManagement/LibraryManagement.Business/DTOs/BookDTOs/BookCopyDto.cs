namespace LibraryManagement.Business.DTOs.BookDTOs;

public class BookCopyDto
{
    public Guid CopyId { get; set; }

    public string Barcode { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? SlotLocation { get; set; }

    public DateOnly AddedDate { get; set; }
}
