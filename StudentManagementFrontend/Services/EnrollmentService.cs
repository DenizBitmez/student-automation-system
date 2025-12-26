using System.Net.Http.Json;

namespace StudentManagementFrontend.Services;

public interface IEnrollmentService
{
    Task<bool> EnrollStudentAsync(int courseId, int studentId);
    Task<bool> UnenrollStudentAsync(int courseId, int studentId);
}

public class EnrollmentService : IEnrollmentService
{
    private readonly HttpClient _httpClient;
    private const string BasePath = "api/enrollment";

    public EnrollmentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> EnrollStudentAsync(int courseId, int studentId)
    {
        try
        {
            var dto = new { CourseId = courseId, StudentId = studentId };
            var response = await _httpClient.PostAsJsonAsync(BasePath, dto);
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                // Already enrolled
                return false;
            }
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error enrolling student: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UnenrollStudentAsync(int courseId, int studentId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BasePath}?courseId={courseId}&studentId={studentId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error unenrolling student: {ex.Message}");
            return false;
        }
    }
}
