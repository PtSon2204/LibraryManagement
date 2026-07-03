using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.BookCopyDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.Common;
using LibraryManagement.Data.UnitOfWorks;
using LibraryManagement.Models.Models;
using LibraryManagement.Models.Queries;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Business.Services
{
    public class BookCopyService : IBookCopyService
    {
        private readonly IUnitOfWork _unitOfWork;

        // Status dùng để đánh dấu ẩn (soft-hide)
        private const string HiddenStatus = "Hidden";

        public BookCopyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static BookCopyDto MapToDto(BookCopy copy) => new BookCopyDto
        {
            CopyId       = copy.CopyId,
            BookId       = copy.BookId,
            BookTitle    = copy.Book?.Title ?? string.Empty,
            Barcode      = copy.Barcode,
            Status       = copy.Status,
            ShelfSlotId  = copy.ShelfSlotId,
            SlotLocation = BuildSlotLocation(copy.ShelfSlot),
            AddedDate    = copy.AddedDate
        };

        /// <summary>
        /// Tạo chuỗi hiển thị vị trí đầy đủ, VD: "Tầng 1 > Giá A > Kệ 2 > S01"
        /// </summary>
        private static string? BuildSlotLocation(ShelfSlot? slot)
        {
            if (slot == null) return null;
            var shelf     = slot.Shelf;
            var bookshelf = shelf?.Bookshelf;
            var floor     = bookshelf?.Floor;
            return $"{floor?.FloorName} > {bookshelf?.Name} > {shelf?.Name} > {slot.SlotCode}";
        }

        // ── Includes helper ───────────────────────────────────────────────────────

        private IQueryable<BookCopy> BaseQuery() =>
            _unitOfWork.BookCopies.Query()
                .Include(c => c.Book)
                .Include(c => c.ShelfSlot)
                    .ThenInclude(sl => sl != null ? sl.Shelf : null)
                        .ThenInclude(s => s != null ? s.Bookshelf : null)
                            .ThenInclude(b => b != null ? b.Floor : null);

        // ── GET paged list ────────────────────────────────────────────────────────

        public async Task<PagedResult<BookCopyDto>> GetBookCopiesAsync(BookCopyQuery query)
        {
            IQueryable<BookCopy> dbQuery = BaseQuery();

            // Mặc định ẩn những bản sao đã bị hide
            if (!query.IncludeHidden.GetValueOrDefault())
            {
                dbQuery = dbQuery.Where(c => c.Status != HiddenStatus);
            }

            // Tìm theo Barcode hoặc tiêu đề sách
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                dbQuery = dbQuery.Where(c =>
                    c.Barcode.Contains(query.SearchTerm) ||
                    c.Book.Title.Contains(query.SearchTerm));
            }

            // Lọc theo BookId
            if (query.BookId.HasValue)
            {
                dbQuery = dbQuery.Where(c => c.BookId == query.BookId.Value);
            }

            // Lọc theo Status
            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                dbQuery = dbQuery.Where(c => c.Status == query.Status);
            }

            // Lọc theo SlotId
            if (!string.IsNullOrWhiteSpace(query.Location))
            {
                // Hỗ trợ tìm theo SlotCode (nếu truyền vào chuỗi)
                dbQuery = dbQuery.Where(c =>
                    c.ShelfSlot != null && c.ShelfSlot.SlotCode.Contains(query.Location));
            }

            int totalCount = await dbQuery.CountAsync();

            var copies = await dbQuery
                .OrderByDescending(c => c.AddedDate)
                .ThenBy(c => c.Barcode)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PagedResult<BookCopyDto>
            {
                Data         = copies.Select(MapToDto).ToList(),
                TotalRecords = totalCount,
                PageNumber   = query.PageNumber,
                PageSize     = query.PageSize,
                TotalPages   = (int)Math.Ceiling((double)totalCount / query.PageSize)
            };
        }

        // ── GET by id ─────────────────────────────────────────────────────────────

        public async Task<BookCopyDto?> GetBookCopyByIdAsync(Guid id)
        {
            var copy = await BaseQuery()
                .FirstOrDefaultAsync(c => c.CopyId == id);
            return copy == null ? null : MapToDto(copy);
        }

        // ── CREATE single ─────────────────────────────────────────────────────────

        public async Task<BookCopyDto> CreateBookCopyAsync(CreateBookCopyDto dto)
        {
            if (dto.ShelfSlotId.HasValue)
            {
                var slot = await _unitOfWork.ShelfSlots.Query()
                    .Include(s => s.BookCopies)
                    .FirstOrDefaultAsync(s => s.SlotId == dto.ShelfSlotId.Value);

                if (slot != null)
                {
                    int currentCount = slot.BookCopies.Count(bc => bc.Status != HiddenStatus);
                    int newStatusCount = (dto.Status != HiddenStatus) ? 1 : 0;

                    if (currentCount + newStatusCount > slot.Capacity)
                    {
                        throw new InvalidOperationException($"Vị trí lưu trữ {slot.SlotCode} đã đầy (Sức chứa: {slot.Capacity}).");
                    }
                }
            }

            var copy = new BookCopy
            {
                CopyId      = Guid.NewGuid(),
                BookId      = dto.BookId,
                Barcode     = dto.Barcode,
                Status      = dto.Status,
                ShelfSlotId = dto.ShelfSlotId,
                AddedDate   = dto.AddedDate ?? DateOnly.FromDateTime(DateTime.UtcNow)
            };

            await _unitOfWork.BookCopies.AddAsync(copy);
            await _unitOfWork.SaveChangesAsync();

            // Reload với đầy đủ include để trả về SlotLocation
            var savedCopy = await BaseQuery()
                .FirstOrDefaultAsync(c => c.CopyId == copy.CopyId);

            return MapToDto(savedCopy!);
        }

        // ── CREATE multiple ───────────────────────────────────────────────────────

        public async Task<IEnumerable<BookCopyDto>> CreateMultipleBookCopiesAsync(CreateMultipleBookCopiesDto dto)
        {
            var slotGroups = dto.Copies.Where(c => c.ShelfSlotId.HasValue).GroupBy(c => c.ShelfSlotId!.Value).ToList();
            foreach (var group in slotGroups)
            {
                var slotId = group.Key;
                var slot = await _unitOfWork.ShelfSlots.Query()
                    .Include(s => s.BookCopies)
                    .FirstOrDefaultAsync(s => s.SlotId == slotId);
                if (slot != null)
                {
                    int currentCount = slot.BookCopies.Count(bc => bc.Status != HiddenStatus);
                    int copiesToAdd = group.Count(c => c.Status != HiddenStatus);
                    if (currentCount + copiesToAdd > slot.Capacity)
                    {
                        throw new InvalidOperationException($"Vị trí lưu trữ {slot.SlotCode} không đủ sức chứa (Trống: {Math.Max(0, slot.Capacity - currentCount)}, Cần thêm: {copiesToAdd}).");
                    }
                }
            }

            var today  = DateOnly.FromDateTime(DateTime.UtcNow);
            var copies = dto.Copies.Select(item => new BookCopy
            {
                CopyId      = Guid.NewGuid(),
                BookId      = dto.BookId,
                Barcode     = item.Barcode,
                Status      = item.Status,
                ShelfSlotId = item.ShelfSlotId,
                AddedDate   = item.AddedDate ?? today
            }).ToList();

            foreach (var copy in copies)
                await _unitOfWork.BookCopies.AddAsync(copy);

            await _unitOfWork.SaveChangesAsync();

            // Reload để map đầy đủ
            var ids    = copies.Select(c => c.CopyId).ToList();
            var saved  = await BaseQuery()
                .Where(c => ids.Contains(c.CopyId))
                .ToListAsync();

            return saved.Select(MapToDto);
        }

        // ── UPDATE ────────────────────────────────────────────────────────────────

        public async Task<bool> UpdateBookCopyAsync(UpdateBookCopyDto dto)
        {
            var copy = await _unitOfWork.BookCopies.GetByIdAsync(dto.CopyId);
            if (copy == null) return false;

            if (dto.ShelfSlotId.HasValue)
            {
                var slot = await _unitOfWork.ShelfSlots.Query()
                    .Include(s => s.BookCopies)
                    .FirstOrDefaultAsync(s => s.SlotId == dto.ShelfSlotId.Value);

                if (slot != null)
                {
                    int currentCount = slot.BookCopies.Count(bc => bc.Status != HiddenStatus && bc.CopyId != copy.CopyId);
                    int newStatusCount = (dto.Status != HiddenStatus) ? 1 : 0;
                    
                    if (currentCount + newStatusCount > slot.Capacity)
                    {
                        throw new InvalidOperationException($"Vị trí lưu trữ {slot.SlotCode} đã đầy (Sức chứa: {slot.Capacity}).");
                    }
                }
            }

            copy.Barcode     = dto.Barcode;
            copy.Status      = dto.Status;
            copy.ShelfSlotId = dto.ShelfSlotId;

            _unitOfWork.BookCopies.Update(copy);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ── TOGGLE HIDE ───────────────────────────────────────────────────────────

        public async Task<bool> ToggleHideAsync(Guid id)
        {
            var copy = await _unitOfWork.BookCopies.GetByIdAsync(id);
            if (copy == null) return false;

            // Nếu đang Hidden thì restore về Available, ngược lại hide lại
            copy.Status = copy.Status == HiddenStatus ? "Available" : HiddenStatus;

            _unitOfWork.BookCopies.Update(copy);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ── DELETE ────────────────────────────────────────────────────────────────

        public async Task<bool> DeleteBookCopyAsync(Guid id)
        {
            var copy = await _unitOfWork.BookCopies.GetByIdAsync(id);
            if (copy == null) return false;

            _unitOfWork.BookCopies.Delete(copy);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
