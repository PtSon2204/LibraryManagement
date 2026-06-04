using System;

namespace LibraryManagement.Business.DTOs.AuthDTOs
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
