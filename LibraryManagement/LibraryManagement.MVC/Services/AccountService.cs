using LibraryManagement.MVC.Common;
using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.Auth;

namespace LibraryManagement.MVC.Services
{
    public class AccountService : IAccountService
    {
        private readonly HttpClient _httpClient;

        public AccountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> ForgotPasswordAsync(string email)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/forgot-password", new { Email = email });
            if (response.IsSuccessStatusCode)
                return null;
            
            var error = await response.Content.ReadAsStringAsync();
            return !string.IsNullOrEmpty(error) ? error : "Lỗi hệ thống khi gửi email";
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginViewModel model)
        {
            var response =
           await _httpClient.PostAsJsonAsync("api/auth/login", model);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        }

        public async Task<ValidationErrorResponse?> RegisterAsync(RegisterViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/auth/register", model);

            if (response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<ValidationErrorResponse>();
        }

        public async Task<string?> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/change-password", new
            {
                CurrentPassword = model.CurrentPassword,
                NewPassword = model.NewPassword,
                ConfirmNewPassword = model.ConfirmNewPassword
            });

            if (response.IsSuccessStatusCode)
                return null;

            var error = await response.Content.ReadAsStringAsync();
            return !string.IsNullOrEmpty(error) ? error : "Lỗi hệ thống khi đổi mật khẩu";
        }
    }
}
