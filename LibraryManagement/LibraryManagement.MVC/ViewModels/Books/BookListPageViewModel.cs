namespace LibraryManagement.MVC.ViewModels.Books;

public class BookListPageViewModel
{
    public BookSearchViewModel Search { get; set; } = new();

    public List<BookListItemViewModel> Books { get; set; } = new();

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}
