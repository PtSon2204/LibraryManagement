namespace LibraryManagement.Business.DTOs.AiDTOs
{
    public class ChatMessageDto
    {
        public string Role { get; set; } = string.Empty; // "user" or "model"
        public string Content { get; set; } = string.Empty;
    }

    public class ChatRequestDto
    {
        public string Prompt { get; set; } = string.Empty;
        public List<ChatMessageDto> History { get; set; } = new();
    }
}
