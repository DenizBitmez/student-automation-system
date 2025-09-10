using System.Net.Http.Json;
using StudentManagementFrontend.Models;

namespace StudentManagementFrontend.Services;

public class TeacherService : ITeacherService
{
    private readonly HttpClient _httpClient;
    private const string BasePath = "api/teachers";

    public TeacherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Teacher>>(BasePath) ?? new List<Teacher>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching teachers: {ex.Message}");
            return new List<Teacher>();
        }
    }

    public async Task<Teacher?> GetTeacherByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Teacher>($"{BasePath}/{id}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching teacher with ID {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<Teacher> CreateTeacherAsync(Teacher teacher)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(BasePath, teacher);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Teacher>() ?? throw new Exception("Failed to deserialize teacher");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating teacher: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> UpdateTeacherAsync(int id, Teacher teacher)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BasePath}/{id}", teacher);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error updating teacher with ID {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteTeacherAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BasePath}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error deleting teacher with ID {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<IEnumerable<Course>> GetTeacherCoursesAsync(int teacherId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Course>>($"{BasePath}/{teacherId}/courses") ?? new List<Course>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching courses for teacher with ID {teacherId}: {ex.Message}");
            return new List<Course>();
        }
    }

    public async Task<int> GetTeacherCountAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<int>($"{BasePath}/count");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching teacher count: {ex.Message}");
            return 0;
        }
    }
}
