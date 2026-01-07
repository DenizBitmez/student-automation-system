namespace StudentManagementFrontend.Models
{
    public class SystemStatsDto
    {
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalActiveEnrollments { get; set; }
    }

    public class EnrollmentTrendDto
    {
        public string Period { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class CoursePerformanceDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public double AverageGrade { get; set; }
        public double AttendanceRate { get; set; }
        public List<GradeDistributionDto> GradeDistribution { get; set; } = new();
    }

    public class GradeDistributionDto
    {
        public string Label { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class StudentPerformanceDto
    {
        public int StudentId { get; set; }
        public double GPA { get; set; }
        public double OverallAttendanceRate { get; set; }
        public int CompletedAssignments { get; set; }
        public int PendingAssignments { get; set; }
    }

    public class PredictionResultDto
    {
        public int StudentId { get; set; }
        public double SuccessProbability { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public double PredictedFinalGrade { get; set; }
        public List<string> Recommendations { get; set; } = new();
    }
}
