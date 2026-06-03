using LibraryManagement.Business.DTOs.BookDTOs;

namespace LibraryManagement.Business.Interfaces
{
    public interface IBookQueryService
    {
        IQueryable<BookOdataDto> GetBooksOdataQuery();
        Task<BookDetailDto?> GetBookDetailAsync(Guid id);
        Task<BookDetailDto> CreateBookAsync(CreateBookDto dto);
    }
}
