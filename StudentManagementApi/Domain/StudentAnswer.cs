using System.ComponentModel.DataAnnotations;

namespace StudentManagementApi.Domain
{
    public class StudentAnswer
    {
        public int Id { get; set; }

        [Required]
        public int ExamResultId { get; set; }
        public ExamResult ExamResult { get; set; } = default!;

        [Required]
        public int QuestionId { get; set; }
        public Question Question { get; set; } = default!;

        public int? SelectedOptionId { get; set; }
        public QuestionOption? SelectedOption { get; set; }

        public string? TextAnswer { get; set; }
    }
}
