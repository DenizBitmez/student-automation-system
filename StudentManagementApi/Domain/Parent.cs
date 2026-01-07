using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementApi.Domain
{
    public class Parent
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = default!;

        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
