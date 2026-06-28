using LibraryManagement.Business.DTOs.LoanDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.Common;
using LibraryManagement.Data.UnitOfWorks;
using LibraryManagement.Models.Models;
using LibraryManagement.Models.Queries;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Business.Services;

public class LoanService : ILoanService
{
    private const string BorrowedStatus = "Borrowed";
    private const string PendingStatus = "Pending";
    private const string ReturnedStatus = "Returned";
    private const string AvailableStatus = "Available";

    private readonly IUnitOfWork _unitOfWork;

    public LoanService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<LoanListPageDto> GetStaffLoansAsync(string? status, string? search, int page, int pageSize)
    {
        var query = BuildLoanDetailQuery(status, search);
        return await ToLoanListPageAsync(query, page, pageSize);
    }

    public async Task<LoanListPageDto> GetReaderLoansAsync(Guid readerId, int page, int pageSize)
    {
        var query = BuildLoanDetailQuery(null, null)
            .Where(d => d.Loan.BorrowerReaderId == readerId);

        return await ToLoanListPageAsync(query, page, pageSize);
    }

    public async Task<BorrowBookResultDto> BorrowBookAsync(Guid readerId, Guid bookId)
    {
        var readerExists = await _unitOfWork.Readers.Query()
            .AnyAsync(r => r.ReaderId == readerId && r.Status == "Active");
        if (!readerExists)
            throw new InvalidOperationException("Tài khoản độc giả không hợp lệ hoặc đã bị khóa.");

        var hasActiveRequestForBook = await _unitOfWork.LoanDetails.Query()
            .AnyAsync(d => d.Loan.BorrowerReaderId == readerId
                && d.Copy.BookId == bookId
                && (d.Status == PendingStatus || d.Status == BorrowedStatus));
        if (hasActiveRequestForBook)
            throw new InvalidOperationException("Bạn đã có yêu cầu hoặc phiếu mượn đang hoạt động cho sách này.");

        var copy = await _unitOfWork.BookCopies.Query()
            .Include(c => c.Book)
            .Where(c => c.BookId == bookId && c.Status == AvailableStatus)
            .OrderBy(c => c.Barcode)
            .FirstOrDefaultAsync();

        if (copy == null)
            throw new InvalidOperationException("Sách hiện không còn bản sao có sẵn để mượn.");

        var now = DateTime.UtcNow;
        var loan = new Loan
        {
            LoanId = Guid.NewGuid(),
            BorrowerReaderId = readerId,
            BorrowedAt = now,
            DueAt = now.Date,
            Status = PendingStatus,
            CreatedAt = now
        };

        var detail = new LoanDetail
        {
            LoanDetailId = Guid.NewGuid(),
            LoanId = loan.LoanId,
            CopyId = copy.CopyId,
            Status = PendingStatus
        };

        copy.Status = PendingStatus;

        await _unitOfWork.Loans.AddAsync(loan);
        await _unitOfWork.LoanDetails.AddAsync(detail);
        _unitOfWork.BookCopies.Update(copy);
        await _unitOfWork.SaveChangesAsync();

        return new BorrowBookResultDto
        {
            LoanId = loan.LoanId,
            LoanDetailId = detail.LoanDetailId,
            BookTitle = copy.Book.Title,
            DueAt = loan.DueAt,
            Status = loan.Status
        };
    }

