using LibraryManagement.MVC.Common;
using LibraryManagement.MVC.ViewModels.Auth;

namespace LibraryManagement.MVC.Interface
{
    public interface IAccountService
    {
        Task<ValidationErrorResponse?> RegisterAsync(RegisterViewModel model);
        Task<LoginResponseDto?> LoginAsync(LoginViewModel model);
        Task<string?> ForgotPasswordAsync(string email);
        Task<string?> ChangePasswordAsync(ChangePasswordViewModel model);
    }
}
