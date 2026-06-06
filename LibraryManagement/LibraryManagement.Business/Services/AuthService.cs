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
        private readonly IJwtService _jwtService;

        public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;

        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var reader = await _unitOfWork.ReaderRepository.GetReaderByEmailAsync(dto.Email);

            if (reader != null)
            {
                bool valid = BCrypt.Net.BCrypt.Verify(dto.Password, reader.PasswordHash);

                if (!valid)
                    throw new Exception("Email hoặc mật khẩu không chính xác");

                var token = _jwtService.GenerateToken(reader.ReaderId, reader.Email, "Reader");

                return new LoginResponseDto
                {
                    Token = token,
                    Email = reader.Email,
                    Role = "Reader"
                };
            }

            var account = await _unitOfWork.AccountRepository.GetAccountByEmailAsync(dto.Email);

            bool check = BCrypt.Net.BCrypt.Verify(dto.Password, account.PasswordHash);

            if (account == null || !check)
                throw new Exception("Email hoặc mật khẩu không chính xác");

            var jwt = _jwtService.GenerateToken(account.AccountId, account.Email, account.Role);

            return new LoginResponseDto
            {
                Token = jwt,
                Email = account.Email,
                Role = account.Role
            };
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
            {
                throw new Exception("Mật khẩu xác nhận không khớp");
            }

            var readerExist = await _unitOfWork.ReaderRepository.GetReaderByEmailAsync(dto.Email);

            if (readerExist != null)
            {
                throw new Exception("Email đã tồn tại");
            }

            var reader = new Reader
            {
                ReaderId = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Readers.AddAsync(reader);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
