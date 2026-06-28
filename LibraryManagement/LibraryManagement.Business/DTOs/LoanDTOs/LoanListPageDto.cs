namespace LibraryManagement.Business.DTOs.LoanDTOs;

public class LoanListPageDto
{
    public List<LoanListItemDto> Loans { get; set; } = new();

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}
