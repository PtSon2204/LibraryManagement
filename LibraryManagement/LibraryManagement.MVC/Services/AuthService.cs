using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.Models.Auth;

namespace LibraryManagement.MVC.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
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

        public async Task<bool> RegisterAsync(RegisterViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register",model);
            var content = await response.Content.ReadAsStringAsync();

            Console.WriteLine(content);
            Console.WriteLine(content);
            return response.IsSuccessStatusCode;
        }
    }
}
