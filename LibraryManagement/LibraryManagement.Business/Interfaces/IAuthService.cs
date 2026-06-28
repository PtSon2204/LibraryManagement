using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.AuthDTOs;

namespace LibraryManagement.Business.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDto dto);

        Task<LoginResponseDto> LoginAsync(LoginDto dto);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
    }
}
