using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.MVC.ViewModels.Books;

public class BookCreateViewModel
{
    [Required(ErrorMessage = "Tên sách không được để trống")]
    [MaxLength(255, ErrorMessage = "Tên sách tối đa 255 ký tự")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(20, ErrorMessage = "ISBN tối đa 20 ký tự")]
    public string? ISBN { get; set; }

    [Range(0, 9999, ErrorMessage = "Năm xuất bản không hợp lệ")]
    public int? PublicationYear { get; set; }

    [MaxLength(50, ErrorMessage = "Ngôn ngữ tối đa 50 ký tự")]
    public string? Language { get; set; }

    [MaxLength(100, ErrorMessage = "Phiên bản tối đa 100 ký tự")]
    public string? Edition { get; set; }

    public string? Description { get; set; }

    [MaxLength(1000, ErrorMessage = "URL ảnh bìa tối đa 1000 ký tự")]
    public string? CoverImageUrl { get; set; }
}
