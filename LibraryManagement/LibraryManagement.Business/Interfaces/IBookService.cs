using LibraryManagement.Business.DTOs.BookDTOs;
using LibraryManagement.Data.Common;
using LibraryManagement.Models.Queries;

namespace LibraryManagement.Business.Interfaces;

public interface IBookService
{
    IQueryable<BookListItemDto> GetBooksQuery();

    Task<BookListPageDto> GetPublicBooksAsync(BookQuery query);

    Task<List<BookListItemDto>> GetLatestBooksAsync(int count);

    Task<BookDetailDto?> GetBookDetailAsync(Guid bookId);

    Task<PagedResult<BookDto>> GetBooksAsync(BookQuery query);

    Task<BookDto?> GetBookByIdAsync(Guid id);

    Task<BookDto> CreateBookAsync(CreateBookDto createBookDto);

    Task<bool> UpdateBookAsync(UpdateBookDto updateBookDto);

    Task<bool> ToggleHideAsync(Guid id);

    Task<bool> DeleteBookAsync(Guid id);
}
