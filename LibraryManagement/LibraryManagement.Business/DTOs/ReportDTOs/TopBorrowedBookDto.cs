namespace LibraryManagement.Business.DTOs.ReportDTOs;

public class TopBorrowedBookDto
{
    public Guid BookId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? ISBN { get; set; }

    public int BorrowCount { get; set; }
}
