using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.BookCopyDTOs;
using LibraryManagement.Data.Common;
using LibraryManagement.Models.Queries;

namespace LibraryManagement.Business.Interfaces
{
    public interface IBookCopyService
    {
        Task<PagedResult<BookCopyDto>> GetBookCopiesAsync(BookCopyQuery query);
        Task<BookCopyDto?> GetBookCopyByIdAsync(Guid id);
        Task<BookCopyDto> CreateBookCopyAsync(CreateBookCopyDto dto);
        Task<IEnumerable<BookCopyDto>> CreateMultipleBookCopiesAsync(CreateMultipleBookCopiesDto dto);
        Task<bool> UpdateBookCopyAsync(UpdateBookCopyDto dto);
        Task<bool> ToggleHideAsync(Guid id);
        Task<bool> DeleteBookCopyAsync(Guid id);
    }
}
