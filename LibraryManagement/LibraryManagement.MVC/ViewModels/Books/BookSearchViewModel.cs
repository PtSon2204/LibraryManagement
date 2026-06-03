namespace LibraryManagement.MVC.ViewModels.Books;

public class BookSearchViewModel
{
    public string? Title { get; set; }

    public string? Language { get; set; }

    public string? Publisher { get; set; }

    public bool AvailableOnly { get; set; }

    public string? SortBy { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
