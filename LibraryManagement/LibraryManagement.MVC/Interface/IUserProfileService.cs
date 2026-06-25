using LibraryManagement.MVC.ViewModels.UserProfiles;

namespace LibraryManagement.MVC.Interface
{
    public interface IUserProfileService
    {
        Task<UserProfileViewModel> GetProfile();
        Task<bool> UpdateProfile(UpdateUserProfileVm model);
    }
}
