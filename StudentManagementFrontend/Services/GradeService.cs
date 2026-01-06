using System.Net.Http.Json;
using StudentManagementFrontend.Models;

namespace StudentManagementFrontend.Services;

public interface IGradeService
{
    Task<bool> SetGradeAsync(int enrollmentId, decimal grade, string? comment);
    Task<IEnumerable<GradeDto>> GetGradesByStudentAsync(int studentId);
    Task<IEnumerable<GradeDto>> GetMyGradesAsync();
}

public class GradeService : IGradeService
{
    private readonly HttpClient _httpClient;
    private const string BasePath = "api/grade";

    public GradeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> SetGradeAsync(int enrollmentId, decimal grade, string? comment)
    {
        try
        {
            var dto = new { EnrollmentId = enrollmentId, Grade = grade, Comment = comment };
            var response = await _httpClient.PostAsJsonAsync(BasePath, dto);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting grade: {ex.Message}");
            return false;
        }
    }

    public async Task<IEnumerable<GradeDto>> GetGradesByStudentAsync(int studentId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<GradeDto>>($"{BasePath}/by-student/{studentId}") ?? new List<GradeDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching grades for student {studentId}: {ex.Message}");
            return new List<GradeDto>();
        }
    }

    public async Task<IEnumerable<GradeDto>> GetMyGradesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<GradeDto>>($"{BasePath}/my") ?? new List<GradeDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching my grades: {ex.Message}");
            return new List<GradeDto>();
        }
    }
}

public class GradeDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal? Grade { get; set; }
    public string? Comment { get; set; }
}
