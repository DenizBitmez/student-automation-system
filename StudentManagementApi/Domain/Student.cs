namespace StudentManagementApi.Domain
{
	public class Student
	{
		public int Id { get; set; }
		public string UserId { get; set; } = default!;
		public ApplicationUser User { get; set; } = default!;
		public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
		public int? ParentId { get; set; }
		public Parent? Parent { get; set; }
		
		public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
	}
}
