using System;
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
            // Thử tìm trong Accounts (Admin / Librarian) trước
            var account = await _unitOfWork.AccountRepository.GetAccountByEmailAsync(dto.Email);
            if (account != null)
            {
                bool valid = BCrypt.Net.BCrypt.Verify(dto.Password, account.PasswordHash);
                if (!valid) throw new Exception("Mật khẩu không chính xác");

                return new LoginResponseDto
                {
                    UserId   = account.AccountId,
                    Email    = account.Email,
                    FullName = account.Profile?.FullName ?? account.Email,
                    Role     = account.Role,  // "Admin" hoặc "Librarian"
                };
            }

            // Thử tìm trong Readers
            var reader = await _unitOfWork.ReaderRepository.GetReaderByEmailAsync(dto.Email);
            if (reader == null) throw new Exception("Email không tồn tại");

            bool readerValid = BCrypt.Net.BCrypt.Verify(dto.Password, reader.PasswordHash);
            if (!readerValid) throw new Exception("Mật khẩu không chính xác");

            return new LoginResponseDto
            {
                UserId   = reader.ReaderId,
                Email    = reader.Email,
                FullName = reader.Profile?.FullName ?? reader.Email,
                Role     = "Reader",
            };
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            // Kiểm tra email tồn tại ở cả 2 bảng
            var existingReader  = await _unitOfWork.ReaderRepository.GetReaderByEmailAsync(dto.Email);
            var existingAccount = await _unitOfWork.AccountRepository.GetAccountByEmailAsync(dto.Email);

            if (existingReader != null || existingAccount != null)
                throw new Exception("Email đã tồn tại");

            if (dto.Password != dto.ConfirmPassword)
                throw new Exception("Mật khẩu không khớp");

            // Đăng ký mặc định là Reader
            var reader = new Reader
            {
                ReaderId     = Guid.NewGuid(),
                Email        = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Status       = "Active",
                CreatedAt    = DateTime.UtcNow,
            };

            await _unitOfWork.ReaderRepository.AddReaderAsync(reader);

            // Tạo profile đính kèm
            var profile = new UserProfile
            {
                ReaderId = reader.ReaderId,
                FullName = dto.FullName,
            };

            await _unitOfWork.UserProfiles.AddAsync(profile);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
