using LibraryManagement.MVC.Models.Auth;

namespace LibraryManagement.MVC.Interface
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterViewModel model);

        Task<LoginResponseDto?> LoginAsync(LoginViewModel model);
    }
}
