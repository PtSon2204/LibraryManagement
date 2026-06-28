using LibraryManagement.Business.DTOs.BookDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.Common;
using LibraryManagement.Data.UnitOfWorks;
using LibraryManagement.Models.Models;
using LibraryManagement.Models.Queries;
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
        return ProjectBooks(_unitOfWork.Books.Query().AsNoTracking().Where(b => !b.IsHidden));
    }

    public async Task<BookListPageDto> GetPublicBooksAsync(BookQuery bookQuery)
    {
        var page = Math.Max(bookQuery.PageNumber, 1);
        var pageSize = Math.Clamp(bookQuery.PageSize, 1, 50);

        var query = BuildPublicBookQuery(bookQuery);

        query = bookQuery.SortBy switch
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
                .Where(b => !b.IsHidden)
                .OrderByDescending(b => b.CreatedAt)
                .Take(count))
            .ToListAsync();
    }

    public async Task<BookDetailDto?> GetBookDetailAsync(Guid bookId)
    {
        return await _unitOfWork.Books.Query()
            .AsNoTracking()
            .Where(b => b.BookId == bookId && !b.IsHidden)
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
                AuthorIds = b.BookAuthors.Select(a => a.AuthorId).ToList(),
                CategoryIds = b.BookCategories.Select(c => c.CategoryId).ToList(),
                TotalCopies = b.BookCopies.Count,
                AvailableCopies = b.BookCopies.Count(c => c.Status == "Available"),
                LocationAvailability = b.BookCopies
                    .GroupBy(c => string.IsNullOrWhiteSpace(c.Location) ? "Chưa xác định" : c.Location)
                    .OrderBy(g => g.Key)
                    .Select(g => new BookLocationAvailabilityDto
                    {
                        Location = g.Key,
                        TotalCopies = g.Count(),
                        AvailableCopies = g.Count(c => c.Status == "Available")
                    })
                    .ToList(),
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

    public async Task<PagedResult<BookDto>> GetBooksAsync(BookQuery query)
    {
        query.PageNumber = Math.Max(query.PageNumber, 1);

        var dbQuery = _unitOfWork.Books.Query().AsNoTracking();

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

        var totalCount = await dbQuery.CountAsync();
        var books = await ProjectBookDtos(dbQuery
                .OrderByDescending(b => b.CreatedAt)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize))
            .ToListAsync();

        return new PagedResult<BookDto>
        {
            Data = books,
            TotalRecords = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
        };
    }

    public async Task<BookDto?> GetBookByIdAsync(Guid id)
    {
        return await ProjectBookDtos(_unitOfWork.Books.Query()
                .AsNoTracking()
                .Where(b => b.BookId == id))
            .FirstOrDefaultAsync();
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

        if (createBookDto.AuthorIds != null && createBookDto.AuthorIds.Any())
        {
            foreach (var authorId in createBookDto.AuthorIds)
            {
                book.BookAuthors.Add(new BookAuthor { AuthorId = authorId });
            }
        }

        if (createBookDto.CategoryIds != null && createBookDto.CategoryIds.Any())
        {
            foreach (var categoryId in createBookDto.CategoryIds)
            {
                book.BookCategories.Add(new BookCategory { CategoryId = categoryId });
            }
        }

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
        var book = await _unitOfWork.Books.Query()
            .Include(b => b.BookAuthors)
            .Include(b => b.BookCategories)
            .FirstOrDefaultAsync(b => b.BookId == updateBookDto.BookId);
            
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

        book.BookAuthors.Clear();
        if (updateBookDto.AuthorIds != null && updateBookDto.AuthorIds.Any())
        {
            foreach (var authorId in updateBookDto.AuthorIds)
            {
                book.BookAuthors.Add(new BookAuthor { AuthorId = authorId });
            }
        }

        book.BookCategories.Clear();
        if (updateBookDto.CategoryIds != null && updateBookDto.CategoryIds.Any())
        {
            foreach (var categoryId in updateBookDto.CategoryIds)
            {
                book.BookCategories.Add(new BookCategory { CategoryId = categoryId });
            }
        }

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

    private IQueryable<Book> BuildPublicBookQuery(BookQuery bookQuery)
    {
        var query = _unitOfWork.Books.Query().AsNoTracking().Where(b => !b.IsHidden);

        if (!string.IsNullOrWhiteSpace(bookQuery.Title))
            query = query.Where(b => b.Title.Contains(bookQuery.Title));

        if (!string.IsNullOrWhiteSpace(bookQuery.Language))
            query = query.Where(b => b.Language != null && b.Language.Contains(bookQuery.Language));

        if (!string.IsNullOrWhiteSpace(bookQuery.Publisher))
            query = query.Where(b => b.Publisher != null && b.Publisher.PublisherName.Contains(bookQuery.Publisher));

        if (bookQuery.AvailableOnly)
            query = query.Where(b => b.BookCopies.Any(c => c.Status == "Available"));

        return query;
    }

    private static IQueryable<BookListItemDto> ProjectBooks(IQueryable<Book> query)
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

    private static IQueryable<BookDto> ProjectBookDtos(IQueryable<Book> query)
    {
        return query.Select(b => new BookDto
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
            IsHidden = b.IsHidden,
            AuthorIds = b.BookAuthors.Select(a => a.AuthorId).ToList(),
            CategoryIds = b.BookCategories.Select(c => c.CategoryId).ToList()
        });
    }
}
