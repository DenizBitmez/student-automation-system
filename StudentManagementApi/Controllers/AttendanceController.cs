using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;

using System.Security.Claims;

namespace StudentManagementApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class AttendanceController(AppDbContext db) : ControllerBase
	{
		[HttpPost("tick/{enrollmentId:int}")]
		[Authorize(Roles = "Teacher,Admin")]
		public async Task<IActionResult> Tick(int enrollmentId, [FromQuery] bool present = true)
		{
			var e = await db.Enrollments.FindAsync(enrollmentId);
			if (e is null) return NotFound();
			db.AttendanceRecords.Add(new Domain.AttendanceRecord { EnrollmentId = enrollmentId, Date = DateTime.UtcNow, Present = present });
			if (present) e.AttendanceCount++;
			await db.SaveChangesAsync();
			return Ok();
		}


		[HttpGet("by-student/{studentId:int}")]
		public async Task<IActionResult> ByStudent(int studentId)
		{
			var list = await db.AttendanceRecords
			.Include(a => a.Enrollment).ThenInclude(e => e.Course)
			.Include(a => a.Enrollment).ThenInclude(e => e.Student)
			.Where(a => a.Enrollment.StudentId == studentId)
			.OrderByDescending(a => a.Date)
			.Select(a => new { a.Id, a.Date, a.Present, a.Enrollment.Course.Name })
			.ToListAsync();
			return Ok(list);
		}

		[HttpGet("my")]
		[Authorize(Roles = "Student")]
		public async Task<IActionResult> GetMyAttendance()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var student = await db.Students.FirstOrDefaultAsync(s => s.UserId == userId);
			if (student == null) return NotFound("Öğrenci bulunamadı.");

			var list = await db.AttendanceRecords
				.Include(a => a.Enrollment).ThenInclude(e => e.Course)
				.Where(a => a.Enrollment.StudentId == student.Id)
				.OrderByDescending(a => a.Date)
				.Select(a => new { a.Id, a.Date, a.Present, a.Enrollment.Course.Name })
				.ToListAsync();
			return Ok(list);
		}
	}
}
