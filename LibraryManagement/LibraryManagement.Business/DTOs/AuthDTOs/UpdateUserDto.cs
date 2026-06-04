using System;

namespace LibraryManagement.Business.DTOs.AuthDTOs
{
    public class UpdateUserDto
    {
        public int RoleId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string Status { get; set; } = null!;
    }
}