    public async Task ConfirmLoanDetailAsync(Guid actorId, Guid loanDetailId, Guid copyId)
    {
        var detail = await _unitOfWork.LoanDetails.Query()
            .Include(d => d.Copy)
                .ThenInclude(c => c.Book)
            .Include(d => d.Loan)
            .FirstOrDefaultAsync(d => d.LoanDetailId == loanDetailId);

        if (detail == null)
            throw new InvalidOperationException("Không tìm thấy yêu cầu mượn cần xác nhận.");

        if (detail.Status != PendingStatus)
            throw new InvalidOperationException("Chỉ có thể xác nhận yêu cầu đang chờ duyệt.");

        var selectedCopy = await _unitOfWork.BookCopies.Query()
            .FirstOrDefaultAsync(c => c.CopyId == copyId && c.BookId == detail.Copy.BookId);

        if (selectedCopy == null)
            throw new InvalidOperationException("Bản sao được chọn không thuộc sách trong yêu cầu mượn.");

        if (selectedCopy.CopyId != detail.CopyId && selectedCopy.Status != AvailableStatus)
            throw new InvalidOperationException("Bản sao được chọn hiện không có sẵn để cho mượn.");

        if (selectedCopy.CopyId == detail.CopyId && selectedCopy.Status != PendingStatus)
            throw new InvalidOperationException("Bản sao đang giữ cho yêu cầu không còn ở trạng thái chờ xác nhận.");

        var now = DateTime.UtcNow;
        if (selectedCopy.CopyId != detail.CopyId)
        {
            detail.Copy.Status = AvailableStatus;
            _unitOfWork.BookCopies.Update(detail.Copy);

            detail.CopyId = selectedCopy.CopyId;
            detail.Copy = selectedCopy;
        }

        detail.Status = BorrowedStatus;
        selectedCopy.Status = BorrowedStatus;
        detail.Loan.Status = BorrowedStatus;
        detail.Loan.BorrowedAt = now;
        detail.Loan.DueAt = now.Date.AddDays(14);
        detail.Loan.ProcessedByAccountId = actorId;
        detail.Loan.UpdatedAt = now;

        _unitOfWork.LoanDetails.Update(detail);
        _unitOfWork.BookCopies.Update(selectedCopy);
        _unitOfWork.Loans.Update(detail.Loan);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ReturnLoanDetailAsync(Guid actorId, string role, Guid loanDetailId)
    {
        var detail = await _unitOfWork.LoanDetails.Query()
            .Include(d => d.Copy)
            .Include(d => d.Loan)
            .FirstOrDefaultAsync(d => d.LoanDetailId == loanDetailId);

        if (detail == null)
            throw new InvalidOperationException("Không tìm thấy phiếu mượn cần trả.");

        if (detail.Status == ReturnedStatus)
            throw new InvalidOperationException("Sách này đã được trả trước đó.");

        if (detail.Status == PendingStatus)
            throw new InvalidOperationException("Yêu cầu mượn chưa được xác nhận nên không thể trả sách.");

        if (role is not ("Librarian" or "Admin"))
            throw new UnauthorizedAccessException("Chỉ thủ thư hoặc quản trị viên mới có thể ghi nhận trả sách.");

        var now = DateTime.UtcNow;
        detail.Status = ReturnedStatus;
        detail.ReturnedAt = now;
        detail.Copy.Status = AvailableStatus;
        detail.Loan.Status = ReturnedStatus;
        detail.Loan.UpdatedAt = now;

        if (role is "Librarian" or "Admin")
            detail.Loan.ProcessedByAccountId = actorId;

        _unitOfWork.LoanDetails.Update(detail);
        _unitOfWork.BookCopies.Update(detail.Copy);
        _unitOfWork.Loans.Update(detail.Loan);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PagedResult<LoanHistoryDto>> GetReaderLoanHistoryAsync(Guid readerId, LoanQuery query)
    {
        query.PageNumber = Math.Max(query.PageNumber, 1);

        var dbQuery = BuildLoanHistoryQuery(readerId, query);
        var totalCount = await dbQuery.CountAsync();
        var items = await ProjectLoanHistory(dbQuery
                .OrderByDescending(l => l.CreatedAt)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize))
            .ToListAsync();

        return new PagedResult<LoanHistoryDto>
        {
            Data = items,
            TotalRecords = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
        };
    }

    public async Task<LoanHistoryDto?> GetLoanDetailByIdAsync(Guid loanId)
    {
        return await ProjectLoanHistory(_unitOfWork.Loans.Query()
                .AsNoTracking()
                .Where(l => l.LoanId == loanId))
            .FirstOrDefaultAsync();
    }

    private IQueryable<LoanDetail> BuildLoanDetailQuery(string? status, string? search)
    {
        var query = _unitOfWork.LoanDetails.Query()
            .AsNoTracking()
            .Include(d => d.Copy)
                .ThenInclude(c => c.Book)
            .Include(d => d.Loan)
                .ThenInclude(l => l.BorrowerReader)
                    .ThenInclude(r => r.Profile)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(d => d.Status == status);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(d => d.Copy.Book.Title.Contains(search)
                || d.Copy.Barcode.Contains(search)
                || d.Loan.BorrowerReader.Email.Contains(search)
                || (d.Loan.BorrowerReader.Profile != null && d.Loan.BorrowerReader.Profile.FullName.Contains(search)));

        return query.OrderByDescending(d => d.Loan.BorrowedAt);
    }

    private IQueryable<Loan> BuildLoanHistoryQuery(Guid readerId, LoanQuery query)
    {
        var dbQuery = _unitOfWork.Loans.Query()
            .AsNoTracking()
            .Where(l => l.BorrowerReaderId == readerId);

        if (!string.IsNullOrWhiteSpace(query.Status))
            dbQuery = dbQuery.Where(l => l.Status == query.Status);

        if (query.FromDate.HasValue)
            dbQuery = dbQuery.Where(l => l.BorrowedAt >= query.FromDate.Value);

        if (query.ToDate.HasValue)
            dbQuery = dbQuery.Where(l => l.BorrowedAt <= query.ToDate.Value);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            dbQuery = dbQuery.Where(l => l.LoanDetails.Any(ld =>
                ld.Copy.Book.Title.ToLower().Contains(term) ||
                ld.Copy.Barcode.ToLower().Contains(term)));
        }

        return dbQuery;
    }

