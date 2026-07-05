using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Business.DTOs.ReservationDTOs;
using LibraryManagement.Data.Common;

namespace LibraryManagement.Business.Interfaces
{
    public interface IReservationService
    {
        Task<List<AvailableSlotDto>> GetAvailableSlotsAsync(Guid roomId, DateTime date);
        Task<ReservationDto> CreateReservationAsync(CreateReservationDto dto);
        Task<bool> CheckInAsync(Guid reservationId);
        Task<PagedResult<ReservationDto>> GetReservationsAsync(int pageNumber, int pageSize, Guid? readerId = null, string? status = null);
        Task<bool> CancelReservationAsync(Guid reservationId, Guid? readerId = null);
    }
}
