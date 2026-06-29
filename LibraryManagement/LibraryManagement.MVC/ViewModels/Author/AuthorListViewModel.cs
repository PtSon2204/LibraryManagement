using System.Collections.Generic;

namespace LibraryManagement.MVC.ViewModels.Author
{
    public class AuthorListViewModel
    {
        public List<AuthorViewModel> Data { get; set; } = new List<AuthorViewModel>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public string? Search { get; set; }
    }
}
