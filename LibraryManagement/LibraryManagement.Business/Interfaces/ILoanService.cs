using LibraryManagement.Business.DTOs.LoanDTOs;
using LibraryManagement.Data.Common;
using LibraryManagement.Models.Queries;

namespace LibraryManagement.Business.Interfaces;

public interface ILoanService
{
    Task<LoanListPageDto> GetStaffLoansAsync(string? status, string? search, int page, int pageSize);

    Task<LoanListPageDto> GetReaderLoansAsync(Guid readerId, int page, int pageSize);

    Task<BorrowBookResultDto> BorrowBookAsync(Guid readerId, Guid bookId);

    Task ReturnLoanDetailAsync(Guid actorId, string role, Guid loanDetailId);

    Task<PagedResult<LoanHistoryDto>> GetReaderLoanHistoryAsync(Guid readerId, LoanQuery query);

    Task<LoanHistoryDto?> GetLoanDetailByIdAsync(Guid loanId);
}
