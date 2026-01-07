using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using StudentManagementApi.Dtos;
using System.Security.Claims;
using System.Linq;
using StudentManagementApi.Domain;

namespace StudentManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnalyticsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("admin/stats")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SystemStatsDto>> GetSystemStats()
        {
            var stats = new SystemStatsDto
            {
                TotalStudents = await _context.Students.CountAsync(),
                TotalTeachers = await _context.Teachers.CountAsync(),
                TotalCourses = await _context.Courses.CountAsync(),
                TotalActiveEnrollments = await _context.Enrollments.CountAsync()
            };
            return Ok(stats);
        }

        [HttpGet("admin/enrollment-trends")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<EnrollmentTrendDto>>> GetEnrollmentTrends()
        {
            // Group by Month/Year
            var trends = await _context.Students
                .GroupBy(s => new { s.EnrolledAt.Year, s.EnrolledAt.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            var result = trends.Select(t => new EnrollmentTrendDto
            {
                Period = $"{t.Month}/{t.Year}",
                Count = t.Count
            }).ToList();

            return Ok(result);
        }

        [HttpGet("teacher/course-performance/{courseId}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<ActionResult<CoursePerformanceDto>> GetCoursePerformance(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) return NotFound();

            var enrollments = await _context.Enrollments.Where(e => e.CourseId == courseId).ToListAsync();
            
            if (!enrollments.Any())
            {
                return Ok(new CoursePerformanceDto 
                { 
                    CourseId = courseId, 
                    CourseName = course.Name,
                    AverageGrade = 0,
                    AttendanceRate = 0
                });
            }

            // Grade Stats
            var gradedEnrollments = enrollments.Where(e => e.Grade.HasValue).ToList();
            double avgGrade = gradedEnrollments.Any() ? (double)gradedEnrollments.Average(e => e.Grade!.Value) : 0;

            // Attendance Stats
            // This is computationally expensive if not optimized, but simple for now
            // We need total scheduled hours vs attended hours for all students in this course
            // Simplified: Fetch logic later if needed. For now returning 0 or mock.
            // Let's try to fetch if possible.
            // Actually, we can just get average of all attendance records for this course?
            // "AttendanceRate" usually means Present / Total Sessions.
            
            var attendanceRecords = await _context.AttendanceRecords
                .Include(ar => ar.Enrollment)
                .Where(ar => ar.Enrollment.CourseId == courseId)
                .ToListAsync();
            
            double attendanceRate = 0;
            if (attendanceRecords.Any())
            {
                int presentCount = attendanceRecords.Count(a => a.Present);
                attendanceRate = (double)presentCount / attendanceRecords.Count() * 100;
            }

            var result = new CoursePerformanceDto
            {
                CourseId = courseId,
                CourseName = course.Name,
                AverageGrade = avgGrade,
                AttendanceRate = attendanceRate,
                GradeDistribution = new List<GradeDistributionDto>
                {
                    new GradeDistributionDto { Label = "0-49 (Fail)", Count = gradedEnrollments.Count(e => e.Grade < 50) },
                    new GradeDistributionDto { Label = "50-69", Count = gradedEnrollments.Count(e => e.Grade >= 50 && e.Grade < 70) },
                    new GradeDistributionDto { Label = "70-84", Count = gradedEnrollments.Count(e => e.Grade >= 70 && e.Grade < 85) },
                    new GradeDistributionDto { Label = "85-100", Count = gradedEnrollments.Count(e => e.Grade >= 85) }
                }
            };

            return Ok(result);
        }

        [HttpGet("student/performance/{studentId}")]
        [Authorize(Roles = "Student,Admin,Teacher")]
        public async Task<ActionResult<StudentPerformanceDto>> GetStudentPerformance(int studentId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            Student? student;

            if (studentId == 0)
            {
                // Resolve current user's student profile
                student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == currentUserId);
            }
            else
            {
                student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            }

            if (student == null) return NotFound("Student profile not found.");

            // Security check
            if (User.IsInRole("Student") && student.UserId != currentUserId)
            {
                return Forbid();
            }

            // Update studentId to the actual ID in case it was 0s
            studentId = student.Id;

            var enrollments = await _context.Enrollments.Where(e => e.StudentId == studentId).ToListAsync();
            
            double gpa = 0;
            if (enrollments.Any(e => e.Grade.HasValue))
            {
                gpa = (double)enrollments.Where(e => e.Grade.HasValue).Average(e => e.Grade!.Value);
            }

            // Attendance
             var attendanceRecords = await _context.AttendanceRecords
                .Include(ar => ar.Enrollment)
                .Where(ar => ar.Enrollment.StudentId == studentId)
                .ToListAsync();
             
             double attendanceRate = 0;
             if (attendanceRecords.Any())
             {
                 attendanceRate = (double)attendanceRecords.Count(a => a.Present) / attendanceRecords.Count() * 100;
             }
             
             // Assignments
             // Submission.StudentId is a string (UserId), so we use student.UserId
             var submissions = await _context.Submissions.Where(s => s.StudentId == student.UserId).ToListAsync();
             
             return Ok(new StudentPerformanceDto
             {
                 StudentId = studentId,
                 GPA = gpa,
                 OverallAttendanceRate = attendanceRate,
                 CompletedAssignments = submissions.Count(),
                 PendingAssignments = 0 
             });
        }
    }
}
