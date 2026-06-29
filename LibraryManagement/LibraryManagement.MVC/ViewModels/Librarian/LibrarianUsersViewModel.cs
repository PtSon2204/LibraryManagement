using LibraryManagement.Business.DTOs.UserManagementDTOs;
using LibraryManagement.Data.Common;

namespace LibraryManagement.MVC.ViewModels.Librarian
{
    public class LibrarianUsersViewModel
    {
        public PagedResult<ReaderListItemDto> Readers { get; set; } = null!;
        public string? Search { get; set; }
    }
}
