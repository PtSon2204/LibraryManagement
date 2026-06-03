using LibraryManagement.MVC.Services.API.Common;
using LibraryManagement.MVC.ViewModels.Books;

namespace LibraryManagement.MVC.Interface.API.Books
{
    public interface IBookApiClient
    {
        Task<ODataResponse<BookListItemViewModel>> GetBooksAsync(BookSearchViewModel search);

        Task<List<BookListItemViewModel>> GetLatestBooksAsync(int count = 6);

        Task<BookDetailViewModel?> GetBookDetailAsync(Guid id);

        Task<BookDetailViewModel?> AddBookAsync(BookCreateViewModel book);
    }
}
