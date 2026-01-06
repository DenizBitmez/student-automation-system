using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using StudentManagementApi.Domain;
using System.Security.Claims;
using static StudentManagementApi.Dtos.ScheduleDtos;

namespace StudentManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ScheduleController(AppDbContext db) : ControllerBase
    {
        [HttpGet("my-schedule")]
        public async Task<ActionResult<IEnumerable<ScheduleItemVm>>> GetMySchedule()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            IQueryable<ScheduleItem> query = db.ScheduleItems
                .Include(s => s.Course)
                .ThenInclude(c => c.Teacher)
                .ThenInclude(t => t.User);

            if (User.IsInRole("Student"))
            {
                var student = await db.Students.FirstOrDefaultAsync(s => s.UserId == userId);
                if (student == null) return NotFound("Student profile not found");

                var enrolledCourseIds = await db.Enrollments
                    .Where(e => e.StudentId == student.Id)
                    .Select(e => e.CourseId)
                    .ToListAsync();
                
                query = query.Where(s => enrolledCourseIds.Contains(s.CourseId));
            }
            else if (User.IsInRole("Teacher"))
            {
                var teacher = await db.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
                if (teacher == null) return NotFound("Teacher profile not found");

                query = query.Where(s => s.Course.TeacherId == teacher.Id);
            }

            return await query
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .Select(s => new ScheduleItemVm(
                    s.Id,
                    s.CourseId,
                    s.Course.Name,
                    s.Course.Code,
                    s.Course.Teacher.User.FullName ?? "Teacher",
                    s.DayOfWeek,
                    s.StartTime.ToString(@"hh\:mm"),
                    s.EndTime.ToString(@"hh\:mm"),
                    s.Classroom
                ))
                .ToListAsync();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ScheduleItemVm>> Create(ScheduleItemCreateDto dto)
        {
            if (!TimeSpan.TryParse(dto.StartTime, out var startTime) || 
                !TimeSpan.TryParse(dto.EndTime, out var endTime))
            {
                return BadRequest("Invalid time format (HH:mm)");
            }

            var course = await db.Courses.Include(c => c.Teacher).ThenInclude(t => t.User).FirstOrDefaultAsync(c => c.Id == dto.CourseId);
            if (course == null) return BadRequest("Invalid course");

            var item = new ScheduleItem
            {
                CourseId = dto.CourseId,
                DayOfWeek = dto.DayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                Classroom = dto.Classroom ?? ""
            };

            db.ScheduleItems.Add(item);
            await db.SaveChangesAsync();

            return Ok(new ScheduleItemVm(
                item.Id,
                item.CourseId,
                course.Name,
                course.Code,
                course.Teacher.User.FullName ?? "Teacher",
                item.DayOfWeek,
                item.StartTime.ToString(@"hh\:mm"),
                item.EndTime.ToString(@"hh\:mm"),
                item.Classroom
            ));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await db.ScheduleItems.FindAsync(id);
            if (item == null) return NotFound();

            db.ScheduleItems.Remove(item);
            await db.SaveChangesAsync();

            return NoContent();
        }
    }
}
