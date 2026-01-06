namespace StudentManagementFrontend.Models
{
    public class ScheduleItem
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public DayOfWeek DayOfWeek { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string Classroom { get; set; } = string.Empty;
    }

    public class ScheduleItemCreateDto
    {
        public int CourseId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string? Classroom { get; set; }
    }
}
