using System.ComponentModel.DataAnnotations;

namespace StudentManagementApi.Domain
{
    public class ExamResult
    {
        public int Id { get; set; }

        [Required]
        public int ExamId { get; set; }
        public Exam Exam { get; set; } = default!;

        [Required]
        public int StudentId { get; set; }
        public Student Student { get; set; } = default!;

        public int Score { get; set; }

        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }

        public ICollection<StudentAnswer> Answers { get; set; } = new List<StudentAnswer>();
    }
}
