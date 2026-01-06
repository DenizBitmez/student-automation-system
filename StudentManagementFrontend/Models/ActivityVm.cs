using System.ComponentModel.DataAnnotations;

namespace StudentManagementFrontend.Models
{
    public class ActivityVm
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Etkinlik adı zorunludur")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Tür seçimi zorunludur")]
        public string Type { get; set; } = "Kitap";
        
        public string? Description { get; set; }
        public DateTime ActivityDate { get; set; } = DateTime.Today;
    }

    public class ActivityType
    {
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
    }
}
