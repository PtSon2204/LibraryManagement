using LibraryManagement.Business.DTOs.ReservationDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement.Business.Interfaces
{
    public interface IRoomSlotLockService
    {
        Task<List<RoomSlotLockDto>> GetLocksByRoomAndDateAsync(Guid roomId, DateTime date);
        Task<RoomSlotLockDto> LockSlotAsync(CreateRoomSlotLockDto dto, Guid lockedByUserId);
        Task<bool> UnlockSlotAsync(int roomSlotLockId);
    }
}
