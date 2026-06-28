namespace LibraryManagement.Business.DTOs.LoanDTOs;

public class BorrowBookResultDto
{
    public Guid LoanId { get; set; }

    public Guid LoanDetailId { get; set; }

    public string BookTitle { get; set; } = string.Empty;

    public DateTime DueAt { get; set; }

    public string Status { get; set; } = string.Empty;
}
