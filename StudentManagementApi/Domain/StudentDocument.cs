using System.ComponentModel.DataAnnotations;

namespace StudentManagementApi.Domain
{
    public class StudentDocument
    {
        public int Id { get; set; }
        
        [Required]
        public int StudentId { get; set; }
        public Student Student { get; set; } = default!;
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = default!;
        
        [Required]
        [StringLength(50)]
        public string Type { get; set; } = default!; // e.g., "Karne", "Sertifika", "Başarı Belgesi"
        
        [Required]
        public string FileUrl { get; set; } = default!;
        
        public DateTime IssueDate { get; set; } = DateTime.UtcNow;
        
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
