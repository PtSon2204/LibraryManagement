using System.Collections.Generic;

namespace LibraryManagement.MVC.ViewModels.Ai
{
    public class ChatMessageViewModel
    {
        public string Role { get; set; } = string.Empty; // "user" or "model"
        public string Content { get; set; } = string.Empty;
    }

    public class ChatRequestViewModel
    {
        public string Prompt { get; set; } = string.Empty;
        public List<ChatMessageViewModel> History { get; set; } = new();
    }
}
