using System.Net.Http.Json;
using StudentManagementFrontend.Models;

namespace StudentManagementFrontend.Services
{
    public class ComplaintService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/complaint";

        public ComplaintService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ComplaintVm>> GetMyComplaintsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ComplaintVm>>($"{BaseUrl}/my") ?? new List<ComplaintVm>();
        }

        public async Task<List<ComplaintVm>> GetAllComplaintsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ComplaintVm>>($"{BaseUrl}/all") ?? new List<ComplaintVm>();
        }

        public async Task CreateComplaintAsync(ComplaintCreateVm complaint)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, complaint);
            response.EnsureSuccessStatusCode();
        }

        public async Task RespondToComplaintAsync(int id, string response)
        {
            var dto = new { Id = id, AdminResponse = response };
            var result = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}/respond", dto);
            result.EnsureSuccessStatusCode();
        }
    }
}
