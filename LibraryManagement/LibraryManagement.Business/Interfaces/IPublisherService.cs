using LibraryManagement.Business.DTOs.PublisherDTOs;
using LibraryManagement.Data.Common;

namespace LibraryManagement.Business.Interfaces
{
    public interface IPublisherService
    {
        Task<PagedResult<PublisherDto>> GetPublishersAsync(string? search, int pageNumber, int pageSize);
        Task<PublisherDto?> GetPublisherByIdAsync(int id);
        Task<PublisherDto> CreatePublisherAsync(CreatePublisherDto dto);
        Task<PublisherDto?> UpdatePublisherAsync(int id, UpdatePublisherDto dto);
        Task<bool> DeletePublisherAsync(int id);
    }
}
