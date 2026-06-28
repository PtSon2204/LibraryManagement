namespace LibraryManagement.Business.DTOs.PublisherDTOs
{
    public class UpdatePublisherDto
    {
        public int PublisherId { get; set; }
        public string PublisherName { get; set; } = null!;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
