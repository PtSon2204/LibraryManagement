using LibraryManagement.MVC.ViewModels.Books;

namespace LibraryManagement.MVC.Interface
{
    public interface IBookService
    {
        Task<BookListViewModel?> GetBooksAsync(string? searchTerm, int? publisherId, int? publicationYear, string? language, int pageNumber, int pageSize);
        Task<BookViewModel?> GetBookByIdAsync(Guid id);

        /// <summary>Tạo sách mới. Trả về null nếu thành công, chuỗi lỗi nếu thất bại.</summary>
        Task<string?> CreateBookAsync(CreateBookViewModel model);

        /// <summary>Cập nhật sách. Trả về null nếu thành công, chuỗi lỗi nếu thất bại.</summary>
        Task<string?> UpdateBookAsync(UpdateBookViewModel model);
        Task<bool> ToggleHideBookAsync(Guid id);
        Task<bool> DeleteBookAsync(Guid id);
    }
}
