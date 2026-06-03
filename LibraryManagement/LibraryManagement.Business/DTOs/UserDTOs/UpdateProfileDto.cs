using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Business.DTOs.UserDTOs
{
    public class UpdateProfileDto
    {
        public string FullName { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public DateOnly? DateOfBirth { get; set; }
    }
}
