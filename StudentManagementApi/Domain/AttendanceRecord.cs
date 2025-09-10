namespace StudentManagementApi.Domain
{
	public class AttendanceRecord
	{
		public int Id { get; set; }
		public int EnrollmentId { get; set; }
		public Enrollment Enrollment { get; set; } = default!;
		public DateTime Date { get; set; }
		public bool Present { get; set; }
	}
}
