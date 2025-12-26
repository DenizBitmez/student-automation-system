namespace StudentManagementFrontend.Models;

public class StudentVm
{
    public int Id { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
    // Add other properties if Backend Vm has them
}
