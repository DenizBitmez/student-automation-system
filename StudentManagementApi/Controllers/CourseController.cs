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


		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<CourseVm>> Create(CourseCreateDto dto)
		{
			var c = new Course { Code = dto.Code, Name = dto.Name, TeacherId = dto.TeacherId };
			db.Courses.Add(c);
			await db.SaveChangesAsync();
			return CreatedAtAction(nameof(Get), new { id = c.Id }, new CourseVm(c.Id, c.Code, c.Name, c.TeacherId, "", c.Status, 0));
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
	}
}
