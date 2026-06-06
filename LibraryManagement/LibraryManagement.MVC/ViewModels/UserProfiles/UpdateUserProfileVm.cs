namespace LibraryManagement.MVC.ViewModels.UserProfiles
{
    public class UpdateUserProfileVm
    {
        public string FullName { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public DateOnly? DateOfBirth { get; set; }
    }
}
