using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryManagement.MVC.Interface
{
    public interface IReservationService
    {
        Task<string> GetAvailableSlotsAsync(Guid roomId, DateTime date);
        Task<bool> CreateReservationAsync(object payload);
        Task<string> GetReservationsAsync(int pageNumber, int pageSize, string status, Guid? readerId = null);
        Task<bool> CheckInAsync(Guid id);
        Task<bool> CancelReservationAsync(Guid id, Guid? readerId = null);
    }
}
