using System.ComponentModel.DataAnnotations;

namespace StudentManagementFrontend.Models;

public class Grade
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Öğrenci seçimi zorunludur")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Ders seçimi zorunludur")]
    public int CourseId { get; set; }

    [Range(0, 100, ErrorMessage = "Not 0-100 arasında olmalıdır")]
    public decimal Score { get; set; }

    [StringLength(2, ErrorMessage = "Harf notu en fazla 2 karakter olabilir")]
    public string? LetterGrade { get; set; }

    [StringLength(50, ErrorMessage = "Dönem en fazla 50 karakter olabilir")]
    public string? Term { get; set; }

    [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Not tarihi zorunludur")]
    public DateTime GradeDate { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Student? Student { get; set; }
    public Course? Course { get; set; }

    // Computed properties
    public string GradeStatus => Score >= 60 ? "Geçti" : "Kaldı";

    public string GradeClass => Score >= 90 ? "text-success" : 
                              Score >= 80 ? "text-primary" :
                              Score >= 70 ? "text-info" :
                              Score >= 60 ? "text-warning" : "text-danger";
}
