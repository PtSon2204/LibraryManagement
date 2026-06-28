namespace LibraryManagement.MVC.ViewModels.Books;

public class BookLocationAvailabilityViewModel
{
    public string Location { get; set; } = string.Empty;

    public int TotalCopies { get; set; }

    public int AvailableCopies { get; set; }
}
