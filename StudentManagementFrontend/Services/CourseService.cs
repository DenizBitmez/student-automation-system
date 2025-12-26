using System.Net.Http.Json;
using StudentManagementFrontend.Models;

namespace StudentManagementFrontend.Services;

public class CourseService : ICourseService
{
    private readonly HttpClient _httpClient;
    private const string BasePath = "api/course";

    public CourseService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<IEnumerable<Course>> GetAllCoursesAsync()
    {
        try
        {
            var vms = await _httpClient.GetFromJsonAsync<IEnumerable<CourseVm>>(BasePath) ?? new List<CourseVm>();
            return vms.Select(MapToCourse);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching courses: {ex.Message}");
            return new List<Course>();
        }
    }

    public async Task<Course?> GetCourseByIdAsync(int id)
    {
        try
        {
            var vm = await _httpClient.GetFromJsonAsync<CourseVm>($"{BasePath}/{id}");
            return vm != null ? MapToCourse(vm) : null;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching course with ID {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> AddCourseAsync(Course course)
    {
         try {
             await CreateCourseAsync(course);
             return true;
         } catch { return false; }
    }

    public async Task<Course> CreateCourseAsync(Course course)
    {
        try
        {
            var createDto = new 
            {
                Code = course.Code,
                Name = course.Name,
                TeacherId = course.TeacherId
            };
            
            var response = await _httpClient.PostAsJsonAsync(BasePath, createDto);
            response.EnsureSuccessStatusCode();
            var vm = await response.Content.ReadFromJsonAsync<CourseVm>();
            if (vm == null) throw new Exception("Failed to deserialize course");
            return MapToCourse(vm);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating course: {ex.Message}");
            throw;
        }
    }

    private static Course MapToCourse(CourseVm vm)
    {
        return new Course
        {
            Id = vm.Id,
            Code = vm.Code,
            Name = vm.Name,
            TeacherId = vm.TeacherId,
            TeacherName = vm.TeacherName,
            Credits = 5,
            Semester = "Fall",
            IsActive = vm.Status == CourseStatus.Active
        };
    }

    public async Task<bool> UpdateCourseAsync(int id, Course course)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BasePath}/{id}", course);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating course with ID {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteCourseAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BasePath}/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting course with ID {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<IEnumerable<Course>> GetCoursesByTeacherIdAsync(int teacherId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Course>>($"{BasePath}/teacher/{teacherId}") ?? new List<Course>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching courses for teacher {teacherId}: {ex.Message}");
            return new List<Course>();
        }
    }

    public async Task<IEnumerable<CourseStudentVm>> GetEnrolledStudentsAsync(int courseId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CourseStudentVm>>($"{BasePath}/{courseId}/students") ?? new List<CourseStudentVm>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching students for course {courseId}: {ex.Message}");
            return new List<CourseStudentVm>();
        }
    }

    public async Task<bool> EnrollStudentAsync(int courseId, int studentId)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{BasePath}/{courseId}/enroll/{studentId}", new { });
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error enrolling student {studentId} in course {courseId}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RemoveStudentFromCourseAsync(int courseId, int studentId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{BasePath}/{courseId}/students/{studentId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing student {studentId} from course {courseId}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateStudentGradeAsync(int courseId, int studentId, double grade)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BasePath}/{courseId}/students/{studentId}/grade", new { grade });
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating grade for student {studentId} in course {courseId}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateCourseStatusAsync(int courseId, bool isActive)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BasePath}/{courseId}/status", new { isActive });
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating status for course {courseId}: {ex.Message}");
            return false;
        }
    }

    public async Task<int> GetEnrolledStudentCountAsync(int courseId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<int>($"{BasePath}/{courseId}/students/count");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting student count for course {courseId}: {ex.Message}");
            return 0;
        }
    }

    public async Task<int> GetActiveCourseCountAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<int>($"{BasePath}/count/active");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting active course count: {ex.Message}");
            return 0;
        }
    }

    public async Task<int> GetTotalCourseCountAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<int>($"{BasePath}/count/total");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting total course count: {ex.Message}");
            return 0;
        }
    }

    public async Task<IEnumerable<Course>> SearchCoursesAsync(string searchTerm)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Course>>($"{BasePath}/search?q={Uri.EscapeDataString(searchTerm)}") ?? new List<Course>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error searching courses with term '{searchTerm}': {ex.Message}");
            return new List<Course>();
        }
    }

    public async Task<IEnumerable<Course>> GetCoursesBySemesterAsync(string semester)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Course>>($"{BasePath}/semester/{Uri.EscapeDataString(semester)}") ?? new List<Course>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching courses for semester {semester}: {ex.Message}");
            return new List<Course>();
        }
    }

    public async Task<IEnumerable<Course>> GetActiveCoursesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Course>>($"{BasePath}/active") ?? new List<Course>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching active courses: {ex.Message}");
            return new List<Course>();
        }
    }
}
