using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.MVC.ViewModels.Publisher
{
    public class PublisherViewModel
    {
        public int PublisherId { get; set; }

        [Required(ErrorMessage = "Tên nhà xuất bản không được để trống.")]
        [StringLength(255, ErrorMessage = "Tên nhà xuất bản không được vượt quá 255 ký tự.")]
        public string PublisherName { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Địa chỉ không được vượt quá 500 ký tự.")]
        public string? Address { get; set; }

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự.")]
        [RegularExpression(@"^\+?[0-9\s-]*$", ErrorMessage = "Số điện thoại không đúng định dạng.")]
        public string? Phone { get; set; }

        [StringLength(255, ErrorMessage = "Email không được vượt quá 255 ký tự.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string? Email { get; set; }
    }

    public class PublisherListViewModel
    {
        public List<PublisherViewModel> Data { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public string? Search { get; set; }
    }
}
