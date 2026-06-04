using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Models.Models;

namespace LibraryManagement.Data.Interfaces
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync(string? search);
        Task<Author?> GetAuthorByIdAsync(int authorId);
        Task AddAuthorAsync(Author author);
        Task UpdateAuthorAsync(Author author);
        Task DeleteAuthorAsync(int authorId);
        Task<bool> HasBooksAsync(int authorId);
    }
}
