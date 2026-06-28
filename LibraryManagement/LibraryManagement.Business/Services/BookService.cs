using LibraryManagement.Business.DTOs.BookDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Business.Services;

public class BookService : IBookService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IQueryable<BookListItemDto> GetBooksQuery()
    {
        return ProjectBooks(_unitOfWork.Books.Query().AsNoTracking());
    }

    public async Task<BookListPageDto> GetBooksAsync(
        string? title,
        string? language,
        string? publisher,
        bool availableOnly,
        string? sortBy,
        int page,
        int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var query = BuildBookQuery(title, language, publisher, availableOnly);

        query = sortBy switch
        {
            "title_desc" => query.OrderByDescending(b => b.Title),
            "year" => query.OrderBy(b => b.PublicationYear),
            "year_desc" => query.OrderByDescending(b => b.PublicationYear),
            _ => query.OrderBy(b => b.Title)
        };

        var totalCount = await query.CountAsync();
        var books = await ProjectBooks(query
                .Skip((page - 1) * pageSize)
                .Take(pageSize))
            .ToListAsync();

        return new BookListPageDto
        {
            Books = books,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<List<BookListItemDto>> GetLatestBooksAsync(int count)
    {
        count = Math.Clamp(count, 1, 12);

        return await ProjectBooks(_unitOfWork.Books.Query()
                .AsNoTracking()
                .OrderByDescending(b => b.CreatedAt)
                .Take(count))
            .ToListAsync();
    }

    public async Task<BookDetailDto?> GetBookDetailAsync(Guid bookId)
    {
        return await _unitOfWork.Books.Query()
            .AsNoTracking()
            .Where(b => b.BookId == bookId)
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
                    .OrderBy(a => a.Author.FullName)
                    .Select(a => a.Author.FullName)
                    .ToList(),
                Categories = b.BookCategories
                    .OrderBy(c => c.Category.CategoryName)
                    .Select(c => c.Category.CategoryName)
                    .ToList(),
                TotalCopies = b.BookCopies.Count,
                AvailableCopies = b.BookCopies.Count(c => c.Status == "Available"),
                Copies = b.BookCopies
                    .OrderBy(c => c.Barcode)
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

    private IQueryable<LibraryManagement.Models.Models.Book> BuildBookQuery(
        string? title,
        string? language,
        string? publisher,
        bool availableOnly)
    {
        var query = _unitOfWork.Books.Query().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(b => b.Title.Contains(title));

        if (!string.IsNullOrWhiteSpace(language))
            query = query.Where(b => b.Language != null && b.Language.Contains(language));

        if (!string.IsNullOrWhiteSpace(publisher))
            query = query.Where(b => b.Publisher != null && b.Publisher.PublisherName.Contains(publisher));

        if (availableOnly)
            query = query.Where(b => b.BookCopies.Any(c => c.Status == "Available"));

        return query;
    }

    private static IQueryable<BookListItemDto> ProjectBooks(IQueryable<LibraryManagement.Models.Models.Book> query)
    {
        return query.Select(b => new BookListItemDto
        {
            BookId = b.BookId,
            Title = b.Title,
            ISBN = b.ISBN,
            PublisherName = b.Publisher != null ? b.Publisher.PublisherName : null,
            PublicationYear = b.PublicationYear,
            Language = b.Language,
            CoverImageUrl = b.CoverImageUrl,
            TotalCopies = b.BookCopies.Count,
            AvailableCopies = b.BookCopies.Count(c => c.Status == "Available")
        });
    }
}
