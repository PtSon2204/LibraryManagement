using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Business.DTOs.AuthDTOs
{
    public class LoginResponseDto
    {
        public Guid UserId { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public string Role { get; set; }
    }
}
