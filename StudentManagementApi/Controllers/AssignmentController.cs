using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using StudentManagementApi.Domain;
using static StudentManagementApi.Dtos.AssignmentDtos;
using System.Security.Claims;

namespace StudentManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssignmentController(AppDbContext db, UserManager<ApplicationUser> um) : ControllerBase
    {
        [HttpGet("course/{courseId:int}")]
        public async Task<ActionResult<IEnumerable<AssignmentVm>>> GetByCourse(int courseId)
        {
            return await db.Assignments
                .Include(a => a.Course)
                .Include(a => a.Teacher)
                .Where(a => a.CourseId == courseId)
                .OrderByDescending(a => a.DueDate)
                .Select(a => new AssignmentVm(
                    a.Id,
                    a.Title,
                    a.Description,
                    a.CreatedAt,
                    a.DueDate,
                    a.CourseId,
                    a.Course.Name,
                    a.Teacher.FullName ?? "Teacher"
                ))
                .ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AssignmentVm>> GetById(int id)
        {
            var a = await db.Assignments
                .Include(a => a.Course)
                .Include(a => a.Teacher)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (a is null) return NotFound();

            return new AssignmentVm(
                a.Id,
                a.Title,
                a.Description,
                a.CreatedAt,
                a.DueDate,
                a.CourseId,
                a.Course.Name,
                a.Teacher.FullName ?? "Teacher"
            );
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<AssignmentVm>> Create(AssignmentCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var course = await db.Courses.FindAsync(dto.CourseId);
            if (course == null) return BadRequest("Invalid course");

            var assignment = new Assignment
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                CourseId = dto.CourseId,
                TeacherId = userId,
                CreatedAt = DateTime.UtcNow
            };

            db.Assignments.Add(assignment);
            await db.SaveChangesAsync();

            var user = await um.FindByIdAsync(userId);

            return CreatedAtAction(nameof(GetById), new { id = assignment.Id },
                new AssignmentVm(assignment.Id, assignment.Title, assignment.Description, assignment.CreatedAt, 
                    assignment.DueDate, assignment.CourseId, course.Name, user?.FullName ?? "Teacher"));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Update(int id, AssignmentUpdateDto dto)
        {
            var a = await db.Assignments.FindAsync(id);
            if (a is null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && a.TeacherId != userId) return Forbid();

            a.Title = dto.Title;
            a.Description = dto.Description;
            a.DueDate = dto.DueDate;

            await db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Delete(int id)
        {
            var a = await db.Assignments.FindAsync(id);
            if (a is null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && a.TeacherId != userId) return Forbid();

            db.Assignments.Remove(a);
            await db.SaveChangesAsync();

            return NoContent();
        }
    }
}
