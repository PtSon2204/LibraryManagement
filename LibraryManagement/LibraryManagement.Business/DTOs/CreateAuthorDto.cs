using System;

namespace LibraryManagement.Business.DTOs
{
    public class CreateAuthorDto
    {
        public string FullName { get; set; } = null!;
        public string? Biography { get; set; }
    }
}
