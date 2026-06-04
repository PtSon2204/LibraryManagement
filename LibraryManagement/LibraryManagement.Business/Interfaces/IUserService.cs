using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.AuthDTOs;
using LibraryManagement.Business.DTOs.UserDTOs;

namespace LibraryManagement.Business.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileDto> GetProfileAsync(Guid id);
        Task<UserProfileDto> UpdateProfileAsync(Guid id, UpdateProfileDto model);

        // Admin user management
        Task<IEnumerable<UserDto>> GetUsersAsync(string? search);
        Task<UserDto?> GetUserByIdAsync(Guid id);
        Task<UserDto> CreateUserAsync(CreateUserDto dto);
        Task<bool> UpdateUserAsync(Guid id, UpdateUserDto dto);
        Task<bool> ToggleUserStatusAsync(Guid id);
    }
}
