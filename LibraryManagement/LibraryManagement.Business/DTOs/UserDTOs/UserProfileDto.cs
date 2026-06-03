using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Business.DTOs.UserDTOs
{
    public class UserProfileDto
    {
        public Guid UserId { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public DateOnly? DateOfBirth { get; set; }
    }
}
