namespace LibraryManagement.Business.DTOs.BookDTOs
{
    public class CreateBookDto
    {
        public string Title { get; set; } = null!;
        public string? ISBN { get; set; }
        public int? PublisherId { get; set; }
        public int? PublicationYear { get; set; }
        public string? Language { get; set; }
        public string? Edition { get; set; }
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public System.Collections.Generic.List<int>? AuthorIds { get; set; } = new();
        public System.Collections.Generic.List<int>? CategoryIds { get; set; } = new();
    }
}
