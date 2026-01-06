using System.Net.Http.Json;
using StudentManagementFrontend.Models;

namespace StudentManagementFrontend.Services
{
    public class ActivityService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/activity";

        public ActivityService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ActivityVm>> GetMyActivitiesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ActivityVm>>($"{BaseUrl}/my") ?? new List<ActivityVm>();
        }

        public async Task<List<ActivityVm>> GetStudentActivitiesAsync(int studentId)
        {
            return await _httpClient.GetFromJsonAsync<List<ActivityVm>>($"{BaseUrl}/student/{studentId}") ?? new List<ActivityVm>();
        }

        public async Task CreateActivityAsync(ActivityVm activity)
        {
            var dto = new 
            { 
                Title = activity.Title, 
                Type = activity.Type, 
                Description = activity.Description, 
                ActivityDate = activity.ActivityDate 
            };
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, dto);
            response.EnsureSuccessStatusCode();
        }
    }
}
