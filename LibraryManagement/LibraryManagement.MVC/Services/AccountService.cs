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
    }
}
