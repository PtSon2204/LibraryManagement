using System;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.UserManagementDTOs;
using LibraryManagement.Data.Common;

namespace LibraryManagement.Business.Interfaces
{
    public interface IUserManagementService
    {
        Task<PagedResult<LibrarianListItemDto>> GetLibrariansAsync(string? search, int pageNumber, int pageSize);
        Task<PagedResult<ReaderListItemDto>> GetReadersAsync(string? search, int pageNumber, int pageSize);
        Task<CreateUserResponseDto> CreateLibrarianAsync(CreateLibrarianDto dto);
        Task<CreateUserResponseDto> CreateReaderAsync(CreateReaderDto dto);
        Task<bool> ToggleLibrarianStatusAsync(Guid id);
        Task<bool> ToggleReaderStatusAsync(Guid id);
    }
}
