namespace LibraryManagement.Business.DTOs.FineDTOs;

public class FineListItemDto
{
    public Guid FineId { get; set; }
    public Guid LoanDetailId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string ReaderName { get; set; } = string.Empty;
    public string ReaderEmail { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PaymentMethod { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}

public class FineListPageDto
{
    public List<FineListItemDto> Fines { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}
