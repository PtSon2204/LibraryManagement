using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Business.DTOs.UserProfileDTOs
{
    public class UpdateUserProfileDto
    {
        public string FullName { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public DateOnly? DateOfBirth { get; set; }
    }
}
