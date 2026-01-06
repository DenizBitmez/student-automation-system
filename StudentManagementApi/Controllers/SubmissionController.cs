using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using StudentManagementApi.Domain;
using static StudentManagementApi.Dtos.SubmissionDtos;
using System.Security.Claims;
using static StudentManagementApi.Dtos.AssignmentDtos;

namespace StudentManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubmissionController(AppDbContext db, UserManager<ApplicationUser> um) : ControllerBase
    {
        [HttpGet("assignment/{assignmentId:int}")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<IEnumerable<SubmissionVm>>> GetByAssignment(int assignmentId)
        {
            return await db.Submissions
                .Include(s => s.Student)
                .Include(s => s.Assignment)
                .Where(s => s.AssignmentId == assignmentId)
                .OrderByDescending(s => s.SubmittedAt)
                .Select(s => new SubmissionVm(
                    s.Id,
                    s.AssignmentId,
                    s.Assignment.Title,
                    s.StudentId,
                    s.Student.FullName ?? "Student",
                    s.Content,
                    s.SubmittedAt,
                    s.Grade,
                    s.Feedback
                ))
                .ToListAsync();
        }

        [HttpGet("student/my-submissions")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<SubmissionVm>>> GetMySubmissions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            return await db.Submissions
                .Include(s => s.Assignment)
                .Where(s => s.StudentId == userId)
                .OrderByDescending(s => s.SubmittedAt)
                .Select(s => new SubmissionVm(
                    s.Id,
                    s.AssignmentId,
                    s.Assignment.Title,
                    s.StudentId,
                    "Me",
                    s.Content,
                    s.SubmittedAt,
                    s.Grade,
                    s.Feedback
                ))
                .ToListAsync();
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<SubmissionVm>> Create(SubmissionCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var assignment = await db.Assignments.FindAsync(dto.AssignmentId);
            if (assignment == null) return BadRequest("Invalid assignment");

            if (DateTime.UtcNow > assignment.DueDate) return BadRequest("Assignment due date has passed");

            // Check if student already submitted
            var existing = await db.Submissions
                .FirstOrDefaultAsync(s => s.AssignmentId == dto.AssignmentId && s.StudentId == userId);
            
            if (existing != null) return BadRequest("You have already submitted for this assignment");

            var submission = new Submission
            {
                AssignmentId = dto.AssignmentId,
                StudentId = userId,
                Content = dto.Content,
                SubmittedAt = DateTime.UtcNow
            };

            db.Submissions.Add(submission);
            await db.SaveChangesAsync();

            var user = await um.FindByIdAsync(userId);

            return Ok(new SubmissionVm(submission.Id, submission.AssignmentId, assignment.Title, submission.StudentId, 
                user?.FullName ?? "Student", submission.Content, submission.SubmittedAt, null, null));
        }

        [HttpPost("{id:int}/grade")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Grade(int id, SubmissionGradeDto dto)
        {
            var s = await db.Submissions.Include(x => x.Assignment).FirstOrDefaultAsync(x => x.Id == id);
            if (s is null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && s.Assignment.TeacherId != userId) return Forbid();

            s.Grade = dto.Grade;
            s.Feedback = dto.Feedback;

            await db.SaveChangesAsync();

            return NoContent();
        }
    }
}
