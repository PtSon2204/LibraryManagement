namespace LibraryManagement.Business.DTOs.RoomDTOs
{
    public class CreateRoomDto
    {
        public string RoomName { get; set; } = null!;
        public int Capacity { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "Available";
    }
}
