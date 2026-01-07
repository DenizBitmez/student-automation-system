using System.Net.Http.Json;
using StudentManagementFrontend.Models;
using static StudentManagementFrontend.Models.ParentDtos;

namespace StudentManagementFrontend.Services
{
    public class ParentService(HttpClient http)
    {
        public async Task<List<ChildSummaryDto>> GetChildrenAsync()
        {
            return await http.GetFromJsonAsync<List<ChildSummaryDto>>("api/parent/children") ?? new();
        }

        public async Task<ChildOverviewDto?> GetChildOverviewAsync(int studentId)
        {
            try 
            {
                return await http.GetFromJsonAsync<ChildOverviewDto>($"api/parent/child/{studentId}/overview");
            }
            catch
            {
                return null;
            }
        }
    }
}
