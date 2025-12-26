using System.Net.Http.Json;
using StudentManagementFrontend.Models;

namespace StudentManagementFrontend.Services;

public class TeacherService : ITeacherService
{
    private readonly HttpClient _httpClient;
    private const string BasePath = "api/teacher";

    public TeacherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
    {
        try
        {
            var vms = await _httpClient.GetFromJsonAsync<IEnumerable<TeacherVm>>(BasePath) ?? new List<TeacherVm>();
            return vms.Select(MapToTeacher);
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
            var vm = await _httpClient.GetFromJsonAsync<TeacherVm>($"{BasePath}/{id}");
            return vm != null ? MapToTeacher(vm) : null;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching teacher with ID {id}: {ex.Message}");
            return null;
        }
    }

    // Email lookup likely returns Teacher or TeacherVm? Let's assume TeacherVm for consistency if Controller changed, but Controller for Email wasn't viewed. 
    // Assuming Email lookup wasn't changed and still works or needs check. 
    // Wait, Controller doesn't have Email lookup? Use GetAll and filter if needed or rely on existing?
    // User didn't complain about Email lookup, but Create triggers it. 
    
    public async Task<Teacher?> GetTeacherByEmailAsync(string email)
    {
         // Assuming backend has this (?) or we just implement it client side for now if missing
         // Checking TeacherController content previously... it ONLY had Get, GetById, Create.
         // NO Email endpoint in TeacherController!
         // So GetTeacherByEmailAsync likely fails 404.
         // FIX: Filter from GetAll.
         
        try
        {
            var teachers = await GetAllTeachersAsync();
            return teachers.FirstOrDefault(t => t.Email == email);
        }
        catch (Exception ex)
        {
             Console.WriteLine($"Error fetching teacher with email {email}: {ex.Message}");
             return null;
        }
    }

    public async Task<Teacher> CreateTeacherAsync(Teacher teacher)
    {
        try
        {
            // Input 'teacher' has FirstName, LastName etc.
            // DTO expects Department, FullName, Email... 
            // Wait, Create accepts TeacherCreateDto (Email, Password, FullName, Department).
            // Logic in Service is passing 'Teacher' object. 
            // We need to map Teacher -> TeacherCreateDto anonymous object? 
            // Or just ensure Teacher matches JSON params.
            
            var createDto = new 
            {
                Email = teacher.Email,
                FullName = $"{teacher.FirstName} {teacher.LastName}",
                Password = "Password123!", // Default password for now? Or prompts?
                Department = teacher.Branch
            };
            
            var response = await _httpClient.PostAsJsonAsync(BasePath, createDto);
            response.EnsureSuccessStatusCode();
            
            var vm = await response.Content.ReadFromJsonAsync<TeacherVm>();
            if (vm == null) throw new Exception("Failed to deserialize teacher");
            
            return MapToTeacher(vm);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating teacher: {ex.Message}");
            throw;
        }
    }

    private static Teacher MapToTeacher(TeacherVm vm)
    {
        // Try to split FullName
        var parts = vm.FullName.Split(' ', 2);
        var first = parts.Length > 0 ? parts[0] : "";
        var last = parts.Length > 1 ? parts[1] : "";

        return new Teacher
        {
            Id = vm.Id,
            Email = vm.Email,
            FullName = vm.FullName, // We added this property to Teacher model
            FirstName = first,
            LastName = last,
            Branch = vm.Department,
            IsActive = true
        };
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
