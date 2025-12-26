using Microsoft.JSInterop;

namespace StudentManagementFrontend.Services;

public interface IReportService
{
    Task DownloadStudentListPdfAsync();
    Task DownloadStudentGradesPdfAsync(int studentId);
    Task DownloadStudentAttendancePdfAsync(int studentId);
    Task DownloadCourseReportPdfAsync(int courseId);
}

public class ReportService : IReportService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly string _baseUrl;

    // We inject HttpClient to get BaseAddress, or Config
    public ReportService(IJSRuntime jsRuntime, HttpClient httpClient)
    {
        _jsRuntime = jsRuntime;
        _baseUrl = httpClient.BaseAddress?.ToString() ?? "http://localhost:5274/";
    }

    private string GetFullUrl(string relativePath) => $"{_baseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";

    public async Task DownloadStudentListPdfAsync()
    {
        var url = GetFullUrl("api/reports/students/pdf");
        await _jsRuntime.InvokeVoidAsync("open", url, "_blank");
    }

    public async Task DownloadStudentGradesPdfAsync(int studentId)
    {
        var url = GetFullUrl($"api/reports/students/{studentId}/grades/pdf");
        await _jsRuntime.InvokeVoidAsync("open", url, "_blank");
    }

    public async Task DownloadStudentAttendancePdfAsync(int studentId)
    {
        var url = GetFullUrl($"api/reports/students/{studentId}/attendance/pdf");
        await _jsRuntime.InvokeVoidAsync("open", url, "_blank");
    }

    public async Task DownloadCourseReportPdfAsync(int courseId)
    {
        var url = GetFullUrl($"api/reports/courses/{courseId}/pdf");
        await _jsRuntime.InvokeVoidAsync("open", url, "_blank");
    }
}
