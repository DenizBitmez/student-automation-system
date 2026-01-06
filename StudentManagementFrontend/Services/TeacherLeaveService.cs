using System.Net.Http.Json;
using StudentManagementFrontend.Models;

namespace StudentManagementFrontend.Services;

public interface ITeacherLeaveService
{
    Task<List<TeacherLeaveVm>> GetMyLeavesAsync();
    Task CreateLeaveRequestAsync(TeacherLeaveCreateVm model);
    Task<List<TeacherLeaveVm>> GetAllLeavesAsync();
    Task UpdateLeaveStatusAsync(int id, string status);
}

public class TeacherLeaveService(HttpClient http) : ITeacherLeaveService
{
    public async Task<List<TeacherLeaveVm>> GetMyLeavesAsync()
    {
        return await http.GetFromJsonAsync<List<TeacherLeaveVm>>("api/TeacherLeave/my") ?? new();
    }

    public async Task CreateLeaveRequestAsync(TeacherLeaveCreateVm model)
    {
        var response = await http.PostAsJsonAsync("api/TeacherLeave", model);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<TeacherLeaveVm>> GetAllLeavesAsync()
    {
        return await http.GetFromJsonAsync<List<TeacherLeaveVm>>("api/TeacherLeave/all") ?? new();
    }

    public async Task UpdateLeaveStatusAsync(int id, string status)
    {
        var response = await http.PutAsJsonAsync($"api/TeacherLeave/{id}/status", new { status });
        response.EnsureSuccessStatusCode();
    }
}
