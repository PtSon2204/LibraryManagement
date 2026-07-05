using System;

namespace LibraryManagement.Business.DTOs.ReservationDTOs
{
    public class CreateReservationDto
    {
        public Guid RoomId { get; set; }
        public Guid ReaderId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
