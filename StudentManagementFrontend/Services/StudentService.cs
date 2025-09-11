using System.Net.Http.Json;
using StudentManagementFrontend.Models;

namespace StudentManagementFrontend.Services;

public class StudentService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "api/ogrenciler";

    public StudentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Student>> GetStudentsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Student>>(BaseUrl) ?? new List<Student>();
    }

    public async Task<IEnumerable<Student>> GetAllStudentsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Student>>(BaseUrl) ?? new List<Student>();
    }

    public async Task<Student> GetStudentAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Student>($"{BaseUrl}/{id}");
    }

    public async Task<Student?> GetStudentByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Student>($"{BaseUrl}/{id}");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching student with ID {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<Student> AddStudentAsync(Student student)
    {
        var response = await _httpClient.PostAsJsonAsync(BaseUrl, student);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Student>();
    }

    public async Task UpdateStudentAsync(Student student)
    {
        var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{student.Id}", student);
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting student with ID {id}: {ex.Message}");
            return false;
        }
    }
}
