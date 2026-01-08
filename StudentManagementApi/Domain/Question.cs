using System.ComponentModel.DataAnnotations;

namespace StudentManagementApi.Domain
{
    public enum QuestionType
    {
        MultipleChoice,
        TrueFalse,
        OpenEnded
    }

    public class Question
    {
        public int Id { get; set; }

        [Required]
        public int ExamId { get; set; }
        public Exam Exam { get; set; } = default!;

        [Required]
        public string Text { get; set; } = string.Empty;

        public int Points { get; set; }

        public QuestionType Type { get; set; }

        public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    }
}
