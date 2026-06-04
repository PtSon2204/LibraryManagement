using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.AuthDTOs;
using LibraryManagement.Business.DTOs.UserDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.UnitOfWorks;
using LibraryManagement.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserProfileDto> GetProfileAsync(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(id);

            if (user == null) throw new Exception("Không tìm thấy người dùng.");

            return new UserProfileDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth
            };
        }

        public Task<UserProfileDto> UpdateProfileAsync(Guid id, UpdateProfileDto model)
        {
            throw new NotImplementedException();
        }

        // ── Admin user management ────────────────────────────────────────────

        public async Task<IEnumerable<UserDto>> GetUsersAsync(string? search)
        {
            var query = _unitOfWork.Users
                .Query()
                .Include(u => u.Role)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(s) ||
                    u.Email.ToLower().Contains(s) ||
                    (u.Phone != null && u.Phone.Contains(s)));
            }

            return await query
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    RoleId = u.RoleId,
                    RoleName = u.Role.RoleName,
                    Email = u.Email,
                    FullName = u.FullName,
                    Phone = u.Phone,
                    Address = u.Address,
                    DateOfBirth = u.DateOfBirth,
                    Status = u.Status,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _unitOfWork.Users
                .Query()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return null;

            return new UserDto
            {
                UserId = user.UserId,
                RoleId = user.RoleId,
                RoleName = user.Role.RoleName,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                Status = user.Status,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            var existing = await _unitOfWork.UserRepository.GetUserByEmailAsync(dto.Email);
            if (existing != null)
                throw new Exception("Email đã tồn tại.");

            var user = new User
            {
                UserId = Guid.NewGuid(),
                RoleId = dto.RoleId,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                Phone = dto.Phone,
                Address = dto.Address,
                DateOfBirth = dto.DateOfBirth,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
            };

            await _unitOfWork.UserRepository.AddUserAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return (await GetUserByIdAsync(user.UserId))!;
        }

        public async Task<bool> UpdateUserAsync(Guid id, UpdateUserDto dto)
        {
            var user = await _unitOfWork.Users
                .Query()
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return false;

            user.RoleId = dto.RoleId;
            user.FullName = dto.FullName;
            user.Phone = dto.Phone;
            user.Address = dto.Address;
            user.DateOfBirth = dto.DateOfBirth;
            user.Status = dto.Status;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleUserStatusAsync(Guid id)
        {
            var user = await _unitOfWork.Users
                .Query()
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return false;

            user.Status = user.Status == "Active" ? "Inactive" : "Active";
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
