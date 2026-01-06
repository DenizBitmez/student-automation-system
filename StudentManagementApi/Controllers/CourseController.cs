using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using static StudentManagementApi.Dtos.CourseDtos;
using StudentManagementApi.Domain;

namespace StudentManagementApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CourseController(AppDbContext db) : ControllerBase
	{
		[HttpGet]
		[Authorize]
		public async Task<IEnumerable<CourseVm>> Get()
=> await db.Courses.Include(c => c.Teacher).ThenInclude(t => t.User)
.Include(c => c.Enrollments)
.Select(c => new CourseVm(c.Id, c.Code, c.Name, c.TeacherId, c.Teacher.User.FullName ?? "", c.Status, c.Enrollments.Count))
.ToListAsync();


		[HttpGet("{id:int}/students")]
		[Authorize(Roles = "Teacher,Admin")]
		public async Task<ActionResult<IEnumerable<CourseStudentVm>>> GetStudents(int id)
		{
			var list = await db.Enrollments
				.Include(e => e.Student).ThenInclude(s => s.User)
				.Where(e => e.CourseId == id)
				.Select(e => new CourseStudentVm(e.StudentId, e.Id, e.Student.Id.ToString(), e.Student.User.FullName ?? "", e.Grade, e.AttendanceCount))
				.ToListAsync();
			return Ok(list);
		}


		[HttpGet("{id:int}")]
		[Authorize]
		public async Task<ActionResult<CourseVm>> GetById(int id)
		{
			var c = await db.Courses.Include(c => c.Teacher).ThenInclude(t => t.User).Include(c => c.Enrollments)
				.FirstOrDefaultAsync(x => x.Id == id);
			if (c is null) return NotFound();
			return new CourseVm(c.Id, c.Code, c.Name, c.TeacherId, c.Teacher.User.FullName ?? "", c.Status, c.Enrollments.Count);
		}


		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<CourseVm>> Create(CourseCreateDto dto)
		{
			var c = new Course { Code = dto.Code, Name = dto.Name, TeacherId = dto.TeacherId };
			db.Courses.Add(c);
			await db.SaveChangesAsync();
			return CreatedAtAction(nameof(GetById), new { id = c.Id }, new CourseVm(c.Id, c.Code, c.Name, c.TeacherId, "", c.Status, 0));
		}


		[HttpPut("{id:int}/status")]
		[Authorize(Roles = "Teacher,Admin")]
		public async Task<IActionResult> UpdateStatus(int id, CourseUpdateStatusDto dto)
		{
			var c = await db.Courses.FindAsync(id);
			if (c is null) return NotFound();
			c.Status = dto.Status;
			await db.SaveChangesAsync();
			return NoContent();
		}

		[HttpPut("{id:int}")]
		[Authorize(Roles = "Teacher,Admin")]
		public async Task<IActionResult> Update(int id, CourseUpdateDto dto)
		{
			var c = await db.Courses.FindAsync(id);
			if (c is null) return NotFound();
			
			c.Code = dto.Code;
			c.Name = dto.Name;
			c.TeacherId = dto.TeacherId;
			
			await db.SaveChangesAsync();
			return NoContent();
		}

		[HttpDelete("{id:int}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int id)
		{
			var c = await db.Courses.FindAsync(id);
			if (c is null) return NotFound();
			
			db.Courses.Remove(c);
			await db.SaveChangesAsync();
			return NoContent();
		}

		[HttpPost("{courseId:int}/enroll/{studentId:int}")]
		[Authorize(Roles = "Teacher,Admin")]
		public async Task<IActionResult> Enroll(int courseId, int studentId)
		{
			var exists = await db.Enrollments.AnyAsync(e => e.CourseId == courseId && e.StudentId == studentId);
			if (exists) return BadRequest("Öğrenci zaten bu derse kayıtlı.");

			var e = new Enrollment { CourseId = courseId, StudentId = studentId };
			db.Enrollments.Add(e);
			await db.SaveChangesAsync();
			return Ok();
		}

		[HttpDelete("{courseId:int}/students/{studentId:int}")]
		[Authorize(Roles = "Teacher,Admin")]
		public async Task<IActionResult> RemoveStudent(int courseId, int studentId)
		{
			var e = await db.Enrollments.FirstOrDefaultAsync(x => x.CourseId == courseId && x.StudentId == studentId);
			if (e is null) return NotFound();

			db.Enrollments.Remove(e);
			await db.SaveChangesAsync();
			return NoContent();
		}

		[HttpPut("{courseId:int}/students/{studentId:int}/grade")]
		[Authorize(Roles = "Teacher,Admin")]
		public async Task<IActionResult> UpdateGrade(int courseId, int studentId, [FromBody] GradeUpdateInternalDto dto)
		{
			var e = await db.Enrollments.FirstOrDefaultAsync(x => x.CourseId == courseId && x.StudentId == studentId);
			if (e is null) return NotFound();

			e.Grade = (decimal)dto.Grade;
			await db.SaveChangesAsync();
			return NoContent();
		}

		[HttpGet("count/total")]
		public async Task<int> GetTotalCount() => await db.Courses.CountAsync();

		[HttpGet("count/active")]
		public async Task<int> GetActiveCount() => await db.Courses.CountAsync(c => c.Status == CourseStatus.Active);

		[HttpGet("search")]
		public async Task<ActionResult<IEnumerable<CourseVm>>> Search([FromQuery] string q)
		{
			return await db.Courses.Include(c => c.Teacher).ThenInclude(t => t.User)
				.Where(c => c.Name.Contains(q) || c.Code.Contains(q))
				.Select(c => new CourseVm(c.Id, c.Code, c.Name, c.TeacherId, c.Teacher.User.FullName ?? "", c.Status, c.Enrollments.Count))
				.ToListAsync();
		}
	}

	public record GradeUpdateInternalDto(double Grade);
}
