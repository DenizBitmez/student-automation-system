using System;

namespace StudentManagementFrontend.Models
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime DueDate { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
    }

    public class AssignmentCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);
        public int CourseId { get; set; }
    }

    public class AssignmentUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
    }

    public class Submission
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public string AssignmentTitle { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public int? Grade { get; set; }
        public string? Feedback { get; set; }
    }

    public class SubmissionCreateDto
    {
        public int AssignmentId { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    public class SubmissionGradeDto
    {
        public int Grade { get; set; }
        public string? Feedback { get; set; }
    }
}
