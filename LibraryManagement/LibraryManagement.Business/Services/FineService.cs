using LibraryManagement.Business.DTOs.FineDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.UnitOfWorks;
using LibraryManagement.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Business.Services;

public class FineService : IFineService
{
    private static readonly Guid LegacyLostBookTemplateId = Guid.Parse("A1000001-0000-0000-0000-000000000004");

    private const string ReturnedStatus = "Returned";
    private const string AvailableStatus = "Available";
    private const string BorrowedStatus = "Borrowed";
    private const string LostStatus = "Lost";
    private const string UnpaidStatus = "Unpaid";
    private const string PaidStatus = "Paid";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public FineService(IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    // ─── Fine Templates ──────────────────────────────────────────────────────

    public async Task<List<FineTemplateDto>> GetActiveTemplatesAsync()
    {
        return await _unitOfWork.FineTemplates.Query()
            .AsNoTracking()
            .Where(t => t.IsActive && t.FineTemplateId != LegacyLostBookTemplateId)
            .OrderBy(t => t.FineType == "PerDay" ? 0 : 1)
            .ThenBy(t => t.Name)
            .Select(t => new FineTemplateDto
            {
                FineTemplateId = t.FineTemplateId,
                Name = t.Name,
                Amount = t.Amount,
                FineType = t.FineType,
                IsActive = t.IsActive
            })
            .ToListAsync();
    }

    public async Task<List<FineTemplateDto>> GetAllTemplatesAsync()
    {
        return await _unitOfWork.FineTemplates.Query()
            .AsNoTracking()
            .OrderBy(t => t.FineType == "PerDay" ? 0 : 1)
            .ThenBy(t => t.Name)
            .Select(t => new FineTemplateDto
            {
                FineTemplateId = t.FineTemplateId,
                Name = t.Name,
                Amount = t.Amount,
                FineType = t.FineType,
                IsActive = t.IsActive
            })
            .ToListAsync();
    }

    public async Task CreateTemplateAsync(string name, decimal amount, string fineType)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("Tên khoản phạt không được để trống.");
        if (amount < 0) throw new InvalidOperationException("Số tiền phạt không hợp lệ.");

        var template = new FineTemplate
        {
            FineTemplateId = Guid.NewGuid(),
            Name = name.Trim(),
            Amount = amount,
            FineType = fineType,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.FineTemplates.AddAsync(template);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateTemplateAsync(Guid id, string name, decimal amount, string fineType, bool isActive)
    {
        var template = await _unitOfWork.FineTemplates.Query()
            .FirstOrDefaultAsync(t => t.FineTemplateId == id);
        if (template == null) throw new InvalidOperationException("Không tìm thấy loại khoản phạt.");
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("Tên khoản phạt không được để trống.");
        if (amount < 0) throw new InvalidOperationException("Số tiền phạt không hợp lệ.");

        template.Name = name.Trim();
        template.Amount = amount;
        template.FineType = fineType;
        template.IsActive = isActive;

        _unitOfWork.FineTemplates.Update(template);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteTemplateAsync(Guid id)
    {
        var template = await _unitOfWork.FineTemplates.Query()
            .FirstOrDefaultAsync(t => t.FineTemplateId == id);
        if (template == null) throw new InvalidOperationException("Không tìm thấy loại khoản phạt.");

        _unitOfWork.FineTemplates.Delete(template);
        await _unitOfWork.SaveChangesAsync();
    }

    // ─── Fine Operations ─────────────────────────────────────────────────────

    public async Task CreateFinesAndReturnAsync(Guid actorId, CreateFineRequest request)
    {
        if (request.SelectedItems.Count == 0 && !request.IsLostBook)
            throw new InvalidOperationException("Vui lòng chọn ít nhất một khoản phạt.");

        var detail = await _unitOfWork.LoanDetails.Query()
            .Include(d => d.Copy)
                .ThenInclude(c => c.Book)
            .Include(d => d.Loan)
                .ThenInclude(l => l.BorrowerReader)
            .FirstOrDefaultAsync(d => d.LoanDetailId == request.LoanDetailId);

        if (detail == null)
            throw new InvalidOperationException("Không tìm thấy phiếu mượn.");
        if (detail.Status == ReturnedStatus)
            throw new InvalidOperationException("Sách này đã được trả trước đó.");
        if (detail.Status != BorrowedStatus && detail.Status != "Overdue")
            throw new InvalidOperationException("Sách chưa được xác nhận mượn, không thể tạo phạt.");

        var now = DateTime.UtcNow;
        var overdueDays = Math.Max(0, (now.Date - detail.Loan.DueAt.Date).Days);
        var selectedTemplateIds = request.SelectedItems.Select(i => i.FineTemplateId).ToList();
        if (selectedTemplateIds.Count != selectedTemplateIds.Distinct().Count())
            throw new InvalidOperationException("Danh sách khoản phạt không hợp lệ.");

        var templates = await _unitOfWork.FineTemplates.Query()
            .AsNoTracking()
            .Where(t => selectedTemplateIds.Contains(t.FineTemplateId)
                && t.IsActive
                && t.FineTemplateId != LegacyLostBookTemplateId)
            .ToDictionaryAsync(t => t.FineTemplateId);

        if (templates.Count != selectedTemplateIds.Count)
            throw new InvalidOperationException("Có loại khoản phạt không tồn tại hoặc đã ngừng áp dụng.");

        var fineItems = request.SelectedItems.Select(item =>
        {
            var template = templates[item.FineTemplateId];
            var amount = template.FineType == "PerDay"
                ? template.Amount * overdueDays
                : template.Amount;
            return (Reason: template.Name, Amount: amount);
        }).ToList();

        if (request.IsLostBook)
        {
            if (detail.Copy.ReplacementPrice <= 0)
                throw new InvalidOperationException("Bản sao chưa có giá thay thế, không thể tạo khoản phạt mất sách.");

            fineItems.Add((Reason: "Mất sách", Amount: detail.Copy.ReplacementPrice));
        }

        var totalAmount = fineItems.Sum(i => i.Amount);
        if (totalAmount <= 0)
            throw new InvalidOperationException("Tổng tiền phạt phải lớn hơn 0.");

        // Tạo Payment
        var payment = new Payment
        {
            PaymentId = Guid.NewGuid(),
            ReaderId = detail.Loan.BorrowerReaderId,
            ProcessedByAccountId = actorId,
            TotalAmount = totalAmount,
            Method = request.PaymentMethod,
            Note = request.Note,
            PaidAt = now
        };
        await _unitOfWork.Payments.AddAsync(payment);

        // Tạo Fine records
        foreach (var item in fineItems)
        {
            var fine = new Fine
            {
                FineId = Guid.NewGuid(),
                LoanDetailId = request.LoanDetailId,
                PaymentId = payment.PaymentId,
                Amount = item.Amount,
                Reason = item.Reason,
                Status = PaidStatus,
                CreatedAt = now,
                PaidAt = now
            };
            await _unitOfWork.Fines.AddAsync(fine);
        }

        // Ghi nhận trả sách
        detail.Status = request.IsLostBook ? LostStatus : ReturnedStatus;
        detail.ReturnedAt = now;
        detail.Copy.Status = request.IsLostBook ? LostStatus : AvailableStatus;
        detail.Loan.Status = request.IsLostBook ? LostStatus : ReturnedStatus;
        detail.Loan.UpdatedAt = now;
        detail.Loan.ProcessedByAccountId = actorId;

        _unitOfWork.LoanDetails.Update(detail);
        _unitOfWork.BookCopies.Update(detail.Copy);
        _unitOfWork.Loans.Update(detail.Loan);

        await _unitOfWork.SaveChangesAsync();

        // Gửi email thông báo trả sách thành công (sau khi nộp phạt)
        if (detail.Loan.BorrowerReader != null)
        {
            await _emailService.SendEmailAsync(
                detail.Loan.BorrowerReader.Email,
                "[Thư viện] Trả sách thành công (có khoản phạt)",
                $"<p>Xin chào,</p>" +
                $"<p>Bạn đã trả thành công cuốn sách <strong>{detail.Copy.Book.Title}</strong>.</p>" +
                $"<p>Các khoản phạt áp dụng đã được thanh toán đầy đủ.</p>" +
                $"<p>Cảm ơn bạn đã sử dụng dịch vụ của thư viện. Trân trọng!</p>"
            );
        }
    }

    public async Task<FineListPageDto> GetFinesAsync(string? search, string? status, int page, int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var query = _unitOfWork.Fines.Query()
            .AsNoTracking()
            .Include(f => f.LoanDetail)
                .ThenInclude(d => d.Copy)
                    .ThenInclude(c => c.Book)
            .Include(f => f.LoanDetail)
                .ThenInclude(d => d.Loan)
                    .ThenInclude(l => l.BorrowerReader)
                        .ThenInclude(r => r.Profile)
            .Include(f => f.Payment)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(f => f.Status == status);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(f =>
                f.LoanDetail.Copy.Book.Title.Contains(search) ||
                f.LoanDetail.Copy.Barcode.Contains(search) ||
                f.LoanDetail.Loan.BorrowerReader.Email.Contains(search) ||
                (f.LoanDetail.Loan.BorrowerReader.Profile != null &&
                 f.LoanDetail.Loan.BorrowerReader.Profile.FullName.Contains(search)) ||
                f.Reason.Contains(search));

        var totalCount = await query.CountAsync();
        var fines = await query
            .OrderByDescending(f => f.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(f => new FineListItemDto
            {
                FineId = f.FineId,
                LoanDetailId = f.LoanDetailId,
                BookTitle = f.LoanDetail.Copy.Book.Title,
                Barcode = f.LoanDetail.Copy.Barcode,
                ReaderName = f.LoanDetail.Loan.BorrowerReader.Profile != null
                    ? f.LoanDetail.Loan.BorrowerReader.Profile.FullName
                    : f.LoanDetail.Loan.BorrowerReader.Email,
                ReaderEmail = f.LoanDetail.Loan.BorrowerReader.Email,
                Reason = f.Reason,
                Amount = f.Amount,
                Status = f.Status,
                PaymentMethod = f.Payment != null ? f.Payment.Method : null,
                CreatedAt = f.CreatedAt,
                PaidAt = f.PaidAt
            })
            .ToListAsync();

        return new FineListPageDto
        {
            Fines = fines,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
}
