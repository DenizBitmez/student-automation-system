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
    public class ComplaintController(AppDbContext db) : ControllerBase
    {
        [HttpGet("my")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyComplaints()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var student = await db.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null) return NotFound("Öğrenci bulunamadı.");

            var complaints = await db.Complaints
                .Where(c => c.StudentId == student.Id)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new ComplaintViewDto(c.Id, c.Title, c.Description, c.CreatedAt, c.AdminResponse, c.IsResolved, c.ResolvedAt))
                .ToListAsync();

            return Ok(complaints);
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CreateComplaint(ComplaintCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var student = await db.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null) return NotFound("Öğrenci bulunamadı.");

            var complaint = new Complaint
            {
                StudentId = student.Id,
                Title = dto.Title,
                Description = dto.Description
            };

            db.Complaints.Add(complaint);
            await db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetAllComplaints()
        {
            var complaints = await db.Complaints
                .Include(c => c.Student)
                .ThenInclude(s => s.User)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new {
                    c.Id,
                    c.Title,
                    c.Description,
                    c.CreatedAt,
                    StudentName = c.Student.User.UserName,
                    c.AdminResponse,
                    c.IsResolved,
                    c.ResolvedAt
                })
                .ToListAsync();

            return Ok(complaints);
        }

        [HttpPut("{id}/respond")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RespondToComplaint(int id, ComplaintResponseDto dto)
        {
            var complaint = await db.Complaints.FindAsync(id);
            if (complaint == null) return NotFound();

            complaint.AdminResponse = dto.AdminResponse;
            complaint.IsResolved = true;
            complaint.ResolvedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
            return Ok();
        }
    }
}
