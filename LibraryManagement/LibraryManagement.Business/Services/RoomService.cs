using LibraryManagement.Business.DTOs.RoomDTOs;
using LibraryManagement.Business.Interfaces;
using LibraryManagement.Data.Common;
using LibraryManagement.Data.UnitOfWorks;
using LibraryManagement.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Business.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<RoomDto>> GetRoomsAsync(string? search, string? status, int pageNumber, int pageSize)
        {
            var query = _unitOfWork.Rooms.Query();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var cleanSearch = search.Trim().ToLower();
                query = query.Where(r => r.RoomName.ToLower().Contains(cleanSearch) || 
                                         (r.Description != null && r.Description.ToLower().Contains(cleanSearch)));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var cleanStatus = status.Trim().ToLower();
                query = query.Where(r => r.Status.ToLower() == cleanStatus);
            }

            query = query.OrderBy(r => r.RoomName);

            var totalRecords = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RoomDto
                {
                    RoomId = r.RoomId,
                    RoomName = r.RoomName,
                    Capacity = r.Capacity,
                    Description = r.Description,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return new PagedResult<RoomDto>
            {
                Data = items,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
            };
        }

        public async Task<RoomDto?> GetRoomByIdAsync(Guid id)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (room == null) return null;

            return new RoomDto
            {
                RoomId = room.RoomId,
                RoomName = room.RoomName,
                Capacity = room.Capacity,
                Description = room.Description,
                Status = room.Status,
                CreatedAt = room.CreatedAt
            };
        }

        public async Task<RoomDto> CreateRoomAsync(CreateRoomDto dto)
        {
            var room = new Room
            {
                RoomId = Guid.NewGuid(), // Will be generated if newsequentialid but setting it explicitly is safe
                RoomName = dto.RoomName.Trim(),
                Capacity = dto.Capacity,
                Description = dto.Description?.Trim(),
                Status = dto.Status.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Rooms.AddAsync(room);
            await _unitOfWork.SaveChangesAsync();

            return new RoomDto
            {
                RoomId = room.RoomId,
                RoomName = room.RoomName,
                Capacity = room.Capacity,
                Description = room.Description,
                Status = room.Status,
                CreatedAt = room.CreatedAt
            };
        }

        public async Task<RoomDto?> UpdateRoomAsync(Guid id, UpdateRoomDto dto)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (room == null) return null;

            room.RoomName = dto.RoomName.Trim();
            room.Capacity = dto.Capacity;
            room.Description = dto.Description?.Trim();
            room.Status = dto.Status.Trim();

            _unitOfWork.Rooms.Update(room);
            await _unitOfWork.SaveChangesAsync();

            return new RoomDto
            {
                RoomId = room.RoomId,
                RoomName = room.RoomName,
                Capacity = room.Capacity,
                Description = room.Description,
                Status = room.Status,
                CreatedAt = room.CreatedAt
            };
        }

        public async Task<bool> DeleteRoomAsync(Guid id)
        {
            var room = await _unitOfWork.Rooms.GetByIdAsync(id);
            if (room == null) return false;

            _unitOfWork.Rooms.Delete(room);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
