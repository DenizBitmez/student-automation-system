using Microsoft.JSInterop;

namespace StudentManagementFrontend.Services;

public interface IReportService
{
    Task DownloadStudentListPdfAsync();
    Task DownloadStudentGradesPdfAsync(int studentId);
    Task DownloadStudentAttendancePdfAsync(int studentId);
    Task DownloadCourseReportPdfAsync(int courseId);
    Task DownloadMyGradesPdfAsync();
    Task DownloadMyAttendancePdfAsync();
}

public class ReportService : IReportService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly HttpClient _httpClient;

    public ReportService(IJSRuntime jsRuntime, HttpClient httpClient)
    {
        _jsRuntime = jsRuntime;
        _httpClient = httpClient;
    }

    private async Task DownloadFileAsync(string relativePath, string fileName)
    {
        try
        {
            var data = await _httpClient.GetByteArrayAsync(relativePath);
            await _jsRuntime.InvokeVoidAsync("downloadFile", fileName, "application/pdf", data);
        }
        catch (Exception ex)
        {
            await _jsRuntime.InvokeVoidAsync("alert", $"Dosya indirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task DownloadStudentListPdfAsync()
    {
        await DownloadFileAsync("api/reports/students/pdf", "ogrenci_listesi.pdf");
    }

    public async Task DownloadStudentGradesPdfAsync(int studentId)
    {
        await DownloadFileAsync($"api/reports/students/{studentId}/grades/pdf", $"karne_{studentId}.pdf");
    }

    public async Task DownloadStudentAttendancePdfAsync(int studentId)
    {
        await DownloadFileAsync($"api/reports/students/{studentId}/attendance/pdf", $"devamsizlik_{studentId}.pdf");
    }

    public async Task DownloadCourseReportPdfAsync(int courseId)
    {
        await DownloadFileAsync($"api/reports/courses/{courseId}/pdf", $"ders_raporu_{courseId}.pdf");
    }

    public async Task DownloadMyGradesPdfAsync()
    {
        await DownloadFileAsync("api/reports/my/grades/pdf", "karne.pdf");
    }

    public async Task DownloadMyAttendancePdfAsync()
    {
        await DownloadFileAsync("api/reports/my/attendance/pdf", "devamsizlik_raporu.pdf");
    }
}
