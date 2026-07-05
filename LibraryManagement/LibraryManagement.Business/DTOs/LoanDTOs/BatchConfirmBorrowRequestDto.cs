namespace LibraryManagement.Business.DTOs.LoanDTOs;

public class BatchConfirmBorrowRequestDto
{
    public List<ConfirmLoanDetailItemDto> Items { get; set; } = new();
}
