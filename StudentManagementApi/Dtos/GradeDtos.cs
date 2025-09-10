namespace StudentManagementApi.Dtos
{
	public class GradeDtos
	{
		public record GradeUpdateDto(int EnrollmentId, decimal Grade, string? Comment);
	}
}
