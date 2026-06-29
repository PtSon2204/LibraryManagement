using System;

namespace LibraryManagement.Business.DTOs.AuthorDTOs
{
    public class UpdateAuthorDto
    {
        public int AuthorId { get; set; }
        public string FullName { get; set; } = null!;
    }
}
