namespace StudentManagementApi.Dtos
{
	public class TeacherDtos
	{
		public record TeacherCreateDto(string Email, string FullName, string Password, string? Department);
		public record TeacherVm(int Id, string Email, string FullName, string? Department);
	}
}
