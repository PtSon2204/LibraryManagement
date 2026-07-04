namespace LibraryManagement.Business.DTOs.LoanDTOs;

public class ReaderLoanSummaryPageDto
{
    public List<ReaderLoanSummaryDto> Readers { get; set; } = new();

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}
