namespace LibraryManagement.Business.DTOs.BookDTOs;

public class BookListPageDto
{
    public List<BookListItemDto> Books { get; set; } = new();

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}
