using System.ComponentModel.DataAnnotations;

namespace StudentManagementFrontend.Models;

public class Teacher
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Ad alanı zorunludur")]
    [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad alanı zorunludur")]
    [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta adresi zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Branş alanı zorunludur")]
    [StringLength(100, ErrorMessage = "Branş en fazla 100 karakter olabilir")]
    public string Branch { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Uzmanlık alanı en fazla 20 karakter olabilir")]
    public string? Expertise { get; set; }

    [DataType(DataType.Date)]
    public DateTime? HireDate { get; set; }

    public string? ProfileImageUrl { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public string? UserId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Course>? Courses { get; set; }
}
