using System;

namespace LibraryManagement.Business.DTOs.BookDTOs
{
    public class UpdateBookDto
    {
        public Guid BookId { get; set; }
        public string Title { get; set; } = null!;
        public string? ISBN { get; set; }
        public int? PublisherId { get; set; }
        public int? PublicationYear { get; set; }
        public string? Language { get; set; }
        public string? Edition { get; set; }
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
    }
}
