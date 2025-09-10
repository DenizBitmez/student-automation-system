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


		[HttpPost]
		public async Task<ActionResult<TeacherVm>> Create(TeacherCreateDto dto)
		{
			var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email, FullName = dto.FullName, EmailConfirmed = true };
			var res = await um.CreateAsync(user, dto.Password);
			if (!res.Succeeded) return BadRequest(res.Errors);
			await um.AddToRoleAsync(user, "Teacher");
			var teacher = new Teacher { UserId = user.Id, Department = dto.Department };
			db.Teachers.Add(teacher);
			await db.SaveChangesAsync();
			return CreatedAtAction(nameof(Get), new { id = teacher.Id }, new TeacherVm(teacher.Id, user.Email!, user.FullName ?? "", teacher.Department));
		}
	}
}
