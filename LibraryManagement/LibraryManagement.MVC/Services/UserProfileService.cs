using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.UserProfiles;

namespace LibraryManagement.MVC.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserProfileService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddJwt()
        {
            var token = _httpContextAccessor.HttpContext!.Session.GetString("AccessToken");

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<UserProfileViewModel> GetProfile()
        {
            AddJwt();
            var response = await _httpClient.GetAsync("api/profile");


            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<UserProfileViewModel>();
        }

        public async Task<bool> UpdateProfile(UpdateUserProfileVm model)
        {
            AddJwt();

            var response = await _httpClient.PutAsJsonAsync("api/profile", model);

            return response.IsSuccessStatusCode;
        }
    }
}
