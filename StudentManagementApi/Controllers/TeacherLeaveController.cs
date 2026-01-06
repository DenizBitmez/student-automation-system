using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using StudentManagementApi.Domain;
using StudentManagementApi.Dtos;
using StudentManagementApi.Services;
using System.Security.Claims;
using System.Collections.Generic;

namespace StudentManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeacherLeaveController(AppDbContext db, INotificationService notificationService) : ControllerBase
{
    [HttpGet("my")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<IEnumerable<TeacherLeaveViewDto>>> GetMyLeaves()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var teacher = await db.Teachers.Include(t => t.User).FirstOrDefaultAsync(t => t.UserId == userId);
        if (teacher == null) return NotFound("Öğretmen bulunamadı.");

        var teacherName = teacher.User?.FullName ?? "Öğretmen";

        var leaves = await db.TeacherLeaves
            .Where(l => l.TeacherId == teacher.Id)
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new TeacherLeaveViewDto(
                l.Id,
                l.TeacherId,
                teacherName,
                l.StartDate,
                l.EndDate,
                l.Reason,
                l.Status,
                l.CreatedAt))
            .ToListAsync();

        return Ok(leaves);
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> CreateLeaveRequest(TeacherLeaveCreateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var teacher = await db.Teachers.Include(t => t.User).FirstOrDefaultAsync(t => t.UserId == userId);
        if (teacher == null) return NotFound("Öğretmen bulunamadı.");

        var leave = new TeacherLeave
        {
            TeacherId = teacher.Id,
            StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc),
            EndDate = DateTime.SpecifyKind(dto.EndDate, DateTimeKind.Utc),
            Reason = dto.Reason,
            Status = "Pending"
        };

        db.TeacherLeaves.Add(leave);
        await db.SaveChangesAsync();

        await notificationService.SendTeacherLeaveNotificationAsync(
            $"{teacher.User.FullName}",
            leave.StartDate,
            leave.EndDate);

        return Ok();
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<TeacherLeaveViewDto>>> GetAllLeaves()
    {
        var leaves = await db.TeacherLeaves
            .Include(l => l.Teacher)
            .ThenInclude(t => t.User)
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new TeacherLeaveViewDto(
                l.Id,
                l.TeacherId,
                $"{l.Teacher.User.FullName}",
                l.StartDate,
                l.EndDate,
                l.Reason,
                l.Status,
                l.CreatedAt))
            .ToListAsync();

        return Ok(leaves);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateLeaveStatus(int id, TeacherLeaveUpdateDto dto)
    {
        var leave = await db.TeacherLeaves
            .Include(l => l.Teacher)
            .FirstOrDefaultAsync(l => l.Id == id);
            
        if (leave == null) return NotFound("İzin talebi bulunamadı.");

        leave.Status = dto.Status;
        await db.SaveChangesAsync();

        await notificationService.SendTeacherLeaveStatusNotificationAsync(
            leave.Teacher.UserId,
            leave.Status,
            leave.StartDate);

        return Ok();
    }
    
    [HttpGet("teacher/{teacherId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<TeacherLeaveViewDto>>> GetTeacherLeaves(int teacherId)
    {
        var leaves = await db.TeacherLeaves
            .Where(l => l.TeacherId == teacherId)
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new TeacherLeaveViewDto(
                l.Id,
                l.TeacherId,
                "", // Not needed in this context
                l.StartDate,
                l.EndDate,
                l.Reason,
                l.Status,
                l.CreatedAt))
            .ToListAsync();

        return Ok(leaves);
    }
}
