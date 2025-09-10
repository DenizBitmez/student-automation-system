namespace StudentManagementApi.Domain
{
	public class Teacher
	{
		public int Id { get; set; }
		public string UserId { get; set; } = default!;
		public ApplicationUser User { get; set; } = default!;
		public string? Department { get; set; }
	}
}
