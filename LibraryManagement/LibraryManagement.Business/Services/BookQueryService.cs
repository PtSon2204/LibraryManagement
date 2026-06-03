using LibraryManagement.Business.DTOs.BookDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.UnitOfWorks;
using LibraryManagement.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Business.Services
{
    public class BookQueryService : IBookQueryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILibraryPolicyService _libraryPolicyService;

        public BookQueryService(IUnitOfWork unitOfWork, ILibraryPolicyService libraryPolicyService)
        {
            _unitOfWork = unitOfWork;
            _libraryPolicyService = libraryPolicyService;
        }

        public async Task<BookDetailDto?> GetBookDetailAsync(Guid id)
        {
            return await _unitOfWork.Books
                .Query()
                .Where(b => b.BookId == id)
                .Select(b => new BookDetailDto
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    ISBN = b.ISBN,
                    Description = b.Description,
                    CoverImageUrl = b.CoverImageUrl,
                    PublisherName = b.Publisher != null ? b.Publisher.PublisherName : null,
                    PublicationYear = b.PublicationYear,
                    Language = b.Language,
                    Edition = b.Edition,

                    Authors = b.BookAuthors
                        .Select(ba => ba.Author.FullName)
                        .ToList(),

                    Categories = b.BookCategories
                        .Select(bc => bc.Category.CategoryName)
                        .ToList(),

                    TotalCopies = b.BookCopies.Count(),

                    AvailableCopies = b.BookCopies.Count(c =>
                        c.Status == _libraryPolicyService.AvailableCopyStatus),

                    Copies = b.BookCopies
                        .Select(c => new BookCopyDto
                        {
                            CopyId = c.CopyId,
                            Barcode = c.Barcode,
                            Status = c.Status,
                            Location = c.Location,
                            AddedDate = c.AddedDate
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<BookDetailDto> CreateBookAsync(CreateBookDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.ISBN))
            {
                var isbn = dto.ISBN.Trim();
                var isbnExists = await _unitOfWork.Books.Query()
                    .AnyAsync(b => b.ISBN == isbn);

                if (isbnExists)
                {
                    throw new InvalidOperationException("ISBN already exists.");
                }
            }

            var book = new Book
            {
                Title = dto.Title.Trim(),
                ISBN = string.IsNullOrWhiteSpace(dto.ISBN) ? null : dto.ISBN.Trim(),
                PublicationYear = dto.PublicationYear,
                Language = string.IsNullOrWhiteSpace(dto.Language) ? null : dto.Language.Trim(),
                Edition = string.IsNullOrWhiteSpace(dto.Edition) ? null : dto.Edition.Trim(),
                Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                CoverImageUrl = string.IsNullOrWhiteSpace(dto.CoverImageUrl) ? null : dto.CoverImageUrl.Trim(),
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Books.AddAsync(book);
            await _unitOfWork.SaveChangesAsync();

            return await GetBookDetailAsync(book.BookId)
                ?? throw new InvalidOperationException("Created book could not be loaded.");
        }

        public IQueryable<BookOdataDto> GetBooksOdataQuery()
        {
            return _unitOfWork.Books
                .Query()
                .Select(b => new BookOdataDto
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    ISBN = b.ISBN,
                    PublisherName = b.Publisher != null ? b.Publisher.PublisherName : null,
                    PublicationYear = b.PublicationYear,
                    Language = b.Language,
                    CoverImageUrl = b.CoverImageUrl,
                    TotalCopies = b.BookCopies.Count(),
                    AvailableCopies = b.BookCopies.Count(c => c.Status == _libraryPolicyService.AvailableCopyStatus)
                });
        }
    }
}
