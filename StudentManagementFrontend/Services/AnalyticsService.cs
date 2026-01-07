using System.Net.Http.Json;
using StudentManagementFrontend.Models;

namespace StudentManagementFrontend.Services
{
    public class AnalyticsService
    {
        private readonly HttpClient _httpClient;

        public AnalyticsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SystemStatsDto?> GetSystemStatsAsync()
        {
            return await _httpClient.GetFromJsonAsync<SystemStatsDto>("api/analytics/admin/stats");
        }

        public async Task<List<EnrollmentTrendDto>> GetEnrollmentTrendsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<EnrollmentTrendDto>>("api/analytics/admin/enrollment-trends") ?? new List<EnrollmentTrendDto>();
        }

        public async Task<CoursePerformanceDto?> GetCoursePerformanceAsync(int courseId)
        {
             return await _httpClient.GetFromJsonAsync<CoursePerformanceDto>($"api/analytics/teacher/course-performance/{courseId}");
        }

        public async Task<StudentPerformanceDto?> GetStudentPerformanceAsync(int studentId)
        {
             return await _httpClient.GetFromJsonAsync<StudentPerformanceDto>($"api/analytics/student/performance/{studentId}");
        }

        public async Task<PredictionResultDto?> GetStudentPredictionAsync(int studentId)
        {
            return await _httpClient.GetFromJsonAsync<PredictionResultDto>($"api/prediction/student/{studentId}");
        }
    }
}
