using System.ComponentModel.DataAnnotations;

namespace StudentManagementApi.Domain
{
    public class QuestionOption
    {
        public int Id { get; set; }

        [Required]
        public int QuestionId { get; set; }
        public Question Question { get; set; } = default!;

        [Required]
        public string Text { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }
    }
}
