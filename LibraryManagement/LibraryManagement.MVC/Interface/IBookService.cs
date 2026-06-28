using LibraryManagement.MVC.ViewModels.Books;

namespace LibraryManagement.MVC.Interface;

public interface IBookService
{
    Task<BookListPageViewModel?> GetBooksAsync(BookSearchViewModel search);

    Task<List<BookListItemViewModel>> GetLatestBooksAsync(int count);

    Task<BookDetailViewModel?> GetBookDetailAsync(Guid bookId);
}
