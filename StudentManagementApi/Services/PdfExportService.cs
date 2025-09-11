using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using System.Text;

namespace StudentManagementApi.Services;

public interface IPdfExportService
{
    Task<byte[]> ExportStudentsToPdfAsync();
    Task<byte[]> ExportGradeReportToPdfAsync(int studentId);
    Task<byte[]> ExportAttendanceReportToPdfAsync(int studentId);
    Task<byte[]> ExportCourseReportToPdfAsync(int courseId);
}

public class PdfExportService : IPdfExportService
{
    private readonly AppDbContext _context;
    private readonly ILogger<PdfExportService> _logger;

    public PdfExportService(AppDbContext context, ILogger<PdfExportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<byte[]> ExportStudentsToPdfAsync()
    {
        var students = await _context.Students
            .Include(s => s.User)
            .OrderBy(s => s.User.FullName)
            .ToListAsync();

        using var stream = new MemoryStream();
        var document = new Document(PageSize.A4, 50, 50, 25, 25);
        var writer = PdfWriter.GetInstance(document, stream);

        document.Open();

        // Title
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, new BaseColor(64, 64, 64));
        var title = new Paragraph("ÖĞRENCİ LİSTESİ RAPORU", titleFont)
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 20
        };
        document.Add(title);

        // Date
        var dateFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, new BaseColor(128, 128, 128));
        var date = new Paragraph($"Rapor Tarihi: {DateTime.Now:dd/MM/yyyy HH:mm}", dateFont)
        {
            Alignment = Element.ALIGN_RIGHT,
            SpacingAfter = 20
        };
        document.Add(date);

        // Table
        var table = new PdfPTable(4) { WidthPercentage = 100 };
        table.SetWidths(new float[] { 1f, 3f, 3f, 2f });

        // Headers
        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, new BaseColor(255, 255, 255));
        var headerCells = new[] { "ID", "Ad Soyad", "E-posta", "Kayıt Tarihi" };
        
        foreach (var header in headerCells)
        {
            var cell = new PdfPCell(new Phrase(header, headerFont))
            {
                BackgroundColor = new BaseColor(64, 64, 64),
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 8
            };
            table.AddCell(cell);
        }

        // Data
        var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        foreach (var student in students)
        {
            table.AddCell(new PdfPCell(new Phrase(student.Id.ToString(), dataFont)) { Padding = 5 });
            table.AddCell(new PdfPCell(new Phrase(student.User.FullName ?? "", dataFont)) { Padding = 5 });
            table.AddCell(new PdfPCell(new Phrase(student.User.Email ?? "", dataFont)) { Padding = 5 });
            table.AddCell(new PdfPCell(new Phrase(student.EnrolledAt.ToString("dd/MM/yyyy"), dataFont)) { Padding = 5 });
        }

        document.Add(table);

        // Summary
        var summaryFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
        var summary = new Paragraph($"\nToplam Öğrenci Sayısı: {students.Count}", summaryFont)
        {
            SpacingBefore = 20
        };
        document.Add(summary);

        document.Close();
        writer.Close();

        return stream.ToArray();
    }

    public async Task<byte[]> ExportGradeReportToPdfAsync(int studentId)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null)
            throw new ArgumentException("Öğrenci bulunamadı");

        using var stream = new MemoryStream();
        var document = new Document(PageSize.A4, 50, 50, 25, 25);
        var writer = PdfWriter.GetInstance(document, stream);

        document.Open();

        // Header
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, new BaseColor(64, 64, 64));
        var title = new Paragraph("NOT RAPORU", titleFont)
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 20
        };
        document.Add(title);

        // Student Info
        var infoFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
        var studentInfo = new Paragraph($"Öğrenci: {student.User.FullName}\nE-posta: {student.User.Email}\nTarih: {DateTime.Now:dd/MM/yyyy}", infoFont)
        {
            SpacingAfter = 20
        };
        document.Add(studentInfo);

        // Grades Table
        var table = new PdfPTable(4) { WidthPercentage = 100 };
        table.SetWidths(new float[] { 2f, 3f, 1f, 2f });

        // Headers
        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, new BaseColor(255, 255, 255));
        var headers = new[] { "Ders Kodu", "Ders Adı", "Not", "Yorum" };
        
        foreach (var header in headers)
        {
            var cell = new PdfPCell(new Phrase(header, headerFont))
            {
                BackgroundColor = new BaseColor(64, 64, 64),
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 8
            };
            table.AddCell(cell);
        }

        // Data
        var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        var grades = new List<decimal>();
        
        foreach (var enrollment in student.Enrollments)
        {
            table.AddCell(new PdfPCell(new Phrase(enrollment.Course.Code, dataFont)) { Padding = 5 });
            table.AddCell(new PdfPCell(new Phrase(enrollment.Course.Name, dataFont)) { Padding = 5 });
            
            if (enrollment.Grade.HasValue)
            {
                table.AddCell(new PdfPCell(new Phrase(enrollment.Grade.Value.ToString("F1"), dataFont)) 
                { 
                    Padding = 5,
                    HorizontalAlignment = Element.ALIGN_CENTER
                });
                grades.Add(enrollment.Grade.Value);
            }
            else
            {
                table.AddCell(new PdfPCell(new Phrase("-", dataFont)) 
                { 
                    Padding = 5,
                    HorizontalAlignment = Element.ALIGN_CENTER
                });
            }
            
            table.AddCell(new PdfPCell(new Phrase(enrollment.Comment ?? "", dataFont)) { Padding = 5 });
        }

        document.Add(table);

        // Statistics
        if (grades.Any())
        {
            var statsFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var average = grades.Average();
            var passedCount = grades.Count(g => g >= 50);
            
            var stats = new Paragraph($"\nİstatistikler:\nNot Ortalaması: {average:F2}\nGeçilen Ders: {passedCount}/{grades.Count}\nBaşarı Oranı: %{(double)passedCount / grades.Count * 100:F1}", statsFont)
            {
                SpacingBefore = 20
            };
            document.Add(stats);
        }

        document.Close();
        writer.Close();

        return stream.ToArray();
    }

    public async Task<byte[]> ExportAttendanceReportToPdfAsync(int studentId)
    {
        var attendanceRecords = await _context.AttendanceRecords
            .Include(a => a.Enrollment)
            .ThenInclude(e => e.Student)
            .ThenInclude(s => s.User)
            .Include(a => a.Enrollment)
            .ThenInclude(e => e.Course)
            .Where(a => a.Enrollment.StudentId == studentId)
            .OrderByDescending(a => a.Date)
            .ToListAsync();

        var student = attendanceRecords.FirstOrDefault()?.Enrollment.Student;
        if (student == null)
            throw new ArgumentException("Öğrenci veya devamsızlık kaydı bulunamadı");

        using var stream = new MemoryStream();
        var document = new Document(PageSize.A4, 50, 50, 25, 25);
        var writer = PdfWriter.GetInstance(document, stream);

        document.Open();

        // Header
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, new BaseColor(64, 64, 64));
        var title = new Paragraph("DEVAMSIZLIK RAPORU", titleFont)
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 20
        };
        document.Add(title);

        // Student Info
        var infoFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
        var studentInfo = new Paragraph($"Öğrenci: {student.User.FullName}\nE-posta: {student.User.Email}\nTarih: {DateTime.Now:dd/MM/yyyy}", infoFont)
        {
            SpacingAfter = 20
        };
        document.Add(studentInfo);

        // Attendance Table
        var table = new PdfPTable(4) { WidthPercentage = 100 };
        table.SetWidths(new float[] { 3f, 2f, 2f, 1f });

        // Headers
        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, new BaseColor(255, 255, 255));
        var headers = new[] { "Ders", "Tarih", "Saat", "Durum" };
        
        foreach (var header in headers)
        {
            var cell = new PdfPCell(new Phrase(header, headerFont))
            {
                BackgroundColor = new BaseColor(64, 64, 64),
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 8
            };
            table.AddCell(cell);
        }

        // Data
        var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        var presentCount = 0;
        var absentCount = 0;
        
        foreach (var record in attendanceRecords)
        {
            table.AddCell(new PdfPCell(new Phrase(record.Enrollment.Course.Name, dataFont)) { Padding = 5 });
            table.AddCell(new PdfPCell(new Phrase(record.Date.ToString("dd/MM/yyyy"), dataFont)) { Padding = 5 });
            table.AddCell(new PdfPCell(new Phrase(record.Date.ToString("HH:mm"), dataFont)) { Padding = 5 });
            
            var status = record.Present ? "Katıldı" : "Katılmadı";
            var statusColor = record.Present ? new BaseColor(0, 128, 0) : new BaseColor(255, 0, 0);
            var statusCell = new PdfPCell(new Phrase(status, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, statusColor)))
            {
                Padding = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            };
            table.AddCell(statusCell);
            
            if (record.Present) presentCount++;
            else absentCount++;
        }

        document.Add(table);

        // Statistics
        if (attendanceRecords.Any())
        {
            var statsFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            var totalCount = attendanceRecords.Count;
            var attendanceRate = (double)presentCount / totalCount * 100;
            
            var stats = new Paragraph($"\nDevamsızlık İstatistikleri:\nToplam Ders: {totalCount}\nKatılım: {presentCount}\nDevamsızlık: {absentCount}\nKatılım Oranı: %{attendanceRate:F1}", statsFont)
            {
                SpacingBefore = 20
            };
            document.Add(stats);
        }

        document.Close();
        writer.Close();

        return stream.ToArray();
    }

    public async Task<byte[]> ExportCourseReportToPdfAsync(int courseId)
    {
        var course = await _context.Courses
            .Include(c => c.Teacher)
            .ThenInclude(t => t.User)
            .Include(c => c.Enrollments)
            .ThenInclude(e => e.Student)
            .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course == null)
            throw new ArgumentException("Ders bulunamadı");

        using var stream = new MemoryStream();
        var document = new Document(PageSize.A4, 50, 50, 25, 25);
        var writer = PdfWriter.GetInstance(document, stream);

        document.Open();

        // Header
        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, new BaseColor(64, 64, 64));
        var title = new Paragraph("DERS RAPORU", titleFont)
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 20
        };
        document.Add(title);

        // Course Info
        var infoFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
        var courseInfo = new Paragraph($"Ders: {course.Name} ({course.Code})\nÖğretmen: {course.Teacher.User.FullName}\nTarih: {DateTime.Now:dd/MM/yyyy}", infoFont)
        {
            SpacingAfter = 20
        };
        document.Add(courseInfo);

        // Students Table
        var table = new PdfPTable(4) { WidthPercentage = 100 };
        table.SetWidths(new float[] { 1f, 3f, 2f, 2f });

        // Headers
        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, new BaseColor(255, 255, 255));
        var headers = new[] { "ID", "Öğrenci", "Not", "Yorum" };
        
        foreach (var header in headers)
        {
            var cell = new PdfPCell(new Phrase(header, headerFont))
            {
                BackgroundColor = new BaseColor(64, 64, 64),
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 8
            };
            table.AddCell(cell);
        }

        // Data
        var dataFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
        var grades = new List<decimal>();
        
        foreach (var enrollment in course.Enrollments.OrderBy(e => e.Student.User.FullName))
        {
            table.AddCell(new PdfPCell(new Phrase(enrollment.Student.Id.ToString(), dataFont)) { Padding = 5 });
            table.AddCell(new PdfPCell(new Phrase(enrollment.Student.User.FullName, dataFont)) { Padding = 5 });
            
            if (enrollment.Grade.HasValue)
            {
                table.AddCell(new PdfPCell(new Phrase(enrollment.Grade.Value.ToString("F1"), dataFont)) 
                { 
                    Padding = 5,
                    HorizontalAlignment = Element.ALIGN_CENTER
                });
                grades.Add(enrollment.Grade.Value);
            }
            else
            {
                table.AddCell(new PdfPCell(new Phrase("-", dataFont)) 
                { 
                    Padding = 5,
                    HorizontalAlignment = Element.ALIGN_CENTER
                });
            }
            
            table.AddCell(new PdfPCell(new Phrase(enrollment.Comment ?? "", dataFont)) { Padding = 5 });
        }

        document.Add(table);

        // Course Statistics
        var statsFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
        var stats = new StringBuilder();
        stats.AppendLine($"\nDers İstatistikleri:");
        stats.AppendLine($"Toplam Öğrenci: {course.Enrollments.Count}");
        
        if (grades.Any())
        {
            var average = grades.Average();
            var passedCount = grades.Count(g => g >= 50);
            stats.AppendLine($"Not Ortalaması: {average:F2}");
            stats.AppendLine($"Geçen Öğrenci: {passedCount}/{grades.Count}");
            stats.AppendLine($"Başarı Oranı: %{(double)passedCount / grades.Count * 100:F1}");
        }
        
        var courseStats = new Paragraph(stats.ToString(), statsFont)
        {
            SpacingBefore = 20
        };
        document.Add(courseStats);

        document.Close();
        writer.Close();

        return stream.ToArray();
    }
}