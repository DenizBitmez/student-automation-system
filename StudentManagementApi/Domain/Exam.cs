using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementApi.Domain
{
    public class Exam
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; } = default!;

        public int DurationMinutes { get; set; } // 0 for unlimited

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<ExamResult> Results { get; set; } = new List<ExamResult>();
    }
}
