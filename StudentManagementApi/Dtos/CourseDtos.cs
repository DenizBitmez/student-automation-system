using StudentManagementApi.Domain;

namespace StudentManagementApi.Dtos
{
	public class CourseDtos
	{
		public record CourseCreateDto(string Code, string Name, int TeacherId);
		public record CourseUpdateDto(string Code, string Name, int TeacherId);
		public record CourseUpdateStatusDto(CourseStatus Status);
		public record CourseVm(int Id, string Code, string Name, int TeacherId, string TeacherName, CourseStatus Status, int StudentCount);
		public record CourseStudentVm(int StudentId, int EnrollmentId, string StudentNumber, string FullName, decimal? Grade, int AttendanceCount);
	}
}
