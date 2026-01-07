using System.Net.Http.Json;
using StudentManagementFrontend.Models;

namespace StudentManagementFrontend.Services
{
    public class MessageService : IMessageService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/message";

        public MessageService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ConversationUserDto>> GetConversationsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ConversationUserDto>>($"{BaseUrl}/conversations") ?? new List<ConversationUserDto>();
        }

        public async Task<List<MessageDto>> GetThreadAsync(string otherUserId)
        {
            return await _httpClient.GetFromJsonAsync<List<MessageDto>>($"{BaseUrl}/thread/{otherUserId}") ?? new List<MessageDto>();
        }

        public async Task SendMessageAsync(SendMessageDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/send", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<ConversationUserDto>> GetContactsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<ConversationUserDto>>($"{BaseUrl}/contacts") ?? new List<ConversationUserDto>();
        }
    }
}
