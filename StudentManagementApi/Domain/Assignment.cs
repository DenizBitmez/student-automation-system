using System.ComponentModel.DataAnnotations;
using StudentManagementApi.Domain;

namespace StudentManagementApi.Domain
{
    public class Assignment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime DueDate { get; set; }

        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; } = default!;

        [Required]
        public string TeacherId { get; set; } = string.Empty;
        public ApplicationUser Teacher { get; set; } = default!;

        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    }
}
