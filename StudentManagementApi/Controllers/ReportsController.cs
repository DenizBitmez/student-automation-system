using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementApi.Services;

namespace StudentManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IPdfExportService _pdfExportService;

    public ReportsController(IPdfExportService pdfExportService)
    {
        _pdfExportService = pdfExportService;
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
}