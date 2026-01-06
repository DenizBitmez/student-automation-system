using System.Net.Http.Json;
using StudentManagementFrontend.Models;

namespace StudentManagementFrontend.Services
{
    public class AnnouncementService(HttpClient http)
    {
        public async Task<List<Announcement>> GetAnnouncementsAsync()
        {
            return await http.GetFromJsonAsync<List<Announcement>>("api/announcement") ?? new();
        }

        public async Task<Announcement?> GetAnnouncementByIdAsync(int id)
        {
            return await http.GetFromJsonAsync<Announcement>($"api/announcement/{id}");
        }

        public async Task<bool> CreateAnnouncementAsync(AnnouncementCreateDto dto)
        {
            var res = await http.PostAsJsonAsync("api/announcement", dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAnnouncementAsync(int id, AnnouncementUpdateDto dto)
        {
            var res = await http.PutAsJsonAsync($"api/announcement/{id}", dto);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAnnouncementAsync(int id)
        {
            var res = await http.DeleteAsync($"api/announcement/{id}");
            return res.IsSuccessStatusCode;
        }
    }
}
