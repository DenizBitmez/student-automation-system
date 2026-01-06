using System.Net.Http.Json;
using StudentManagementFrontend.Models;

namespace StudentManagementFrontend.Services
{
    public class ScheduleService(HttpClient http)
    {
        public async Task<List<ScheduleItem>> GetMyScheduleAsync()
        {
            try
            {
                return await http.GetFromJsonAsync<List<ScheduleItem>>("api/schedule/my-schedule") ?? new();
            }
            catch (Exception)
            {
                return new();
            }
        }

        public async Task<bool> CreateScheduleItemAsync(ScheduleItemCreateDto dto)
        {
            var res = await http.PostAsJsonAsync("api/schedule", dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteScheduleItemAsync(int id)
        {
            var res = await http.DeleteAsync($"api/schedule/{id}");
            return res.IsSuccessStatusCode;
        }
    }
}
