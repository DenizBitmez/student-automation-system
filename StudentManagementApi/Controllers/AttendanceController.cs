using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;

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
	}
}
