using LibraryManagement.Business.DTOs.UserManagementDTOs;
using LibraryManagement.Data.Common;

namespace LibraryManagement.MVC.ViewModels.Admin
{
    public class AdminUsersViewModel
    {
        public PagedResult<LibrarianListItemDto> Librarians { get; set; } = null!;
        public PagedResult<ReaderListItemDto> Readers { get; set; } = null!;
        public string? SearchLibrarians { get; set; }
        public string? SearchReaders { get; set; }
    }
}
