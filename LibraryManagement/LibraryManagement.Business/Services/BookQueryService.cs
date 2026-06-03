using LibraryManagement.Business.DTOs.BookDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.UnitOfWorks;
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
