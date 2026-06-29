using System;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.UserManagementDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.Common;
using LibraryManagement.Data.UnitOfWorks;
using LibraryManagement.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Business.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public UserManagementService(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<PagedResult<LibrarianListItemDto>> GetLibrariansAsync(string? search, int pageNumber, int pageSize)
        {
            IQueryable<Account> query = _unitOfWork.Accounts.Query()
                .Include(a => a.Profile)
                .Where(a => a.Role == "Librarian");

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(a =>
                    a.Email.Contains(search) ||
                    (a.Profile != null && a.Profile.FullName.Contains(search)) ||
                    (a.Profile != null && a.Profile.Phone != null && a.Profile.Phone.Contains(search)));
            }

            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(a => a.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new LibrarianListItemDto
                {
                    AccountId = a.AccountId,
                    Email = a.Email,
                    FullName = a.Profile != null ? a.Profile.FullName : "Chưa cập nhật",
                    Phone = a.Profile != null ? a.Profile.Phone : null,
                    Address = a.Profile != null ? a.Profile.Address : null,
                    DateOfBirth = a.Profile != null ? a.Profile.DateOfBirth : null,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return new PagedResult<LibrarianListItemDto>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<PagedResult<ReaderListItemDto>> GetReadersAsync(string? search, int pageNumber, int pageSize)
        {
            IQueryable<Reader> query = _unitOfWork.Readers.Query()
                .Include(r => r.Profile);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r =>
                    r.Email.Contains(search) ||
                    (r.Profile != null && r.Profile.FullName.Contains(search)) ||
                    (r.Profile != null && r.Profile.Phone != null && r.Profile.Phone.Contains(search)));
            }

            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ReaderListItemDto
                {
                    ReaderId = r.ReaderId,
                    Email = r.Email,
                    FullName = r.Profile != null ? r.Profile.FullName : "Chưa cập nhật",
                    Phone = r.Profile != null ? r.Profile.Phone : null,
                    Address = r.Profile != null ? r.Profile.Address : null,
                    DateOfBirth = r.Profile != null ? r.Profile.DateOfBirth : null,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return new PagedResult<ReaderListItemDto>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<CreateUserResponseDto> CreateLibrarianAsync(CreateLibrarianDto dto)
        {
            var existsAccount = await _unitOfWork.AccountRepository.GetAccountByEmailAsync(dto.Email);
            var existsReader = await _unitOfWork.ReaderRepository.GetReaderByEmailAsync(dto.Email);
            if (existsAccount != null || existsReader != null)
            {
                throw new Exception("Email đã tồn tại trong hệ thống.");
            }

            var password = GenerateRandomPassword();
            var account = new Account
            {
                AccountId = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Librarian",
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            var profile = new UserProfile
            {
                UserProfileId = Guid.NewGuid(),
                AccountId = account.AccountId,
                FullName = dto.FullName,
                Phone = dto.Phone,
                Address = dto.Address,
                DateOfBirth = dto.DateOfBirth
            };

            await _unitOfWork.Accounts.AddAsync(account);
            await _unitOfWork.UserProfiles.AddAsync(profile);
            await _unitOfWork.SaveChangesAsync();

            // Send Email
            try
            {
                await _emailService.SendEmailAsync(
                    dto.Email,
                    "[Library] Cấp tài khoản Thủ thư",
                    $"<p>Xin chào <strong>{dto.FullName}</strong>,</p>" +
                    $"<p>Tài khoản thủ thư của bạn trên hệ thống Quản lý Thư viện đã được khởi tạo thành công:</p>" +
                    $"<ul>" +
                    $"<li><strong>Email đăng nhập:</strong> {dto.Email}</li>" +
                    $"<li><strong>Mật khẩu tạm thời:</strong> <strong>{password}</strong></li>" +
                    $"</ul>" +
                    $"<p>Vui lòng đăng nhập và tiến hành đổi mật khẩu ngay để bảo mật thông tin.</p>"
                );
            }
            catch (Exception)
            {
                // Silently bypass email failures so creation doesn't crash on dev environment
            }

            return new CreateUserResponseDto
            {
                Email = dto.Email,
                Password = password
            };
        }

        public async Task<CreateUserResponseDto> CreateReaderAsync(CreateReaderDto dto)
        {
            var existsAccount = await _unitOfWork.AccountRepository.GetAccountByEmailAsync(dto.Email);
            var existsReader = await _unitOfWork.ReaderRepository.GetReaderByEmailAsync(dto.Email);
            if (existsAccount != null || existsReader != null)
            {
                throw new Exception("Email đã tồn tại trong hệ thống.");
            }

            var password = GenerateRandomPassword();
            var reader = new Reader
            {
                ReaderId = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            var profile = new UserProfile
            {
                UserProfileId = Guid.NewGuid(),
                ReaderId = reader.ReaderId,
                FullName = dto.FullName,
                Phone = dto.Phone,
                Address = dto.Address,
                DateOfBirth = dto.DateOfBirth
            };

            await _unitOfWork.Readers.AddAsync(reader);
            await _unitOfWork.UserProfiles.AddAsync(profile);
            await _unitOfWork.SaveChangesAsync();

            // Send Email
            try
            {
                await _emailService.SendEmailAsync(
                    dto.Email,
                    "[Library] Cấp tài khoản Độc giả",
                    $"<p>Xin chào <strong>{dto.FullName}</strong>,</p>" +
                    $"<p>Tài khoản độc giả của bạn đã được đăng ký thành công trên hệ thống:</p>" +
                    $"<ul>" +
                    $"<li><strong>Email đăng nhập:</strong> {dto.Email}</li>" +
                    $"<li><strong>Mật khẩu tạm thời:</strong> <strong>{password}</strong></li>" +
                    $"</ul>" +
                    $"<p>Vui lòng sử dụng thông tin này để đăng nhập vào hệ thống thư viện.</p>"
                );
            }
            catch (Exception)
            {
                // Silently bypass email failures
            }

            return new CreateUserResponseDto
            {
                Email = dto.Email,
                Password = password
            };
        }

        public async Task<bool> ToggleLibrarianStatusAsync(Guid id)
        {
            var account = await _unitOfWork.Accounts.GetByIdAsync(id);
            if (account == null || account.Role != "Librarian")
                return false;

            account.Status = (account.Status == "Active") ? "Inactive" : "Active";
            account.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Accounts.Update(account);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleReaderStatusAsync(Guid id)
        {
            var reader = await _unitOfWork.Readers.GetByIdAsync(id);
            if (reader == null)
                return false;

            reader.Status = (reader.Status == "Active") ? "Suspended" : "Active";
            reader.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Readers.Update(reader);
            await _unitOfWork.SaveChangesAsync();
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
    }
}
