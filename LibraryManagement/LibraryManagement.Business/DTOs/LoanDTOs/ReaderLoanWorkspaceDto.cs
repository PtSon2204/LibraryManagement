namespace LibraryManagement.Business.DTOs.LoanDTOs;

public class ReaderLoanWorkspaceDto
{
    public Guid ReaderId { get; set; }

    public string ReaderName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    public string ReaderStatus { get; set; } = string.Empty;

    public List<LoanListItemDto> PendingLoans { get; set; } = new();

    public List<LoanListItemDto> BorrowedLoans { get; set; } = new();

    public List<LoanListItemDto> OverdueLoans { get; set; } = new();
}
