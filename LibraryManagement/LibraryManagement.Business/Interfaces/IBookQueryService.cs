using LibraryManagement.Business.DTOs.BookDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Business.Interfaces
{
    public interface IBookQueryService
    {
        IQueryable<BookOdataDto> GetBooksOdataQuery();
        Task<BookDetailDto?> GetBookDetailAsync(Guid id);
    }
}
