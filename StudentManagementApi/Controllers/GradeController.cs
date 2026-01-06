using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using StudentManagementApi.Services;
using System.Security.Claims;
using static StudentManagementApi.Dtos.GradeDtos;
using static StudentManagementApi.Dtos.StudentActionDtos;

namespace StudentManagementApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class GradeController(AppDbContext db, INotificationService notificationService) : ControllerBase
	{
		[HttpPost]
		[Authorize(Roles = "Teacher,Admin")]
		public async Task<IActionResult> SetGrade(GradeUpdateDto dto)
		{
			var enrollment = await db.Enrollments
				.Include(e => e.Student)
				.ThenInclude(s => s.User)
				.Include(e => e.Course)
				.FirstOrDefaultAsync(e => e.Id == dto.EnrollmentId);
			
			if (enrollment is null) return NotFound();
			
			var oldGrade = enrollment.Grade;
			enrollment.Grade = dto.Grade;
			enrollment.Comment = dto.Comment;
			await db.SaveChangesAsync();
			
			// Send notification to student
			if (enrollment.Student.User.Id != null)
			{
				await notificationService.SendGradeNotificationAsync(
					enrollment.Student.User.Id, 
					enrollment.Course.Name, 
					dto.Grade, 
					dto.Comment
				);
			}
			
			return Ok();
		}


		[HttpGet("by-student/{studentId:int}")]
		[Authorize]
		public async Task<IActionResult> GetGradesByStudent(int studentId)
		{
			var list = await db.Enrollments.Include(x => x.Course)
			.Where(x => x.StudentId == studentId)
			.Select(x => new { x.Id, x.Course.Code, x.Course.Name, x.Grade, x.Comment })
			.ToListAsync();
			return Ok(list);
		}

		[HttpGet("my")]
		[Authorize(Roles = "Student")]
		public async Task<IActionResult> GetMyGrades()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var student = await db.Students.FirstOrDefaultAsync(s => s.UserId == userId);
			if (student == null) return NotFound("Öğrenci bulunamadı.");

			var list = await db.Enrollments.Include(x => x.Course)
				.Where(x => x.StudentId == student.Id)
				.Select(x => new { x.Id, x.Course.Code, x.Course.Name, x.Grade, x.Comment })
				.ToListAsync();
			return Ok(list);
		}
	}
}
