using LibraryManagement.Business.DTOs.BookDTOs;

namespace LibraryManagement.Business.Interfaces;

public interface IBookService
{
    IQueryable<BookListItemDto> GetBooksQuery();

    Task<BookListPageDto> GetBooksAsync(
        string? title,
        string? language,
        string? publisher,
        bool availableOnly,
        string? sortBy,
        int page,
        int pageSize);

    Task<List<BookListItemDto>> GetLatestBooksAsync(int count);

    Task<BookDetailDto?> GetBookDetailAsync(Guid bookId);
}
