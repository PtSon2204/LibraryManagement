using System;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.UserManagementDTOs;
using LibraryManagement.Data.Common;

namespace LibraryManagement.MVC.Interface
{
    public interface IUserService
    {
        Task<PagedResult<LibrarianListItemDto>?> GetLibrariansAsync(string? search, int pageNumber, int pageSize);
        Task<PagedResult<ReaderListItemDto>?> GetReadersAsync(string? search, int pageNumber, int pageSize);
        Task<CreateUserResponseDto?> CreateLibrarianAsync(CreateLibrarianDto model);
        Task<CreateUserResponseDto?> CreateReaderAsync(CreateReaderDto model);
        Task<bool> ToggleLibrarianStatusAsync(Guid id);
        Task<bool> ToggleReaderStatusAsync(Guid id);
    }
}
