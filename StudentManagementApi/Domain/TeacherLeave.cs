using System.ComponentModel.DataAnnotations;

namespace StudentManagementApi.Domain;

public class TeacherLeave
{
    public int Id { get; set; }
    
    [Required]
    public int TeacherId { get; set; }
    public Teacher Teacher { get; set; } = default!;
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [Required]
    [StringLength(1000)]
    public string Reason { get; set; } = default!;
    
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
