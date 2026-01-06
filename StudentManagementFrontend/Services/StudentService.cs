using System.Net.Http.Json;
using StudentManagementFrontend.Models;

namespace StudentManagementFrontend.Services;

public class StudentService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "api/student";

    public StudentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Student>> GetStudentsAsync()
    {
        var vms = await _httpClient.GetFromJsonAsync<List<StudentVm>>(BaseUrl) ?? new List<StudentVm>();
        return vms.Select(MapToStudent).ToList();
    }

    public async Task<IEnumerable<Student>> GetAllStudentsAsync()
    {
        var vms = await _httpClient.GetFromJsonAsync<List<StudentVm>>(BaseUrl) ?? new List<StudentVm>();
        return vms.Select(MapToStudent);
    }

    public async Task<Student?> GetStudentAsync(int id)
    {
        var vm = await _httpClient.GetFromJsonAsync<StudentVm>($"{BaseUrl}/{id}");
        return vm != null ? MapToStudent(vm) : null;
    }

    public async Task<Student?> GetStudentByIdAsync(int id)
    {
        try
        {
            var vm = await _httpClient.GetFromJsonAsync<StudentVm>($"{BaseUrl}/{id}");
            return vm != null ? MapToStudent(vm) : null;
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

    public async Task<Student?> AddStudentAsync(Student student)
    {
        var createDto = new
        {
             Email = student.Email,
             FullName = $"{student.FirstName} {student.LastName}".Trim(),
             Password = "Password123!" // Default
        };
        
        var response = await _httpClient.PostAsJsonAsync(BaseUrl, createDto);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Server Error ({response.StatusCode}): {errorContent}");
        }
        var vm = await response.Content.ReadFromJsonAsync<StudentVm>();
        return vm != null ? MapToStudent(vm) : null;
    }
    
    private static Student MapToStudent(StudentVm vm)
    {
        // Split FullName if possible, or leave/use FullName property in Model
        var parts = vm.FullName.Split(' ', 2);
        return new Student
        {
            Id = vm.Id,
            StudentNumber = vm.StudentNumber,
            Email = vm.Email,
            FullName = vm.FullName,
            FirstName = parts.Length > 0 ? parts[0] : "",
            LastName = parts.Length > 1 ? parts[1] : "",
            EnrolledAt = vm.EnrolledAt
        };
    }

    public async Task UpdateStudentAsync(Student student)
    {
        var updateDto = new 
        {
            FullName = string.IsNullOrEmpty(student.FullName) ? $"{student.FirstName} {student.LastName}".Trim() : student.FullName
        };
        var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{student.Id}", updateDto);
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
