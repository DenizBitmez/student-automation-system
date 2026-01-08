using System;
using System.Collections.Generic;

namespace StudentManagementFrontend.Models
{
    public class ExamDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public bool CanTake { get; set; }
        public int? Score { get; set; }
    }

    public class ExamCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; } = DateTime.Now.AddDays(1);
        public List<QuestionCreateDto> Questions { get; set; } = new();
    }

    public class QuestionCreateDto
    {
        public string Text { get; set; } = string.Empty;
        public int Points { get; set; }
        public QuestionType Type { get; set; }
        public List<QuestionOptionCreateDto> Options { get; set; } = new();
    }

    public class QuestionOptionCreateDto
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

    public enum QuestionType
    {
        MultipleChoice,
        TrueFalse,
        OpenEnded
    }

    public class ExamTakeDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public List<QuestionTakeDto> Questions { get; set; } = new();
    }

    public class QuestionTakeDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Points { get; set; }
        public QuestionType Type { get; set; }
        public List<QuestionOptionTakeDto> Options { get; set; } = new();
    }

    public class QuestionOptionTakeDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class ExamSubmitDto
    {
        public int ExamId { get; set; }
        public List<StudentAnswerDto> Answers { get; set; } = new();
    }

    public class StudentAnswerDto
    {
        public int QuestionId { get; set; }
        public int? SelectedOptionId { get; set; }
        public string? TextAnswer { get; set; }
    }

    public class ExamResultDto
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int Score { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
