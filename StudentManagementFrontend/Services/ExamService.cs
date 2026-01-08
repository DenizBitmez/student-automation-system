using System.Net.Http.Json;
using StudentManagementFrontend.Models;

namespace StudentManagementFrontend.Services
{
    public class ExamService(HttpClient http)
    {
        public async Task<int> CreateAsync(ExamCreateDto dto)
        {
            var response = await http.PostAsJsonAsync("api/exam", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<int>();
            }
            return 0;
        }

        public async Task<List<ExamDto>> GetAvailableExamsAsync()
        {
            return await http.GetFromJsonAsync<List<ExamDto>>("api/exam/available") ?? new();
        }

        public async Task<List<ExamDto>> GetCreatedExamsAsync()
        {
            return await http.GetFromJsonAsync<List<ExamDto>>("api/exam/created") ?? new();
        }

        public async Task<ExamTakeDto?> GetExamForTakingAsync(int id)
        {
            try
            {
                return await http.GetFromJsonAsync<ExamTakeDto>($"api/exam/{id}/take");
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> SubmitExamAsync(ExamSubmitDto dto)
        {
            var response = await http.PostAsJsonAsync("api/exam/submit", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<int>();
            }
            return -1;
        }

        public async Task<List<ExamResultDto>> GetExamResultsAsync(int examId)
        {
            return await http.GetFromJsonAsync<List<ExamResultDto>>($"api/exam/{examId}/results") ?? new();
        }
    }
}
