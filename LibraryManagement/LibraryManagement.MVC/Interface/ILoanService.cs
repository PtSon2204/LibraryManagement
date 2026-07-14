using LibraryManagement.MVC.ViewModels.Loans;

namespace LibraryManagement.MVC.Interface;

public interface ILoanService
{
    Task<LoanListPageViewModel?> GetStaffLoansAsync(LoanSearchViewModel search);

    Task<ReaderLoanSummaryPageViewModel?> GetStaffReaderLoanSummariesAsync(LoanSearchViewModel search);

    Task<ReaderLoanWorkspaceViewModel?> GetStaffReaderLoanWorkspaceAsync(Guid readerId);

    Task<LoanListPageViewModel?> GetMyLoansAsync(LoanSearchViewModel search);

    Task<BorrowBookResultViewModel?> BorrowBookAsync(Guid bookId);

    Task<string?> ConfirmBorrowRequestAsync(Guid loanDetailId, Guid copyId);

    Task<string?> ConfirmBorrowRequestsAsync(Guid readerId, List<ConfirmLoanDetailItemViewModel> items);

    Task<string?> RejectBorrowRequestAsync(Guid loanDetailId, string? reason);

    Task<string?> ReturnBookAsync(Guid loanDetailId);

    Task<LoanListViewModel?> GetMyLoanHistoryAsync(string? searchTerm, string? status, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize);

    Task<LoanViewModel?> GetLoanDetailAsync(Guid loanId);
}
