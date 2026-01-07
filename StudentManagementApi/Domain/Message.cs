using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StudentManagementApi.Domain;

namespace StudentManagementApi.Domain
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string SenderId { get; set; } = string.Empty;
        
        [ForeignKey("SenderId")]
        public virtual ApplicationUser? Sender { get; set; }

        [Required]
        public string ReceiverId { get; set; } = string.Empty;

        [ForeignKey("ReceiverId")]
        public virtual ApplicationUser? Receiver { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;
    }
}
