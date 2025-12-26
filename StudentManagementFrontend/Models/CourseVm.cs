namespace StudentManagementFrontend.Models;

public enum CourseStatus
{
    Active,
    Completed,
    Archived
}

public class CourseVm
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public CourseStatus Status { get; set; }
    public int StudentCount { get; set; }
}
