using LibraryManagement.MVC.ViewModels.Books;

namespace LibraryManagement.MVC.Interface;

public interface IBookService
{
    Task<BookListPageViewModel?> GetBooksAsync(BookSearchViewModel search);

    Task<List<BookListItemViewModel>> GetLatestBooksAsync(int count);

    Task<BookDetailViewModel?> GetBookDetailAsync(Guid bookId);

    Task<BookListViewModel?> GetBooksAsync(string? searchTerm, int? publisherId, int? publicationYear, string? language, int pageNumber, int pageSize);

    Task<BookViewModel?> GetBookByIdAsync(Guid id);

    Task<string?> CreateBookAsync(CreateBookViewModel model);

    Task<string?> UpdateBookAsync(UpdateBookViewModel model);

    Task<bool> ToggleHideBookAsync(Guid id);

    Task<bool> DeleteBookAsync(Guid id);
}
