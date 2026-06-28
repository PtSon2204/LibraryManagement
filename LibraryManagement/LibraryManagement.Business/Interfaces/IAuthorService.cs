using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.AuthorDTOs;

namespace LibraryManagement.Business.Interfaces
{
    public interface IAuthorService
    {
        Task<List<AuthorDto>> GetAllAuthorsAsync();
    }
}
