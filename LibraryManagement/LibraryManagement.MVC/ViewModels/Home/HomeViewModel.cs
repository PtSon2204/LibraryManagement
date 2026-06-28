using LibraryManagement.MVC.ViewModels.Books;

namespace LibraryManagement.MVC.ViewModels.Home;

public class HomeViewModel
{
    public List<BookListItemViewModel> LatestBooks { get; set; } = new();
}
