using System.ComponentModel.DataAnnotations;

namespace StudentManagementFrontend.Models;

public class Attendance
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Öğrenci seçimi zorunludur")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Ders seçimi zorunludur")]
    public int CourseId { get; set; }

    [Required(ErrorMessage = "Tarih zorunludur")]
    public DateTime Date { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Devamsızlık durumu zorunludur")]
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Absent;

    [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir")]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Student? Student { get; set; }
    public Course? Course { get; set; }

    // Computed properties
    public string StatusClass => Status switch
    {
        AttendanceStatus.Present => "text-success",
        AttendanceStatus.Excused => "text-info",
        AttendanceStatus.Late => "text-warning",
        _ => "text-danger"
    };

    public string StatusText => Status switch
    {
        AttendanceStatus.Present => "Katıldı",
        AttendanceStatus.Excused => "İzinli",
        AttendanceStatus.Late => "Geç Geldi",
        _ => "Katılmadı"
    };
}

public enum AttendanceStatus
{
    [Display(Name = "Yok")]
    Absent = 0,
    
    [Display(Name = "Var")]
    Present = 1,
    
    [Display(Name = "İzinli")]
    Excused = 2,
    
    [Display(Name = "Geç Geldi")]
    Late = 3
}
