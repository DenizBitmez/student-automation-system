using System.ComponentModel.DataAnnotations;

namespace StudentManagementFrontend.Models
{
    public class Course
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Ders kodu zorunludur")]
        [StringLength(20, ErrorMessage = "Ders kodu en fazla 20 karakter olabilir")]
        public string Code { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Ders adı zorunludur")]
        [StringLength(100, ErrorMessage = "Ders adı en fazla 100 karakter olabilir")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Dönem bilgisi zorunludur")]
        [StringLength(20, ErrorMessage = "Dönem bilgisi en fazla 20 karakter olabilir")]
        public string Semester { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "AKTS kredisi zorunludur")]
        [Range(1, 10, ErrorMessage = "AKTS kredisi 1-10 arasında olmalıdır")]
        public int Credits { get; set; } = 3;
        
        [Range(0, 200, ErrorMessage = "Kapasite 0-200 arasında olmalıdır")]
        public int? Capacity { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [Required(ErrorMessage = "Öğretmen seçimi zorunludur")]
        public int? TeacherId { get; set; }
        
        public Teacher? Teacher { get; set; }

        public string TeacherName { get; set; } = string.Empty;
        
        // Navigation properties
        public ICollection<StudentCourse>? StudentCourses { get; set; }
        public ICollection<Grade>? Grades { get; set; }
        public ICollection<Attendance>? Attendances { get; set; }
        
        // Helper properties
        public string DisplayName => $"{Code} - {Name}";
        
        // For form handling
        public bool IsNew => Id == 0;
    }
}
