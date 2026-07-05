using System;

namespace LibraryManagement.Business.DTOs.ReservationDTOs
{
    public class AvailableSlotDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public string? Reason { get; set; } // e.g. "Past", "Booked"
    }
}
