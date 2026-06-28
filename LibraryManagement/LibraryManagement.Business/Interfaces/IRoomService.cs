using LibraryManagement.Business.DTOs.RoomDTOs;
using LibraryManagement.Data.Common;
using System;
using System.Threading.Tasks;

namespace LibraryManagement.Business.Interfaces
{
    public interface IRoomService
    {
        Task<PagedResult<RoomDto>> GetRoomsAsync(string? search, string? status, int pageNumber, int pageSize);
        Task<RoomDto?> GetRoomByIdAsync(Guid id);
        Task<RoomDto> CreateRoomAsync(CreateRoomDto dto);
        Task<RoomDto?> UpdateRoomAsync(Guid id, UpdateRoomDto dto);
        Task<bool> DeleteRoomAsync(Guid id);
    }
}
