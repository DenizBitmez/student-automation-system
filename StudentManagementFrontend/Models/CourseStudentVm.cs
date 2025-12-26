namespace StudentManagementFrontend.Models;

public class CourseStudentVm
{
    public int StudentId { get; set; }
    public int EnrollmentId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public decimal? Grade { get; set; }
    public int AttendanceCount { get; set; }
}
