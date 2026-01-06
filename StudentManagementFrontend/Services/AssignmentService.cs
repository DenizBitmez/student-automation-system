using System.Net.Http.Json;
using StudentManagementFrontend.Models;

namespace StudentManagementFrontend.Services
{
    public class AssignmentService(HttpClient http)
    {
        // Assignments
        public async Task<List<Assignment>> GetAssignmentsByCourseAsync(int courseId)
        {
            return await http.GetFromJsonAsync<List<Assignment>>($"api/assignment/course/{courseId}") ?? new();
        }

        public async Task<Assignment?> GetAssignmentByIdAsync(int id)
        {
            return await http.GetFromJsonAsync<Assignment>($"api/assignment/{id}");
        }

        public async Task<bool> CreateAssignmentAsync(AssignmentCreateDto dto)
        {
            var res = await http.PostAsJsonAsync("api/assignment", dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAssignmentAsync(int id, AssignmentUpdateDto dto)
        {
            var res = await http.PutAsJsonAsync($"api/assignment/{id}", dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAssignmentAsync(int id)
        {
            var res = await http.DeleteAsync($"api/assignment/{id}");
            return res.IsSuccessStatusCode;
        }

        // Submissions
        public async Task<List<Submission>> GetSubmissionsByAssignmentAsync(int assignmentId)
        {
            return await http.GetFromJsonAsync<List<Submission>>($"api/submission/assignment/{assignmentId}") ?? new();
        }

        public async Task<List<Submission>> GetMySubmissionsAsync()
        {
            return await http.GetFromJsonAsync<List<Submission>>("api/submission/student/my-submissions") ?? new();
        }

        public async Task<bool> SubmitAssignmentAsync(SubmissionCreateDto dto)
        {
            var res = await http.PostAsJsonAsync("api/submission", dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> GradeSubmissionAsync(int submissionId, SubmissionGradeDto dto)
        {
            var res = await http.PostAsJsonAsync($"api/submission/{submissionId}/grade", dto);
            return res.IsSuccessStatusCode;
        }
    }
}
