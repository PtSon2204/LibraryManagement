using LibraryManagement.Business.DTOs.PublisherDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.Common;
using LibraryManagement.Data.UnitOfWorks;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Business.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PublisherService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<PublisherDto>> GetPublishersAsync(string? search, int pageNumber, int pageSize)
        {
            var query = _unitOfWork.Publishers.Query();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p =>
                    p.PublisherName.Contains(search) ||
                    (p.Email != null && p.Email.Contains(search)) ||
                    (p.Phone != null && p.Phone.Contains(search)) ||
                    (p.Address != null && p.Address.Contains(search)));

            var total = await query.CountAsync();

            var data = await query
                .OrderBy(p => p.PublisherName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PublisherDto
                {
                    PublisherId = p.PublisherId,
                    PublisherName = p.PublisherName,
                    Address = p.Address,
                    Phone = p.Phone,
                    Email = p.Email
                })
                .ToListAsync();

            return new PagedResult<PublisherDto>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = total,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize)
            };
        }

        public async Task<PublisherDto?> GetPublisherByIdAsync(int id)
        {
            var publisher = await _unitOfWork.Publishers.GetByIdAsync(id);
            if (publisher == null) return null;

            return new PublisherDto
            {
                PublisherId = publisher.PublisherId,
                PublisherName = publisher.PublisherName,
                Address = publisher.Address,
                Phone = publisher.Phone,
                Email = publisher.Email
            };
        }

        public async Task<PublisherDto> CreatePublisherAsync(CreatePublisherDto dto)
        {
            var publisher = new LibraryManagement.Models.Models.Publisher
            {
                PublisherName = dto.PublisherName,
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.Email
            };

            await _unitOfWork.Publishers.AddAsync(publisher);
            await _unitOfWork.SaveChangesAsync();

            return new PublisherDto
            {
                PublisherId = publisher.PublisherId,
                PublisherName = publisher.PublisherName,
                Address = publisher.Address,
                Phone = publisher.Phone,
                Email = publisher.Email
            };
        }

        public async Task<PublisherDto?> UpdatePublisherAsync(int id, UpdatePublisherDto dto)
        {
            var publisher = await _unitOfWork.Publishers.GetByIdAsync(id);
            if (publisher == null) return null;

            publisher.PublisherName = dto.PublisherName;
            publisher.Address = dto.Address;
            publisher.Phone = dto.Phone;
            publisher.Email = dto.Email;

            _unitOfWork.Publishers.Update(publisher);
            await _unitOfWork.SaveChangesAsync();

            return new PublisherDto
            {
                PublisherId = publisher.PublisherId,
                PublisherName = publisher.PublisherName,
                Address = publisher.Address,
                Phone = publisher.Phone,
                Email = publisher.Email
            };
        }

        public async Task<bool> DeletePublisherAsync(int id)
        {
            var publisher = await _unitOfWork.Publishers.GetByIdAsync(id);
            if (publisher == null) return false;

            _unitOfWork.Publishers.Delete(publisher);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
