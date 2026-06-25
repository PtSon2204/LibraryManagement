using LibraryManagement.MVC.ViewModels.Publisher;

namespace LibraryManagement.MVC.Interface
{
    public interface IPublisherService
    {
        Task<PublisherListViewModel?> GetPublishersAsync(string? search, int pageNumber, int pageSize);
        Task<PublisherViewModel?> GetPublisherByIdAsync(int id);
    }
}
