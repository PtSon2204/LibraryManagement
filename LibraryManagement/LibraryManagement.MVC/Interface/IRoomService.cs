using LibraryManagement.MVC.ViewModels.Room;
using System;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Interface
{
    public interface IRoomService
    {
        Task<RoomListViewModel?> GetRoomsAsync(string? search, string? status, int pageNumber, int pageSize);
        Task<RoomViewModel?> GetRoomByIdAsync(Guid id);
        Task<string?> CreateRoomAsync(RoomViewModel model);
        Task<string?> UpdateRoomAsync(RoomViewModel model);
        Task<bool> DeleteRoomAsync(Guid id);
    }
}
