using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.BookDTOs;
using LibraryManagement.Data.Common;
using LibraryManagement.Models.Queries;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.UnitOfWorks;
using LibraryManagement.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Business.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<BookDto>> GetBooksAsync(BookQuery query)
        {
            var dbQuery = _unitOfWork.Books.Query();

            if (!query.IncludeHidden.GetValueOrDefault())
            {
                dbQuery = dbQuery.Where(b => !b.IsHidden);
            }

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                dbQuery = dbQuery.Where(b => b.Title.Contains(query.SearchTerm) || 
                                         (b.ISBN != null && b.ISBN.Contains(query.SearchTerm)));
            }

            if (query.PublisherId.HasValue)
            {
                dbQuery = dbQuery.Where(b => b.PublisherId == query.PublisherId.Value);
            }

            if (query.PublicationYear.HasValue)
            {
                dbQuery = dbQuery.Where(b => b.PublicationYear == query.PublicationYear.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Language))
            {
                dbQuery = dbQuery.Where(b => b.Language == query.Language);
            }

            int totalCount = await dbQuery.CountAsync();

            var books = await dbQuery
                .OrderByDescending(b => b.CreatedAt)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(b => new BookDto
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    ISBN = b.ISBN,
                    PublisherId = b.PublisherId,
                    PublisherName = b.Publisher != null ? b.Publisher.PublisherName : null,
                    PublicationYear = b.PublicationYear,
                    Language = b.Language,
                    Edition = b.Edition,
                    Description = b.Description,
                    CoverImageUrl = b.CoverImageUrl,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt,
                    IsHidden = b.IsHidden
                })
                .ToListAsync();

            return new PagedResult<BookDto>
            {
                Data = books,
                TotalRecords = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalPages = (int)System.Math.Ceiling((double)totalCount / query.PageSize)
            };
        }

        public async Task<BookDto?> GetBookByIdAsync(Guid id)
        {
            var book = await _unitOfWork.Books.Query()
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(b => b.BookId == id);
            if (book == null) return null;

            return new BookDto
            {
                BookId = book.BookId,
                Title = book.Title,
                ISBN = book.ISBN,
                PublisherId = book.PublisherId,
                PublisherName = book.Publisher?.PublisherName,
                PublicationYear = book.PublicationYear,
                Language = book.Language,
                Edition = book.Edition,
                Description = book.Description,
                CoverImageUrl = book.CoverImageUrl,
                CreatedAt = book.CreatedAt,
                UpdatedAt = book.UpdatedAt,
                IsHidden = book.IsHidden
            };
        }

        public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto)
        {
            var book = new Book
            {
                BookId = Guid.NewGuid(),
                Title = createBookDto.Title,
                ISBN = createBookDto.ISBN,
                PublisherId = createBookDto.PublisherId,
                PublicationYear = createBookDto.PublicationYear,
                Language = createBookDto.Language,
                Edition = createBookDto.Edition,
                Description = createBookDto.Description,
                CoverImageUrl = createBookDto.CoverImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Books.AddAsync(book);
            await _unitOfWork.SaveChangesAsync();

            return new BookDto
            {
                BookId = book.BookId,
                Title = book.Title,
                ISBN = book.ISBN,
                PublisherId = book.PublisherId,
                PublicationYear = book.PublicationYear,
                Language = book.Language,
                Edition = book.Edition,
                Description = book.Description,
                CoverImageUrl = book.CoverImageUrl,
                CreatedAt = book.CreatedAt,
                UpdatedAt = book.UpdatedAt,
                IsHidden = book.IsHidden
            };
        }

        public async Task<bool> UpdateBookAsync(UpdateBookDto updateBookDto)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(updateBookDto.BookId);
            if (book == null) return false;

            book.Title = updateBookDto.Title;
            book.ISBN = updateBookDto.ISBN;
            book.PublisherId = updateBookDto.PublisherId;
            book.PublicationYear = updateBookDto.PublicationYear;
            book.Language = updateBookDto.Language;
            book.Edition = updateBookDto.Edition;
            book.Description = updateBookDto.Description;
            book.CoverImageUrl = updateBookDto.CoverImageUrl;
            book.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Books.Update(book);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ToggleHideAsync(Guid id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null) return false;

            book.IsHidden = !book.IsHidden;
            book.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Books.Update(book);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteBookAsync(Guid id)
        {
            var book = await _unitOfWork.Books.GetByIdAsync(id);
            if (book == null) return false;

            _unitOfWork.Books.Delete(book);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
