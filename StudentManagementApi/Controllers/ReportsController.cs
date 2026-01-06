using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using StudentManagementApi.Services;

namespace StudentManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IPdfExportService _pdfExportService;
    private readonly AppDbContext _db;

    public ReportsController(IPdfExportService pdfExportService, AppDbContext db)
    {
        _pdfExportService = pdfExportService;
        _db = db;
    }

    [HttpGet("students/pdf")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> ExportStudentsToPdf()
    {
        try
        {
            var pdfBytes = await _pdfExportService.ExportStudentsToPdfAsync();
            var fileName = $"ogrenci_listesi_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "PDF oluşturulurken hata oluştu.", error = ex.Message });
        }
    }

    [HttpGet("students/{studentId:int}/grades/pdf")]
    public async Task<IActionResult> ExportStudentGradesToPdf(int studentId)
    {
        try
        {
            var pdfBytes = await _pdfExportService.ExportGradeReportToPdfAsync(studentId);
            var fileName = $"not_raporu_ogrenci_{studentId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "PDF oluşturulurken hata oluştu.", error = ex.Message });
        }
    }

    [HttpGet("students/{studentId:int}/attendance/pdf")]
    public async Task<IActionResult> ExportStudentAttendanceToPdf(int studentId)
    {
        try
        {
            var pdfBytes = await _pdfExportService.ExportAttendanceReportToPdfAsync(studentId);
            var fileName = $"devamsizlik_raporu_ogrenci_{studentId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "PDF oluşturulurken hata oluştu.", error = ex.Message });
        }
    }

    [HttpGet("courses/{courseId:int}/pdf")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> ExportCourseReportToPdf(int courseId)
    {
        try
        {
            var pdfBytes = await _pdfExportService.ExportCourseReportToPdfAsync(courseId);
            var fileName = $"ders_raporu_{courseId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "PDF oluşturulurken hata oluştu.", error = ex.Message });
        }
    }

    [HttpGet("my/grades/pdf")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> ExportMyGradesToPdf()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var student = await _db.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null) return NotFound("Öğrenci bulunamadı.");

            var pdfBytes = await _pdfExportService.ExportGradeReportToPdfAsync(student.Id);
            return File(pdfBytes, "application/pdf", $"karne_{student.Id}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "PDF oluşturulurken hata oluştu.", error = ex.Message });
        }
    }

    [HttpGet("my/attendance/pdf")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> ExportMyAttendanceToPdf()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var student = await _db.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null) return NotFound("Öğrenci bulunamadı.");

            var pdfBytes = await _pdfExportService.ExportAttendanceReportToPdfAsync(student.Id);
            return File(pdfBytes, "application/pdf", $"devamsizlik_{student.Id}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "PDF oluşturulurken hata oluştu.", error = ex.Message });
        }
    }
}