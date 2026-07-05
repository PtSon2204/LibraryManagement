using System;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Interface
{
    public interface IRoomSlotLockService
    {
        Task<string> GetLocksByRoomAndDateAsync(Guid roomId, DateTime date);
        Task<bool> LockSlotAsync(object payload);
        Task<bool> UnlockSlotAsync(int id);
    }
}