    private static async Task<LoanListPageDto> ToLoanListPageAsync(IQueryable<LoanDetail> query, int page, int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var totalCount = await query.CountAsync();
        var today = DateTime.UtcNow.Date;
        var loans = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new LoanListItemDto
            {
                LoanId = d.LoanId,
                LoanDetailId = d.LoanDetailId,
                BookId = d.Copy.BookId,
                BookTitle = d.Copy.Book.Title,
                Barcode = d.Copy.Barcode,
                BorrowerName = d.Loan.BorrowerReader.Profile != null ? d.Loan.BorrowerReader.Profile.FullName : d.Loan.BorrowerReader.Email,
                BorrowerEmail = d.Loan.BorrowerReader.Email,
                BorrowedAt = d.Loan.BorrowedAt,
                DueAt = d.Loan.DueAt,
                ReturnedAt = d.ReturnedAt,
                Status = d.Status,
                IsOverdue = d.Status == BorrowedStatus && d.Loan.DueAt.Date < today,
                CopyOptions = d.Status == PendingStatus
                    ? d.Copy.Book.BookCopies
                        .Where(c => c.Status == AvailableStatus || c.CopyId == d.CopyId)
                        .OrderBy(c => c.Location)
                        .ThenBy(c => c.Barcode)
                        .Select(c => new LoanCopyOptionDto
                        {
                            CopyId = c.CopyId,
                            Barcode = c.Barcode,
                            Location = c.Location,
                            Status = c.Status
                        })
                        .ToList()
                    : new List<LoanCopyOptionDto>()
            })
            .ToListAsync();

        return new LoanListPageDto
        {
            Loans = loans,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private static IQueryable<LoanHistoryDto> ProjectLoanHistory(IQueryable<Loan> query)
    {
        return query.Select(l => new LoanHistoryDto
        {
            LoanId = l.LoanId,
            BorrowedAt = l.BorrowedAt,
            DueAt = l.DueAt,
            Status = l.Status,
            CreatedAt = l.CreatedAt,
            ProcessedByLibrarian = l.ProcessedByAccount != null
                ? (l.ProcessedByAccount.Profile != null ? l.ProcessedByAccount.Profile.FullName : l.ProcessedByAccount.Email)
                : null,
            LoanDetails = l.LoanDetails.Select(ld => new LoanDetailHistoryDto
            {
                LoanDetailId = ld.LoanDetailId,
                CopyId = ld.CopyId,
                BookTitle = ld.Copy.Book.Title,
                Barcode = ld.Copy.Barcode,
                CoverImageUrl = ld.Copy.Book.CoverImageUrl,
                ReturnedAt = ld.ReturnedAt,
                Status = ld.Status
            }).ToList()
        });
    }
}
