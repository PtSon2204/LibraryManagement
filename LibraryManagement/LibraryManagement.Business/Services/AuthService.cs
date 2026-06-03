using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.AuthDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.UnitOfWorks;
using LibraryManagement.Models.Models;

namespace LibraryManagement.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(dto.Email);

            if (user == null)
            {
                throw new Exception("Email không tồn tại");
            }

            bool valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!valid)
            {
                throw new Exception("Mật khẩu không chính xác");
            }

            return new LoginResponseDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.RoleName,
            };
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _unitOfWork.UserRepository.GetUserByEmailAsync(dto.Email);

            if (existingUser != null)
            {
                throw new Exception("Email đã tồn tại");
            }

            if (dto.Password != dto.ConfirmPassword)
            {
                throw new Exception("Mật khẩu không khớp");
            }

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                RoleId = 3,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
            };

            await _unitOfWork.UserRepository.AddUserAsync(user);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
