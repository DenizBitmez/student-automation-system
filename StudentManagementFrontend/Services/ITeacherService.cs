using StudentManagementFrontend.Models;

namespace StudentManagementFrontend.Services;

public interface ITeacherService
{
    Task<IEnumerable<Teacher>> GetAllTeachersAsync();
    Task<Teacher?> GetTeacherByIdAsync(int id);
    Task<Teacher> CreateTeacherAsync(Teacher teacher);
    Task<bool> UpdateTeacherAsync(int id, Teacher teacher);
    Task<bool> DeleteTeacherAsync(int id);
    Task<IEnumerable<Course>> GetTeacherCoursesAsync(int teacherId);
    Task<int> GetTeacherCountAsync();
}
