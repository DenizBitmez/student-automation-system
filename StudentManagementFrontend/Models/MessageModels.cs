using System.ComponentModel.DataAnnotations;

namespace StudentManagementFrontend.Models
{
    public class SendMessageDto
    {
        [Required(ErrorMessage = "Receiver is required")]
        public string ReceiverId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;
    }

    public class MessageDto
    {
        public int Id { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        public bool IsMe { get; set; } 
    }

    public class ConversationUserDto
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int UnreadCount { get; set; }
        public DateTime? LastMessageTime { get; set; }
    }
}
