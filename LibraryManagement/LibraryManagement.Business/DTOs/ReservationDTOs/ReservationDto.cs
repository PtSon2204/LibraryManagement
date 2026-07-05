using System;

namespace LibraryManagement.Business.DTOs.ReservationDTOs
{
    public class ReservationDto
    {
        public Guid ReservationId { get; set; }
        public Guid ReaderId { get; set; }
        public string ReaderName { get; set; } = string.Empty;
        public Guid RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime ReservationDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? ActualCheckInTime { get; set; }
        public bool IsNoShow { get; set; }
    }
}
