using System.Net.Http.Json;

namespace StudentManagementFrontend.Services;

public interface IAttendanceService
{
    Task<bool> TickAttendanceAsync(int enrollmentId, bool present);
    Task<IEnumerable<AttendanceRecordDto>> GetAttendanceByStudentAsync(int studentId);
}

public class AttendanceService : IAttendanceService
{
    private readonly HttpClient _httpClient;
    private const string BasePath = "api/attendance";

    public AttendanceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> TickAttendanceAsync(int enrollmentId, bool present)
    {
        try
        {
            var response = await _httpClient.PostAsync($"{BasePath}/tick/{enrollmentId}?present={present}", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error ticking attendance: {ex.Message}");
            return false;
        }
    }

    public async Task<IEnumerable<AttendanceRecordDto>> GetAttendanceByStudentAsync(int studentId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<AttendanceRecordDto>>($"{BasePath}/by-student/{studentId}") ?? new List<AttendanceRecordDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching attendance for student {studentId}: {ex.Message}");
            return new List<AttendanceRecordDto>();
        }
    }
}

public class AttendanceRecordDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public bool Present { get; set; }
    public string Name { get; set; } = string.Empty; // Course Name
}
