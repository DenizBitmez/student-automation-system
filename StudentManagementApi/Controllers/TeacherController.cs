using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using StudentManagementApi.Domain;
using static StudentManagementApi.Dtos.TeacherDtos;

namespace StudentManagementApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize(Roles = "Admin")]
	public class TeacherController(AppDbContext db, UserManager<ApplicationUser> um) : ControllerBase
	{
		[HttpGet]
		public async Task<IEnumerable<TeacherVm>> Get()
=> await db.Teachers.Include(t => t.User)
.Select(t => new TeacherVm(t.Id, t.User.Email!, t.User.FullName ?? "", t.Department))
.ToListAsync();


		[HttpGet("{id:int}")]
		public async Task<ActionResult<TeacherVm>> GetById(int id)
		{
			var t = await db.Teachers.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
			if (t is null) return NotFound();
			return new TeacherVm(t.Id, t.User.Email!, t.User.FullName ?? "", t.Department);
		}


		[HttpPost]
		public async Task<ActionResult<TeacherVm>> Create(TeacherCreateDto dto)
		{
			// Fix: Use Email as Username or derive it, ensuring no mismatch
			var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email, FullName = dto.FullName, EmailConfirmed = true };
			var res = await um.CreateAsync(user, dto.Password);
			if (!res.Succeeded) return BadRequest(res.Errors);
			await um.AddToRoleAsync(user, "Teacher");
			var teacher = new Teacher { UserId = user.Id, Department = dto.Department };
			db.Teachers.Add(teacher);
			await db.SaveChangesAsync();
			return CreatedAtAction(nameof(GetById), new { id = teacher.Id }, new TeacherVm(teacher.Id, user.Email!, user.FullName ?? "", teacher.Department));
		}

		[HttpPut("{id:int}")]
		public async Task<IActionResult> Update(int id, TeacherUpdateDto dto)
		{
			var t = await db.Teachers.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
			if (t is null) return NotFound();

			t.Department = dto.Department;
			t.User.FullName = dto.FullName;

			await db.SaveChangesAsync();
			return NoContent();
		}

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> Delete(int id)
		{
			var t = await db.Teachers.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
			if (t is null) return NotFound();

			// Remove teacher entry
			db.Teachers.Remove(t);
			
			// Optional: Remove user entry too? For now just the teacher role/entry
			// In many systems we keep the user but remove the role/profile. 
			// But here let's be thorough if it's a "Delete Teacher" action.
			await um.DeleteAsync(t.User);

			await db.SaveChangesAsync();
			return NoContent();
		}

		[HttpGet("count")]
		public async Task<int> GetCount() => await db.Teachers.CountAsync();
	}
}
