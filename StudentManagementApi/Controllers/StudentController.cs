using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using StudentManagementApi.Domain;
using static StudentManagementApi.Dtos.StudentDtos;

namespace StudentManagementApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin,Teacher")]
	public class StudentController(AppDbContext db, UserManager<ApplicationUser> um) : ControllerBase
	{
		[HttpGet]
		public async Task<ActionResult<IEnumerable<StudentVm>>> Get()
=> await db.Students.Include(s => s.User)
.Select(s => new StudentVm(s.Id, s.User.Email!, s.User.FullName ?? "", s.EnrolledAt))
.ToListAsync();


		[HttpPost]
		public async Task<ActionResult<StudentVm>> Create(StudentCreateDto dto)
		{
			var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email, FullName = dto.FullName, EmailConfirmed = true };
			var res = await um.CreateAsync(user, dto.Password);
			if (!res.Succeeded) return BadRequest(res.Errors);
			await um.AddToRoleAsync(user, "Student");
			var student = new Student { UserId = user.Id };
			db.Students.Add(student);
			await db.SaveChangesAsync();
			return CreatedAtAction(nameof(GetById), new { id = student.Id }, new StudentVm(student.Id, user.Email!, user.FullName ?? "", student.EnrolledAt));
		}


		[HttpGet("{id:int}")]
		[AllowAnonymous]
		public async Task<ActionResult<StudentVm>> GetById(int id)
		{
			var s = await db.Students.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
			if (s is null) return NotFound();
			return new StudentVm(s.Id, s.User.Email!, s.User.FullName ?? "", s.EnrolledAt);
		}


		[HttpPut("{id:int}")]
		public async Task<IActionResult> Update(int id, StudentUpdateDto dto)
		{
			var s = await db.Students.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
			if (s is null) return NotFound();
			s.User.FullName = dto.FullName;
			await db.SaveChangesAsync();
			return NoContent();
		}

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> Delete(int id)
		{
			var s = await db.Students.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
			if (s is null) return NotFound();
			
			db.Students.Remove(s);
            if(s.User != null) {
			    await um.DeleteAsync(s.User);
            }
            
			await db.SaveChangesAsync();
			return NoContent();
		}
	}
}
