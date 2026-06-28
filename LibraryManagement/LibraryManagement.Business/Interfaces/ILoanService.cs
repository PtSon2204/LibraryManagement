using LibraryManagement.Business.DTOs.LoanDTOs;

namespace LibraryManagement.Business.Interfaces;

public interface ILoanService
{
    Task<LoanListPageDto> GetStaffLoansAsync(string? status, string? search, int page, int pageSize);

    Task<LoanListPageDto> GetReaderLoansAsync(Guid readerId, int page, int pageSize);

    Task<BorrowBookResultDto> BorrowBookAsync(Guid readerId, Guid bookId);

    Task ReturnLoanDetailAsync(Guid actorId, string role, Guid loanDetailId);
}
