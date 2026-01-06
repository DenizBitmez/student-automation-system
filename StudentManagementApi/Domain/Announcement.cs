using System.ComponentModel.DataAnnotations;
using StudentManagementApi.Domain;

namespace StudentManagementApi.Domain
{
    public class Announcement
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string AuthorId { get; set; } = string.Empty;
        public ApplicationUser Author { get; set; } = default!;
    }
}
