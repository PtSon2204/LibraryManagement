namespace LibraryManagement.Business.DTOs.BookDTOs;

public class BookListItemDto
{
    public Guid BookId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? ISBN { get; set; }

    public string? PublisherName { get; set; }

    public int? PublicationYear { get; set; }

    public string? Language { get; set; }

    public string? CoverImageUrl { get; set; }

    public int TotalCopies { get; set; }

    public int AvailableCopies { get; set; }
}
