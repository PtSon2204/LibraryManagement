using System;

namespace LibraryManagement.Business.DTOs
{
    public class AuthorDto
    {
        public int AuthorId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Biography { get; set; }
    }
}
