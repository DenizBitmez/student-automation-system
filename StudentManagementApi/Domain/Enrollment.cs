namespace StudentManagementApi.Domain
{
	public class Enrollment
	{
		public int Id { get; set; }
		public int StudentId { get; set; }
		public Student Student { get; set; } = default!;
		public int CourseId { get; set; }
		public Course Course { get; set; } = default!;
		public decimal? Grade { get; set; }
		public int AttendanceCount { get; set; }
		public string? Comment { get; set; }
	}
}
