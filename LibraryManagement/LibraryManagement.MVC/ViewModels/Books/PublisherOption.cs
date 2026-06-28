namespace LibraryManagement.MVC.ViewModels.Books
{
    /// <summary>Option hiển thị trong dropdown chọn Nhà xuất bản</summary>
    public class PublisherOption
    {
        public int PublisherId { get; set; }
        public string PublisherName { get; set; } = null!;
    }
}
