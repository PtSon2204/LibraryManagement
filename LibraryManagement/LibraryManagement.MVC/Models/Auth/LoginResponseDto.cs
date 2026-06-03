namespace LibraryManagement.MVC.Models.Auth
{
    public class LoginResponseDto
    {
        public Guid UserId { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string Role { get; set; }
    }
}
