using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using StudentManagementApi.Domain;
using StudentManagementApi.Dtos;
using System.Security.Claims;
using static StudentManagementApi.Dtos.ParentDtos;

namespace StudentManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Parent,Admin")] 
    // Admin included for testing, though mostly for Parents
    public class ParentController(AppDbContext db, UserManager<ApplicationUser> um) : ControllerBase
    {
        [HttpGet("children")]
        public async Task<ActionResult<List<ChildSummaryDto>>> GetMyChildren()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var parent = await db.Parents
                .Include(p => p.Students)
                .ThenInclude(s => s.User)
                .Include(p => p.Students)
                .ThenInclude(s => s.Enrollments)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (parent == null) return NotFound("Parent profile not found.");

            var children = parent.Students.Select(s => new ChildSummaryDto(
                s.Id,
                s.User.FullName,
                "Grade " + ((DateTime.UtcNow.Year - s.EnrolledAt.Year) + 1), // Simple grade calc
                s.Enrollments.Count
            )).ToList();

            return Ok(children);
        }

        [HttpGet("child/{studentId}/overview")]
        public async Task<ActionResult<ChildOverviewDto>> GetChildOverview(int studentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var parent = await db.Parents
                .Include(p => p.Students)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (parent == null) return NotFound("Parent profile not found.");

            // Verify child belongs to parent
            var student = await db.Students
                .Include(s => s.User)
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null) return NotFound("Student not found");
            if (student.ParentId != parent.Id) return Forbid();

            // Calculate Stats (Reusing logic from AnalyticsController roughly)
            var enrollments = student.Enrollments;
            
            double gpa = 0;
            if (enrollments.Any(e => e.Grade.HasValue))
                gpa = (double)enrollments.Where(e => e.Grade.HasValue).Average(e => e.Grade!.Value);

            var attendanceCount = await db.AttendanceRecords
                .CountAsync(a => a.Enrollment.StudentId == studentId);
            var presentCount = await db.AttendanceRecords
                .CountAsync(a => a.Enrollment.StudentId == studentId && a.Present);
            
            double attendanceRate = attendanceCount > 0 ? (double)presentCount / attendanceCount * 100 : 0;

            var assignmentCount = await db.Submissions.CountAsync(s => s.StudentId == student.UserId);

            var grades = new List<GradeDistributionDto>(); 
            
            return Ok(new ChildOverviewDto(
                student.Id,
                student.User.FullName,
                Math.Round(gpa, 2),
                Math.Round(attendanceRate, 1),
                assignmentCount,
                grades
            ));
        }
    }
}
