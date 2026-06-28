using System;

namespace LibraryManagement.Business.DTOs.RoomDTOs
{
    public class RoomDto
    {
        public Guid RoomId { get; set; }
        public string RoomName { get; set; } = null!;
        public int Capacity { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
