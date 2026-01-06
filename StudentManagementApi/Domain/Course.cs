namespace StudentManagementApi.Domain
{
	public class Course
	{
		public int Id { get; set; }
		public string Code { get; set; } = default!; 
		public string Name { get; set; } = default!;
		public int TeacherId { get; set; }
		public Teacher Teacher { get; set; } = default!;
		public CourseStatus Status { get; set; } = CourseStatus.Active;
		public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
	}

	public enum CourseStatus { Active, Completed, Archived }
}
