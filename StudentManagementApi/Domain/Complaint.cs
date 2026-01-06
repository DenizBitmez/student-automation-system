using System.ComponentModel.DataAnnotations;

namespace StudentManagementApi.Domain
{
    public class Complaint
    {
        public int Id { get; set; }
        
        [Required]
        public int StudentId { get; set; }
        public Student Student { get; set; } = default!;
        
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = default!;
        
        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = default!;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [StringLength(1000)]
        public string? AdminResponse { get; set; }
        
        public bool IsResolved { get; set; } = false;
        
        public DateTime? ResolvedAt { get; set; }
    }
}
