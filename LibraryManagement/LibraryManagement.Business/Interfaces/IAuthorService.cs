using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs;

namespace LibraryManagement.Business.Interfaces
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorDto>> GetAuthorsAsync(string? search);
        Task<AuthorDto?> GetAuthorByIdAsync(int authorId);
        Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createAuthorDto);
        Task<bool> UpdateAuthorAsync(int authorId, UpdateAuthorDto updateAuthorDto);
        Task<bool> DeleteAuthorAsync(int authorId);
    }
}
