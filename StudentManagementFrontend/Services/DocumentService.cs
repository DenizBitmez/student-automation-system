using System.Net.Http.Json;
using StudentManagementFrontend.Models;

namespace StudentManagementFrontend.Services
{
    public class DocumentService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/document";

        public DocumentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<DocumentVm>> GetMyDocumentsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<DocumentVm>>($"{BaseUrl}/my") ?? new List<DocumentVm>();
        }

        public async Task<List<DocumentVm>> GetStudentDocumentsAsync(int studentId)
        {
            return await _httpClient.GetFromJsonAsync<List<DocumentVm>>($"{BaseUrl}/student/{studentId}") ?? new List<DocumentVm>();
        }
        public async Task<byte[]> GetStudentCertificateAsync()
        {
            var response = await _httpClient.GetAsync("api/certificate/student-certificate");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> GetTranscriptAsync()
        {
            var response = await _httpClient.GetAsync("api/certificate/transcript");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
