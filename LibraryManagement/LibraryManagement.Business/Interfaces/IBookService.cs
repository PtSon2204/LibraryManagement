using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.BookDTOs;
using LibraryManagement.Data.Common;
using LibraryManagement.Models.Queries;

namespace LibraryManagement.Business.Interfaces
{
    public interface IBookService
    {
        Task<PagedResult<BookDto>> GetBooksAsync(BookQuery query);
        Task<BookDto?> GetBookByIdAsync(Guid id);
        Task<BookDto> CreateBookAsync(CreateBookDto createBookDto);
        Task<bool> UpdateBookAsync(UpdateBookDto updateBookDto);
        Task<bool> ToggleHideAsync(Guid id);
        Task<bool> DeleteBookAsync(Guid id);
    }
}
