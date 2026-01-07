using StudentManagementFrontend.Models;

namespace StudentManagementFrontend.Services
{
    public interface IMessageService
    {
        Task<List<ConversationUserDto>> GetConversationsAsync();
        Task<List<MessageDto>> GetThreadAsync(string otherUserId);
        Task SendMessageAsync(SendMessageDto dto);
        Task<List<ConversationUserDto>> GetContactsAsync();
    }
}
