namespace LibraryManagement.Business.DTOs.BookDTOs;

public class BookLocationAvailabilityDto
{
    public string Location { get; set; } = string.Empty;

    public int TotalCopies { get; set; }

    public int AvailableCopies { get; set; }
}
