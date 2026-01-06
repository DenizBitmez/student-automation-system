using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using StudentManagementApi.Domain;
using System.Security.Claims;
using static StudentManagementApi.Dtos.StudentActionDtos;

namespace StudentManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ActivityController(AppDbContext db) : ControllerBase
    {
        [HttpGet("my")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyActivities()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var student = await db.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null) return NotFound("Öğrenci bulunamadı.");

            var activities = await db.SocialActivities
                .Where(a => a.StudentId == student.Id)
                .OrderByDescending(a => a.ActivityDate)
                .Select(a => new ActivityViewDto(a.Id, a.Title, a.Type, a.Description, a.ActivityDate))
                .ToListAsync();

            return Ok(activities);
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CreateActivity(ActivityCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var student = await db.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null) return NotFound("Öğrenci bulunamadı.");

            var activity = new SocialActivity
            {
                StudentId = student.Id,
                Title = dto.Title,
                Type = dto.Type,
                Description = dto.Description,
                ActivityDate = DateTime.SpecifyKind(dto.ActivityDate, DateTimeKind.Utc)
            };

            db.SocialActivities.Add(activity);
            await db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetStudentActivities(int studentId)
        {
            var activities = await db.SocialActivities
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.ActivityDate)
                .ToListAsync();

            return Ok(activities);
        }
    }
}
