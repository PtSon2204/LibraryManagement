namespace LibraryManagement.Business.DTOs.BookDTOs;

public class BookDetailDto
{
    public Guid BookId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? ISBN { get; set; }

    public string? Description { get; set; }

    public string? CoverImageUrl { get; set; }

    public string? PublisherName { get; set; }

    public int? PublicationYear { get; set; }

    public string? Language { get; set; }

    public string? Edition { get; set; }

    public List<string> Authors { get; set; } = new();

    public List<string> Categories { get; set; } = new();

    public int TotalCopies { get; set; }

    public int AvailableCopies { get; set; }

    public List<BookCopyDto> Copies { get; set; } = new();
}
