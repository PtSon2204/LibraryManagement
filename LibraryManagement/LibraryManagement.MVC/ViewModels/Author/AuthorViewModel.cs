using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.MVC.ViewModels.Author
{
    public class AuthorViewModel
    {
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Tên tác giả không được để trống.")]
        [StringLength(255, ErrorMessage = "Tên tác giả không được vượt quá 255 ký tự.")]
        [Display(Name = "Tên tác giả")]
        public string FullName { get; set; } = null!;
    }
}
