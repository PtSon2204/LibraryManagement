using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.AuthorDTOs;
using LibraryManagement.Data.Common;

namespace LibraryManagement.Business.Interfaces
{
    public interface IAuthorService
    {
        Task<List<AuthorDto>> GetAllAuthorsAsync();
        Task<PagedResult<AuthorDto>> GetAuthorsAsync(string? search, int pageNumber, int pageSize);
        Task<AuthorDto?> GetAuthorByIdAsync(int id);
        Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto dto);
        Task<AuthorDto?> UpdateAuthorAsync(int id, UpdateAuthorDto dto);
        Task<bool> DeleteAuthorAsync(int id);
    }
}
