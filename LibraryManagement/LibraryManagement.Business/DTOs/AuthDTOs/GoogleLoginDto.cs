namespace LibraryManagement.Business.DTOs.AuthDTOs
{
    public class GoogleLoginDto
    {
        public string Email { get; set; } = null!;

        public string? FullName { get; set; }
    }
}
