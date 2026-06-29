using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.MVC.ViewModels.Author;
using LibraryManagement.MVC.ViewModels.Books;

namespace LibraryManagement.MVC.Interface
{
    public interface IAuthorService
    {
        Task<List<AuthorOption>?> GetAllAuthorsAsync();
        Task<AuthorListViewModel?> GetAuthorsAsync(string? search, int pageNumber, int pageSize);
        Task<AuthorViewModel?> GetAuthorByIdAsync(int id);
        Task<string?> CreateAuthorAsync(AuthorViewModel model);
        Task<string?> UpdateAuthorAsync(AuthorViewModel model);
        Task<bool> DeleteAuthorAsync(int id);
    }
}
