namespace LibraryManagement.MVC.Interface
{
    public interface ILoanService
    {
        Task<ViewModels.Loans.LoanListViewModel?> GetMyLoanHistoryAsync(string? searchTerm, string? status, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize);
        Task<ViewModels.Loans.LoanViewModel?> GetLoanDetailAsync(Guid loanId);
    }
}
