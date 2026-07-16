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
    private readonly IEmailService _emailService;

    public LoanService(IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<LoanListPageDto> GetStaffLoansAsync(string? status, string? search, int page, int pageSize)
    {
        var query = BuildLoanDetailQuery(status, search);
        return await ToLoanListPageAsync(query, page, pageSize);
    }

    public async Task<ReaderLoanSummaryPageDto> GetStaffReaderLoanSummariesAsync(string? search, int page, int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 50);
        var today = DateTime.UtcNow.Date;

        var query = _unitOfWork.Readers.Query()
            .AsNoTracking()
            .Where(r => r.Loans.Any(l => l.LoanDetails.Any(d => d.Status == PendingStatus || d.Status == BorrowedStatus || d.Status == "Overdue")));

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(r => r.Email.Contains(search)
                || (r.Profile != null && (r.Profile.FullName.Contains(search) || (r.Profile.Phone != null && r.Profile.Phone.Contains(search)))));

        var totalCount = await query.CountAsync();
        var readers = await query
            .OrderBy(r => r.Profile != null ? r.Profile.FullName : r.Email)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ReaderLoanSummaryDto
            {
                ReaderId = r.ReaderId,
                ReaderName = r.Profile != null ? r.Profile.FullName : r.Email,
                Email = r.Email,
                Phone = r.Profile != null ? r.Profile.Phone : null,
                ReaderStatus = r.Status,
                PendingCount = r.Loans.SelectMany(l => l.LoanDetails).Count(d => d.Status == PendingStatus),
                BorrowedCount = r.Loans.SelectMany(l => l.LoanDetails).Count(d => d.Status == BorrowedStatus || d.Status == "Overdue"),
                OverdueCount = r.Loans.SelectMany(l => l.LoanDetails).Count(d => d.Status == "Overdue" || (d.Status == BorrowedStatus && d.Loan.DueAt.Date < today)),
                UnpaidFineCount = r.Loans.SelectMany(l => l.LoanDetails).SelectMany(d => d.Fines).Count(f => f.Status == "Unpaid"),
                UnpaidFineAmount = r.Loans.SelectMany(l => l.LoanDetails).SelectMany(d => d.Fines).Where(f => f.Status == "Unpaid").Sum(f => f.Amount)
            })
            .ToListAsync();

        return new ReaderLoanSummaryPageDto
        {
            Readers = readers,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<ReaderLoanWorkspaceDto?> GetStaffReaderLoanWorkspaceAsync(Guid readerId)
    {
        var reader = await _unitOfWork.Readers.Query()
            .AsNoTracking()
            .Include(r => r.Profile)
            .FirstOrDefaultAsync(r => r.ReaderId == readerId);

        if (reader == null)
            return null;

        var today = DateTime.UtcNow.Date;
        var loans = await ToLoanListItemsAsync(BuildLoanDetailQuery(null, null).Where(d => d.Loan.BorrowerReaderId == readerId));

        return new ReaderLoanWorkspaceDto
        {
            ReaderId = reader.ReaderId,
            ReaderName = reader.Profile != null ? reader.Profile.FullName : reader.Email,
            Email = reader.Email,
            Phone = reader.Profile?.Phone,
            ReaderStatus = reader.Status,
            PendingLoans = loans.Where(l => l.Status == PendingStatus).ToList(),
            BorrowedLoans = loans.Where(l => l.Status == BorrowedStatus && !l.IsOverdue).ToList(),
            OverdueLoans = loans.Where(l => l.Status == "Overdue" || (l.Status == BorrowedStatus && l.DueAt.Date < today)).ToList()
        };
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

        // Gửi email xác nhận cho độc giả
        var reader = await _unitOfWork.Readers.GetByIdAsync(detail.Loan.BorrowerReaderId);
        if (reader != null)
        {
            await _emailService.SendEmailAsync(
                reader.Email,
                "[Thư viện] Yêu cầu mượn sách đã được xác nhận",
                $"<p>Xin chào,</p>" +
                $"<p>Yêu cầu mượn sách <strong>{detail.Copy.Book.Title}</strong> của bạn đã được thủ thư xác nhận.</p>" +
                $"<p>📅 Hạn trả: <strong>{detail.Loan.DueAt:dd/MM/yyyy}</strong></p>" +
                $"<p>Vui lòng đến quầy để nhận sách. Trân trọng!</p>"
            );
        }
    }

    public async Task ConfirmLoanDetailsAsync(Guid actorId, Guid readerId, List<ConfirmLoanDetailItemDto> items)
    {
        if (items.Count == 0)
            throw new InvalidOperationException("Vui lòng chọn ít nhất một yêu cầu mượn để xác nhận.");

        var duplicateCopy = items.GroupBy(i => i.CopyId).FirstOrDefault(g => g.Count() > 1);
        if (duplicateCopy != null)
            throw new InvalidOperationException("Một bản sao không thể được chọn cho nhiều sách cùng lúc.");

        var detailIds = items.Select(i => i.LoanDetailId).ToList();
        var details = await _unitOfWork.LoanDetails.Query()
            .Include(d => d.Copy)
                .ThenInclude(c => c.Book)
            .Include(d => d.Loan)
            .Where(d => detailIds.Contains(d.LoanDetailId))
            .ToListAsync();

        if (details.Count != items.Count)
            throw new InvalidOperationException("Một hoặc nhiều yêu cầu mượn không còn tồn tại.");

        if (details.Any(d => d.Loan.BorrowerReaderId != readerId))
            throw new InvalidOperationException("Chỉ có thể xác nhận cùng lúc các yêu cầu của cùng một độc giả.");

        if (details.Any(d => d.Status != PendingStatus))
            throw new InvalidOperationException("Chỉ có thể xác nhận các yêu cầu đang chờ duyệt.");

        var copyIds = items.Select(i => i.CopyId).ToList();
        var selectedCopies = await _unitOfWork.BookCopies.Query()
            .Where(c => copyIds.Contains(c.CopyId))
            .ToDictionaryAsync(c => c.CopyId);

        if (selectedCopies.Count != items.Count)
            throw new InvalidOperationException("Một hoặc nhiều bản sao được chọn không tồn tại.");

        foreach (var item in items)
        {
            var detail = details.Single(d => d.LoanDetailId == item.LoanDetailId);
            var selectedCopy = selectedCopies[item.CopyId];

            if (selectedCopy.BookId != detail.Copy.BookId)
                throw new InvalidOperationException("Bản sao được chọn không thuộc đúng sách trong yêu cầu mượn.");

            if (selectedCopy.CopyId != detail.CopyId && selectedCopy.Status != AvailableStatus)
                throw new InvalidOperationException("Một hoặc nhiều bản sao được chọn hiện không có sẵn.");

            if (selectedCopy.CopyId == detail.CopyId && selectedCopy.Status != PendingStatus)
                throw new InvalidOperationException("Một hoặc nhiều bản sao đang giữ không còn ở trạng thái chờ xác nhận.");
        }

        var now = DateTime.UtcNow;
        foreach (var item in items)
        {
            var detail = details.Single(d => d.LoanDetailId == item.LoanDetailId);
            var selectedCopy = selectedCopies[item.CopyId];

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
        }

        await _unitOfWork.SaveChangesAsync();

        // Gửi email xác nhận hàng loạt cho độc giả
        var batchReader = await _unitOfWork.Readers.GetByIdAsync(details.First().Loan.BorrowerReaderId);
        if (batchReader != null)
        {
            var bookList = string.Join("", details.Select(d =>
                $"<li><strong>{d.Copy.Book.Title}</strong></li>"));
            await _emailService.SendEmailAsync(
                batchReader.Email,
                "[Thư viện] Yêu cầu mượn sách đã được xác nhận",
                $"<p>Xin chào,</p>" +
                $"<p>Các yêu cầu mượn sách sau của bạn đã được thủ thư xác nhận:</p>" +
                $"<ul>{bookList}</ul>" +
                $"<p>📅 Hạn trả: <strong>{details.First().Loan.DueAt:dd/MM/yyyy}</strong></p>" +
                $"<p>Vui lòng đến quầy để nhận sách. Trân trọng!</p>"
            );
        }
    }

    public async Task RejectLoanDetailAsync(Guid actorId, Guid loanDetailId, string? reason)
    {
        var detail = await _unitOfWork.LoanDetails.Query()
            .Include(d => d.Copy)
                .ThenInclude(c => c.Book)
            .Include(d => d.Loan)
            .FirstOrDefaultAsync(d => d.LoanDetailId == loanDetailId);

        if (detail == null)
            throw new InvalidOperationException("Không tìm thấy yêu cầu mượn cần từ chối.");

        if (detail.Status != PendingStatus)
            throw new InvalidOperationException("Chỉ có thể từ chối yêu cầu đang chờ duyệt.");

        var now = DateTime.UtcNow;

        // Trả bản sao về trạng thái Available
        detail.Copy.Status = AvailableStatus;
        detail.Status = "Rejected";
        detail.Loan.Status = "Rejected";
        detail.Loan.UpdatedAt = now;
        detail.Loan.ProcessedByAccountId = actorId;

        _unitOfWork.LoanDetails.Update(detail);
        _unitOfWork.BookCopies.Update(detail.Copy);
        _unitOfWork.Loans.Update(detail.Loan);
        await _unitOfWork.SaveChangesAsync();

        // Gửi email thông báo từ chối cho độc giả
        var reader = await _unitOfWork.Readers.GetByIdAsync(detail.Loan.BorrowerReaderId);
        if (reader != null)
        {
            var reasonText = string.IsNullOrWhiteSpace(reason)
                ? "Thủ thư không nêu lý do cụ thể."
                : reason;

            await _emailService.SendEmailAsync(
                reader.Email,
                "[Thư viện] Yêu cầu mượn sách bị từ chối",
                $"<p>Xin chào,</p>" +
                $"<p>Rất tiếc, yêu cầu mượn sách <strong>{detail.Copy.Book.Title}</strong> của bạn đã bị từ chối.</p>" +
                $"<p>📌 Lý do: {reasonText}</p>" +
                $"<p>Bạn có thể liên hệ thủ thư để biết thêm thông tin. Trân trọng!</p>"
            );
        }
    }

    public async Task ReturnLoanDetailAsync(Guid actorId, string role, Guid loanDetailId)
    {
        var detail = await _unitOfWork.LoanDetails.Query()
            .Include(d => d.Copy)
                .ThenInclude(c => c.Book)
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

        // Gửi email thông báo trả sách thành công
        var reader = await _unitOfWork.Readers.GetByIdAsync(detail.Loan.BorrowerReaderId);
        if (reader != null)
        {
            await _emailService.SendEmailAsync(
                reader.Email,
                "[Thư viện] Trả sách thành công",
                $"<p>Xin chào,</p>" +
                $"<p>Bạn đã trả thành công cuốn sách <strong>{detail.Copy.Book.Title}</strong>.</p>" +
                $"<p>Cảm ơn bạn đã sử dụng dịch vụ của thư viện. Trân trọng!</p>"
            );
        }
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

    private async Task<LoanListPageDto> ToLoanListPageAsync(IQueryable<LoanDetail> query, int page, int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var totalCount = await query.CountAsync();
        var loans = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new LoanListPageDto
        {
            Loans = await ToLoanListItemsAsync(loans),
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private async Task<List<LoanListItemDto>> ToLoanListItemsAsync(IQueryable<LoanDetail> query)
    {
        return await ToLoanListItemsAsync(await query.ToListAsync());
    }

    private async Task<List<LoanListItemDto>> ToLoanListItemsAsync(List<LoanDetail> details)
    {
        var today = DateTime.UtcNow.Date;
        var pendingDetails = details.Where(d => d.Status == PendingStatus).ToList();
        var pendingBookIds = pendingDetails.Select(d => d.Copy.BookId).Distinct().ToList();
        var reservedCopyIds = pendingDetails.Select(d => d.CopyId).ToList();
        var copyOptions = pendingBookIds.Count == 0
            ? new Dictionary<Guid, List<LoanCopyOptionDto>>()
            : (await _unitOfWork.BookCopies.Query()
                .AsNoTracking()
                .Include(c => c.ShelfSlot)
                .Where(c => pendingBookIds.Contains(c.BookId) && (c.Status == AvailableStatus || reservedCopyIds.Contains(c.CopyId)))
                .OrderBy(c => c.ShelfSlot != null ? c.ShelfSlot.SlotCode : string.Empty)
                .ThenBy(c => c.Barcode)
                .Select(c => new
                {
                    c.BookId,
                    Option = new LoanCopyOptionDto
                    {
                        CopyId = c.CopyId,
                        Barcode = c.Barcode,
                        SlotLocation = c.ShelfSlot != null ? c.ShelfSlot.SlotCode : null,
                        Status = c.Status
                    }
                })
                .ToListAsync())
                .GroupBy(c => c.BookId)
                .ToDictionary(g => g.Key, g => g.Select(c => c.Option).ToList());

        return details.Select(d => new LoanListItemDto
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
                ? copyOptions.GetValueOrDefault(d.Copy.BookId, new List<LoanCopyOptionDto>())
                : new List<LoanCopyOptionDto>()
        }).ToList();
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
