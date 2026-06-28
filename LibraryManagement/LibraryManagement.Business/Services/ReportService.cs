using LibraryManagement.Business.DTOs.ReportDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Business.Services;

public class ReportService : IReportService
{
    private const string BorrowedStatus = "Borrowed";
    private const string AvailableStatus = "Available";
    private const string UnpaidStatus = "Unpaid";

    private readonly IUnitOfWork _unitOfWork;

    public ReportService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<LibraryReportDto> GetLibraryReportAsync(ReportQueryDto query)
    {
        var today = DateTime.UtcNow.Date;
        var fromDate = (query.FromDate ?? today.AddDays(-30)).Date;
        var toDate = (query.ToDate ?? today).Date;
        if (fromDate > toDate)
            (fromDate, toDate) = (toDate, fromDate);

        var nextToDate = toDate.AddDays(1);
        var loansInRange = _unitOfWork.Loans.Query()
            .AsNoTracking()
            .Where(l => l.BorrowedAt >= fromDate && l.BorrowedAt < nextToDate);

        var returnedInRange = _unitOfWork.LoanDetails.Query()
            .AsNoTracking()
            .Where(d => d.ReturnedAt >= fromDate && d.ReturnedAt < nextToDate);

        var topBorrowedBooks = await _unitOfWork.LoanDetails.Query()
            .AsNoTracking()
            .Where(d => d.Loan.BorrowedAt >= fromDate && d.Loan.BorrowedAt < nextToDate)
            .GroupBy(d => new { d.Copy.BookId, d.Copy.Book.Title, d.Copy.Book.ISBN })
            .Select(g => new TopBorrowedBookDto
            {
                BookId = g.Key.BookId,
                Title = g.Key.Title,
                ISBN = g.Key.ISBN,
                BorrowCount = g.Count()
            })
            .OrderByDescending(b => b.BorrowCount)
            .ThenBy(b => b.Title)
            .Take(10)
            .ToListAsync();

        var loansByStatus = await loansInRange
            .GroupBy(l => l.Status)
            .Select(g => new LoanStatusReportDto
            {
                Status = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync();

        return new LibraryReportDto
        {
            FromDate = fromDate,
            ToDate = toDate,
            TotalBooks = await _unitOfWork.Books.Query().AsNoTracking().CountAsync(),
            TotalCopies = await _unitOfWork.BookCopies.Query().AsNoTracking().CountAsync(),
            AvailableCopies = await _unitOfWork.BookCopies.Query().AsNoTracking().CountAsync(c => c.Status == AvailableStatus),
            TotalReaders = await _unitOfWork.Readers.Query().AsNoTracking().CountAsync(),
            NewReaders = await _unitOfWork.Readers.Query().AsNoTracking().CountAsync(r => r.CreatedAt >= fromDate && r.CreatedAt < nextToDate),
            LoansCreated = await loansInRange.CountAsync(),
            BooksReturned = await returnedInRange.CountAsync(),
            ActiveLoans = await _unitOfWork.Loans.Query().AsNoTracking().CountAsync(l => l.Status == BorrowedStatus),
            OverdueLoans = await _unitOfWork.Loans.Query().AsNoTracking().CountAsync(l => l.Status == BorrowedStatus && l.DueAt.Date < today),
            UnpaidFineAmount = await _unitOfWork.Fines.Query().AsNoTracking().Where(f => f.Status == UnpaidStatus).SumAsync(f => (decimal?)f.Amount) ?? 0,
            LoansByStatus = loansByStatus,
            TopBorrowedBooks = topBorrowedBooks
        };
    }
}
