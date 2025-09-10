namespace StudentManagementApi.Dtos
{
	public class StudentDtos
	{
		public record StudentCreateDto(string Email, string FullName, string Password);
		public record StudentUpdateDto(string FullName);
		public record StudentVm(int Id, string Email, string FullName, DateTime EnrolledAt);
	}
}
