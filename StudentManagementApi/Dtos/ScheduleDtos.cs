using System;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementApi.Dtos
{
    public class ScheduleDtos
    {
        public record ScheduleItemCreateDto(
            [Required] int CourseId,
            [Required] DayOfWeek DayOfWeek,
            [Required] string StartTime, // HH:mm
            [Required] string EndTime,   // HH:mm
            string? Classroom
        );

        public record ScheduleItemVm(
            int Id,
            int CourseId,
            string CourseName,
            string CourseCode,
            string TeacherName,
            DayOfWeek DayOfWeek,
            string StartTime,
            string EndTime,
            string Classroom
        );
    }
}
