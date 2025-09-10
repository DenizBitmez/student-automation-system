using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementApi.Data;
using Microsoft.EntityFrameworkCore;
using static StudentManagementApi.Dtos.EnrollmentDtos;

namespace StudentManagementApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Teacher,Admin")]
	public class EnrollmentController(AppDbContext db):ControllerBase
	{
		[HttpPost]
		public async Task<IActionResult> Enroll(EnrollDto dto)
		{
			var exists = await db.Enrollments.AnyAsync(e => e.CourseId == dto.CourseId && e.StudentId == dto.StudentId);
			if (exists) return Conflict("Already enrolled");
			db.Enrollments.Add(new Domain.Enrollment { CourseId = dto.CourseId, StudentId = dto.StudentId });
			await db.SaveChangesAsync();
			return Ok();
		}


		[HttpDelete]
		public async Task<IActionResult> Unenroll([FromQuery] int courseId, [FromQuery] int studentId)
		{
			var e = await db.Enrollments.FirstOrDefaultAsync(x => x.CourseId == courseId && x.StudentId == studentId);
			if (e is null) return NotFound();
			db.Enrollments.Remove(e);
			await db.SaveChangesAsync();
			return NoContent();
		}
	}
}
