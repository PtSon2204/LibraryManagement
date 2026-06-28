using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.MVC.ViewModels.Category
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Tên thể loại không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên thể loại không được vượt quá 100 ký tự.")]
        public string CategoryName { get; set; } = null!;
    }

    public class CategoryListViewModel
    {
        public List<CategoryViewModel> Data { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public string? Search { get; set; }
    }
}
