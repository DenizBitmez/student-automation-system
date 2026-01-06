using System.ComponentModel.DataAnnotations;
using StudentManagementApi.Domain;

namespace StudentManagementApi.Domain
{
    public class Submission
    {
        public int Id { get; set; }

        [Required]
        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = default!;

        [Required]
        public string StudentId { get; set; } = string.Empty;
        public ApplicationUser Student { get; set; } = default!;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public int? Grade { get; set; }
        public string? Feedback { get; set; }
    }
}
