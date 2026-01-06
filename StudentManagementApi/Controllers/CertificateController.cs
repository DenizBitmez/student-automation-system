using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using StudentManagementApi.Services;
using System.Security.Claims;

namespace StudentManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CertificateController(AppDbContext db, IPdfExportService pdfService, IConfiguration cfg) : ControllerBase
    {
        [HttpGet("student-certificate")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetStudentCertificate()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var student = await db.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null) return NotFound("Student not found");

            string baseUrl = cfg["AppBaseUrl"] ?? "http://localhost:5004";
            string verificationUrl = $"{baseUrl}/verify/cert/{student.Id}-{Guid.NewGuid().ToString().Substring(0,8)}";

            var pdf = await pdfService.GenerateStudentCertificateAsync(student.Id, verificationUrl);
            return File(pdf, "application/pdf", "Ogrenci_Belgesi.pdf");
        }

        [HttpGet("transcript")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetTranscript()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var student = await db.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null) return NotFound("Student not found");

            string baseUrl = cfg["AppBaseUrl"] ?? "http://localhost:5004";
            string verificationUrl = $"{baseUrl}/verify/transcript/{student.Id}-{Guid.NewGuid().ToString().Substring(0,8)}";

            var pdf = await pdfService.GenerateTranscriptAsync(student.Id, verificationUrl);
            return File(pdf, "application/pdf", "Transcript.pdf");
        }
    }
}
