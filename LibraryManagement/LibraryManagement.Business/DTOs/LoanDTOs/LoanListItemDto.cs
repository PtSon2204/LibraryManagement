namespace LibraryManagement.Business.DTOs.LoanDTOs;

public class LoanListItemDto
{
    public Guid LoanId { get; set; }

    public Guid LoanDetailId { get; set; }

    public Guid BookId { get; set; }

    public string BookTitle { get; set; } = string.Empty;

    public string Barcode { get; set; } = string.Empty;

    public decimal ReplacementPrice { get; set; }

    public string BorrowerName { get; set; } = string.Empty;

    public string BorrowerEmail { get; set; } = string.Empty;

    public DateTime BorrowedAt { get; set; }

    public DateTime DueAt { get; set; }

    public DateTime? ReturnedAt { get; set; }

    public string Status { get; set; } = string.Empty;

    public bool IsOverdue { get; set; }

    public List<LoanCopyOptionDto> CopyOptions { get; set; } = new();
}
