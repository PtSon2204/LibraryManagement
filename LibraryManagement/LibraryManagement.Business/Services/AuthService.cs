using System;
using System.Linq;
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
        private readonly IEmailService _emailService;

        public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _emailService = emailService;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            // Kiểm tra cả Account (Staff/Admin) và Reader
            var account = await _unitOfWork.AccountRepository.GetAccountByEmailAsync(email);
            var reader = await _unitOfWork.ReaderRepository.GetReaderByEmailAsync(email);

            if (account == null && reader == null)
                return false; // Email không tồn tại

            var newPassword = GenerateRandomPassword();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            if (account != null)
            {
                account.PasswordHash = hashedPassword;
                _unitOfWork.AccountRepository.UpdateAccount(account);
            }
            else if (reader != null)
            {
                reader.PasswordHash = hashedPassword;
                _unitOfWork.ReaderRepository.UpdateReader(reader);
            }

            await _unitOfWork.SaveChangesAsync();

            // Gửi email chứa mật khẩu mới
            await _emailService.SendEmailAsync(
                email,
                "[Library] Khôi phục mật khẩu",
                $"<p>Xin chào,</p><p>Hệ thống đã tạo một mật khẩu mới ngẫu nhiên cho bạn.</p><p>Mật khẩu mới của bạn là: <strong>{newPassword}</strong></p><p>Vui lòng đăng nhập bằng mật khẩu này. Bạn nên đổi lại mật khẩu của riêng mình sau khi đăng nhập thành công.</p><p>Nếu không phải bạn, hãy liên hệ với quản trị viên ngay lập tức.</p>"
            );

            return true;
        }

        private string GenerateRandomPassword()
        {
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "!@#$%^&*()";

            var random = new Random();
            var password = new char[10];

            password[0] = upper[random.Next(upper.Length)];
            password[1] = lower[random.Next(lower.Length)];
            password[2] = digits[random.Next(digits.Length)];
            password[3] = special[random.Next(special.Length)];

            const string allChars = upper + lower + digits + special;
            for (int i = 4; i < 10; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }   

            return new string(password.OrderBy(x => random.Next()).ToArray());
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
                    UserId = reader.ReaderId,
                    Token = token,
                    Email = reader.Email,
                    FullName = reader.Profile?.FullName,
                    Role = "Reader"
                };
            }

            var account = await _unitOfWork.AccountRepository.GetAccountByEmailAsync(dto.Email);

            if (account == null)
                throw new Exception("Email hoặc mật khẩu không chính xác");

            bool check = BCrypt.Net.BCrypt.Verify(
                dto.Password,
                account.PasswordHash);

            if (!check)
                throw new Exception("Email hoặc mật khẩu không chính xác");

            var jwt = _jwtService.GenerateToken(account.AccountId, account.Email, account.Role);

            return new LoginResponseDto
            {
                UserId = account.AccountId,
                Token = jwt,
                Email = account.Email,
                FullName = account.Profile?.FullName,
                Role = account.Role
            };
        }

        public async Task<LoginResponseDto> GoogleLoginAsync(GoogleLoginDto dto)
        {
            // Tìm reader theo email Google đã xác thực
            var reader = await _unitOfWork.ReaderRepository.GetReaderByEmailAsync(dto.Email);

            if (reader == null)
            {
                // Tạo Reader mới nếu chưa có tài khoản
                reader = new Reader
                {
                    ReaderId = Guid.NewGuid(),
                    Email = dto.Email,
                    // Placeholder hash - user này đăng nhập bằng Google, không có password truyền thống
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Readers.AddAsync(reader);

                var profile = new UserProfile
                {
                    UserProfileId = Guid.NewGuid(),
                    ReaderId = reader.ReaderId,
                    FullName = dto.FullName ?? dto.Email
                };

                await _unitOfWork.UserProfiles.AddAsync(profile);

                await _unitOfWork.SaveChangesAsync();

                // Reload để có Navigation property Profile
                reader = await _unitOfWork.ReaderRepository.GetReaderByEmailAsync(dto.Email);
            }

            var token = _jwtService.GenerateToken(reader!.ReaderId, reader.Email, "Reader");

            return new LoginResponseDto
            {
                UserId = reader.ReaderId,
                Token = token,
                Email = reader.Email,
                FullName = reader.Profile?.FullName,
                Role = "Reader"
            };
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
            {
                throw new Exception("Mật khẩu xác nhận không khớp");
            }

            var readerExist = await _unitOfWork.ReaderRepository.GetReaderByEmailAsync(dto.Email);

            var accountExist = await _unitOfWork.AccountRepository.GetAccountByEmailAsync(dto.Email);

            if (readerExist != null || accountExist != null)
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

            var profile = new UserProfile
            {
                UserProfileId = Guid.NewGuid(),
                ReaderId = reader.ReaderId,
                FullName = dto.FullName
            };
            
            await _unitOfWork.UserProfiles.AddAsync(profile);

            await _unitOfWork.SaveChangesAsync();
        }


        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmNewPassword)
                throw new Exception("Mật khẩu xác nhận không khớp.");

            // Check if user is an Account
            var account = await _unitOfWork.Accounts.GetByIdAsync(userId);
            if (account != null)
            {
                bool valid = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, account.PasswordHash);
                if (!valid)
                    throw new Exception("Mật khẩu hiện tại không chính xác.");

                account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                _unitOfWork.Accounts.Update(account);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            // Check if user is a Reader
            var reader = await _unitOfWork.Readers.GetByIdAsync(userId);
            if (reader != null)
            {
                bool valid = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, reader.PasswordHash);
                if (!valid)
                    throw new Exception("Mật khẩu hiện tại không chính xác.");

                reader.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                _unitOfWork.Readers.Update(reader);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }

            throw new Exception("Không tìm thấy người dùng.");
        }
    }
}
