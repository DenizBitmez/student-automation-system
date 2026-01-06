using System;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementApi.Domain
{
    public class ScheduleItem
    {
        public int Id { get; set; }

        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; } = default!;

        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [StringLength(100)]
        public string Classroom { get; set; } = string.Empty;
    }
}
