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
    public class DocumentController(AppDbContext db) : ControllerBase
    {
        [HttpGet("my")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyDocuments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var student = await db.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null) return NotFound("Öğrenci bulunamadı.");

            var documents = await db.StudentDocuments
                .Where(d => d.StudentId == student.Id)
                .OrderByDescending(d => d.IssueDate)
                .Select(d => new DocumentViewDto(d.Id, d.Title, d.Type, d.FileUrl, d.IssueDate))
                .ToListAsync();

            return Ok(documents);
        }

        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetStudentDocuments(int studentId)
        {
            var documents = await db.StudentDocuments
                .Where(d => d.StudentId == studentId)
                .OrderByDescending(d => d.IssueDate)
                .ToListAsync();

            return Ok(documents);
        }

        // For now, we don't implement upload as per user request to just "see" them
        // but an admin would typically upload these.
    }
}
