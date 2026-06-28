using LibraryManagement.MVC.ViewModels.BookCopies;
using System;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Interface
{
    public interface IBookCopyService
    {
        Task<BookCopyListViewModel?> GetBookCopiesAsync(Guid bookId, string? searchTerm, string? status,
                                                        string? location, int pageNumber, int pageSize);
        Task<BookCopyViewModel?> GetBookCopyByIdAsync(Guid id);
        Task<string?> CreateBookCopyAsync(CreateBookCopyViewModel model);
        Task<string?> GenerateBookCopiesAsync(GenerateBookCopiesViewModel model);
        Task<string?> UpdateBookCopyAsync(UpdateBookCopyViewModel model);
        Task<bool> ToggleHideAsync(Guid id);
        Task<bool> DeleteBookCopyAsync(Guid id);
    }
}
