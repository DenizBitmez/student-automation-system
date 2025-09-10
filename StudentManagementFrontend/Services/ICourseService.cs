using StudentManagementFrontend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagementFrontend.Services
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<Course> GetCourseByIdAsync(int id);
        Task<bool> AddCourseAsync(Course course);
        Task<bool> UpdateCourseAsync(int id, Course course);
        Task<bool> DeleteCourseAsync(int id);
        Task<IEnumerable<Student>> GetEnrolledStudentsAsync(int courseId);
        Task<bool> EnrollStudentAsync(int courseId, int studentId);
        Task<bool> RemoveStudentFromCourseAsync(int courseId, int studentId);
        Task<bool> UpdateStudentGradeAsync(int courseId, int studentId, double grade);
        Task<IEnumerable<Course>> SearchCoursesAsync(string searchTerm);
        Task<IEnumerable<Course>> GetCoursesByTeacherIdAsync(int teacherId);
        Task<IEnumerable<Course>> GetActiveCoursesAsync();
        Task<int> GetEnrolledStudentCountAsync(int courseId);
    }
}
