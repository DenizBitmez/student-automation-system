using System.ComponentModel.DataAnnotations;

namespace StudentManagementApi.Domain
{
    public class SocialActivity
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
        public string Type { get; set; } = default!; // e.g., "Kitap", "Spor", "Sanat", "Gönüllülük"
        
        [StringLength(1000)]
        public string? Description { get; set; }
        
        public DateTime ActivityDate { get; set; } = DateTime.UtcNow;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
