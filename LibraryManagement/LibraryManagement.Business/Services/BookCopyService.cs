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
            CopyId    = copy.CopyId,
            BookId    = copy.BookId,
            BookTitle = copy.Book?.Title ?? string.Empty,
            Barcode   = copy.Barcode,
            Status    = copy.Status,
            Location  = copy.Location,
            AddedDate = copy.AddedDate
        };

        // ── GET paged list ────────────────────────────────────────────────────────

        public async Task<PagedResult<BookCopyDto>> GetBookCopiesAsync(BookCopyQuery query)
        {
            IQueryable<BookCopy> dbQuery = _unitOfWork.BookCopies.Query()
                .Include(c => c.Book);

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

            // Lọc theo Status (chỉ khi không phải filter ẩn)
            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                dbQuery = dbQuery.Where(c => c.Status == query.Status);
            }

            // Lọc theo Location
            if (!string.IsNullOrWhiteSpace(query.Location))
            {
                dbQuery = dbQuery.Where(c => c.Location != null && c.Location.Contains(query.Location));
            }

            int totalCount = await dbQuery.CountAsync();

            var copies = await dbQuery
                .OrderByDescending(c => c.AddedDate)
                .ThenBy(c => c.Barcode)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(c => new BookCopyDto
                {
                    CopyId    = c.CopyId,
                    BookId    = c.BookId,
                    BookTitle = c.Book != null ? c.Book.Title : string.Empty,
                    Barcode   = c.Barcode,
                    Status    = c.Status,
                    Location  = c.Location,
                    AddedDate = c.AddedDate
                })
                .ToListAsync();

            return new PagedResult<BookCopyDto>
            {
                Data         = copies,
                TotalRecords = totalCount,
                PageNumber   = query.PageNumber,
                PageSize     = query.PageSize,
                TotalPages   = (int)Math.Ceiling((double)totalCount / query.PageSize)
            };
        }

        // ── GET by id ─────────────────────────────────────────────────────────────

        public async Task<BookCopyDto?> GetBookCopyByIdAsync(Guid id)
        {
            var copy = await _unitOfWork.BookCopies.Query()
                .Include(c => c.Book)
                .FirstOrDefaultAsync(c => c.CopyId == id);

            return copy == null ? null : MapToDto(copy);
        }

        // ── CREATE single ─────────────────────────────────────────────────────────

        public async Task<BookCopyDto> CreateBookCopyAsync(CreateBookCopyDto dto)
        {
            var copy = new BookCopy
            {
                CopyId    = Guid.NewGuid(),
                BookId    = dto.BookId,
                Barcode   = dto.Barcode,
                Status    = dto.Status,
                Location  = dto.Location,
                AddedDate = dto.AddedDate ?? DateOnly.FromDateTime(DateTime.UtcNow)
            };

            await _unitOfWork.BookCopies.AddAsync(copy);
            await _unitOfWork.SaveChangesAsync();

            // Load book để trả về BookTitle
            var savedCopy = await _unitOfWork.BookCopies.Query()
                .Include(c => c.Book)
                .FirstOrDefaultAsync(c => c.CopyId == copy.CopyId);

            return MapToDto(savedCopy!);
        }

        // ── CREATE multiple ───────────────────────────────────────────────────────

        public async Task<IEnumerable<BookCopyDto>> CreateMultipleBookCopiesAsync(CreateMultipleBookCopiesDto dto)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var copies = dto.Copies.Select(item => new BookCopy
            {
                CopyId    = Guid.NewGuid(),
                BookId    = dto.BookId,
                Barcode   = item.Barcode,
                Status    = item.Status,
                Location  = item.Location,
                AddedDate = item.AddedDate ?? today
            }).ToList();

            foreach (var copy in copies)
            {
                await _unitOfWork.BookCopies.AddAsync(copy);
            }
            await _unitOfWork.SaveChangesAsync();

            // Load book title để map
            var book = await _unitOfWork.Books.GetByIdAsync(dto.BookId);
            string bookTitle = book?.Title ?? string.Empty;

            return copies.Select(c => new BookCopyDto
            {
                CopyId    = c.CopyId,
                BookId    = c.BookId,
                BookTitle = bookTitle,
                Barcode   = c.Barcode,
                Status    = c.Status,
                Location  = c.Location,
                AddedDate = c.AddedDate
            });
        }

        // ── UPDATE ────────────────────────────────────────────────────────────────

        public async Task<bool> UpdateBookCopyAsync(UpdateBookCopyDto dto)
        {
            var copy = await _unitOfWork.BookCopies.GetByIdAsync(dto.CopyId);
            if (copy == null) return false;

            copy.Barcode  = dto.Barcode;
            copy.Status   = dto.Status;
            copy.Location = dto.Location;

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
