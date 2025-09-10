using System.ComponentModel.DataAnnotations;

namespace StudentManagementFrontend.Models;

public class StudentCourse
{
    [Required(ErrorMessage = "Öğrenci seçimi zorunludur")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Ders seçimi zorunludur")]
    public int CourseId { get; set; }

    [DataType(DataType.Date)]
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    [StringLength(500, ErrorMessage = "Notlar en fazla 500 karakter olabilir")]
    public string? Notes { get; set; }

    // Navigation properties
    public Student? Student { get; set; }
    public Course? Course { get; set; }

    // Computed properties
    public string Status => IsActive ? "Aktif" : "Pasif";
    public string StatusClass => IsActive ? "text-success" : "text-muted";
}
