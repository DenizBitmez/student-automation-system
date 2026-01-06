using System.ComponentModel.DataAnnotations;

namespace StudentManagementFrontend.Models
{
    public class ComplaintVm
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Konu başlığı zorunludur")]
        [StringLength(100, ErrorMessage = "Başlık en fazla 100 karakter olabilir")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Açıklama zorunludur")]
        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
        public string Description { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; }
        public string? AdminResponse { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string? StudentName { get; set; }
    }

    public class ComplaintCreateVm
    {
        [Required(ErrorMessage = "Konu başlığı zorunludur")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Açıklama zorunludur")]
        public string Description { get; set; } = string.Empty;
    }
}
