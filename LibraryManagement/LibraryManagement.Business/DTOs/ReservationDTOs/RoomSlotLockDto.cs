using System;

namespace LibraryManagement.Business.DTOs.ReservationDTOs
{
    public class RoomSlotLockDto
    {
        public int RoomSlotLockId { get; set; }
        public Guid RoomId { get; set; }
        public DateTime LockDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Reason { get; set; }
        public Guid? LockedByUserId { get; set; }
        public string? LockedByUserName { get; set; }
    }

    public class CreateRoomSlotLockDto
    {
        public Guid RoomId { get; set; }
        public DateTime LockDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Reason { get; set; }
    }
}
