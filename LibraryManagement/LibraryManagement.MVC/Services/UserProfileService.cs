using LibraryManagement.MVC.Interface;
using LibraryManagement.MVC.ViewModels.UserProfiles;

namespace LibraryManagement.MVC.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly HttpClient _httpClient;
        public UserProfileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }



        public async Task<UserProfileViewModel> GetProfile()
        {
            var response = await _httpClient.GetAsync("api/profile");


            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<UserProfileViewModel>();
        }

        public async Task<bool> UpdateProfile(UpdateUserProfileVm model)
        {
            var response = await _httpClient.PutAsJsonAsync("api/profile", model);

            return response.IsSuccessStatusCode;
        }
    }
}
