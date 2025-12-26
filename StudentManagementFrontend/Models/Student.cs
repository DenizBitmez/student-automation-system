namespace StudentManagementFrontend.Models;

public class Student
{
    public int Id { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Class { get; set; }
    public DateTime EnrollmentDate { get; set; } = DateTime.Now;
    
    // Properties matching Backend DTO
    public string FullName { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; } // Set by backend DTO
}
