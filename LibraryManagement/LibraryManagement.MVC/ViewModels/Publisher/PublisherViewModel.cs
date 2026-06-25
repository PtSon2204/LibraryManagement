namespace LibraryManagement.MVC.ViewModels.Publisher
{
    public class PublisherViewModel
    {
        public int PublisherId { get; set; }
        public string PublisherName { get; set; } = null!;
        public string? Address { get; set; }
        public string? Phone { get; set; }
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
