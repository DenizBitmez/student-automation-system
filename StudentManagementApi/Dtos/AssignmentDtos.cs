using System;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementApi.Dtos
{
    public class AssignmentDtos
    {
        public record AssignmentCreateDto(
            [Required] string Title,
            [Required] string Description,
            [Required] DateTime DueDate,
            [Required] int CourseId
        );

        public record AssignmentUpdateDto(
            [Required] string Title,
            [Required] string Description,
            [Required] DateTime DueDate
        );

        public record AssignmentVm(
            int Id,
            string Title,
            string Description,
            DateTime CreatedAt,
            DateTime DueDate,
            int CourseId,
            string CourseName,
            string TeacherName
        );
    }

    public class SubmissionDtos
    {
        public record SubmissionCreateDto(
            [Required] int AssignmentId,
            [Required] string Content
        );

        public record SubmissionGradeDto(
            [Required] int Grade,
            string? Feedback
        );

        public record SubmissionVm(
            int Id,
            int AssignmentId,
            string AssignmentTitle,
            string StudentId,
            string StudentName,
            string Content,
            DateTime SubmittedAt,
            int? Grade,
            string? Feedback
        );
    }
}
