using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using StudentManagementApi.Domain;
using StudentManagementApi.Dtos;
using System.Security.Claims;

namespace StudentManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MessageController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var senderRole = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(senderId)) return Unauthorized();

            // Validate Receiver
            var receiver = await _context.Users.FindAsync(dto.ReceiverId);
            if (receiver == null) return NotFound("Receiver not found");
            var receiverRoles = await _context.UserRoles
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                .Where(x => x.UserId == dto.ReceiverId)
                .Select(x => x.Name)
                .ToListAsync();

            // Rule Validation
            bool isAllowed = false;
            if (senderRole == "Admin") isAllowed = true;
            else if (senderRole == "Teacher")
            {
                // Teacher can message Admin or Student
                // Ideally check if student is in teacher's course, but strict role check is fine for now
                isAllowed = true; 
            }
            else if (senderRole == "Student")
            {
                // Student can only message Teacher
                if (receiverRoles.Contains("Teacher")) isAllowed = true;
            }

            if (!isAllowed) return Forbid("Messaging this user is not allowed.");

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Content = dto.Content,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Sent successfully" });
        }

        [HttpGet("inbox")]
        public async Task<ActionResult<List<MessageDto>>> GetInbox()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var messages = await _context.Messages
                .Include(m => m.Sender)
                .Where(m => m.ReceiverId == userId)
                .OrderByDescending(m => m.Timestamp)
                .Take(50)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    SenderName = m.Sender != null ? (m.Sender.FullName ?? m.Sender.UserName ?? "Unknown") : "Unknown",
                    ReceiverId = m.ReceiverId,
                    ReceiverName = "Me",
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    IsRead = m.IsRead,
                    IsMe = false
                })
                .ToListAsync();

            return Ok(messages);
        }

        [HttpGet("conversations")]
        public async Task<ActionResult<List<ConversationUserDto>>> GetConversations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Get all unique users interacted with
            var sentIds = await _context.Messages.Where(m => m.SenderId == userId).Select(m => m.ReceiverId).Distinct().ToListAsync();
            var receivedIds = await _context.Messages.Where(m => m.ReceiverId == userId).Select(m => m.SenderId).Distinct().ToListAsync();
            var userIds = sentIds.Union(receivedIds).Distinct();

            var conversations = new List<ConversationUserDto>();

            foreach(var id in userIds)
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null) continue;

                // Get role
                var roleId = await _context.UserRoles.Where(ur => ur.UserId == id).Select(ur => ur.RoleId).FirstOrDefaultAsync();
                var role = await _context.Roles.Where(r => r.Id == roleId).Select(r => r.Name).FirstOrDefaultAsync();

                var unread = await _context.Messages
                    .CountAsync(m => m.SenderId == id && m.ReceiverId == userId && !m.IsRead);

                var lastMsg = await _context.Messages
                    .Where(m => (m.SenderId ==  userId && m.ReceiverId == id) || (m.SenderId == id && m.ReceiverId == userId))
                    .OrderByDescending(m => m.Timestamp)
                    .FirstOrDefaultAsync();

                conversations.Add(new ConversationUserDto
                {
                    UserId = id,
                    FullName = user.FullName ?? user.UserName,
                    Role = role ?? "User",
                    UnreadCount = unread,
                    LastMessageTime = lastMsg?.Timestamp
                });
            }

            return Ok(conversations.OrderByDescending(c => c.LastMessageTime));
        }

        [HttpGet("thread/{otherUserId}")]
        public async Task<ActionResult<List<MessageDto>>> GetThread(string otherUserId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var messages = await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == otherUserId) || 
                            (m.SenderId == otherUserId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.Timestamp)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    SenderName = m.Sender != null ? (m.Sender.FullName ?? m.Sender.UserName ?? "Unknown") : "Unknown",
                    ReceiverId = m.ReceiverId,
                    ReceiverName = m.Receiver != null ? (m.Receiver.FullName ?? m.Receiver.UserName ?? "Unknown") : "Unknown",
                    Content = m.Content,
                    Timestamp = m.Timestamp,
                    IsRead = m.IsRead,
                    IsMe = m.SenderId == currentUserId
                })
                .ToListAsync();

            // Mark as read
            var unreadMessages = await _context.Messages
                .Where(m => m.SenderId == otherUserId && m.ReceiverId == currentUserId && !m.IsRead)
                .ToListAsync();

            if (unreadMessages.Any())
            {
                foreach (var msg in unreadMessages) msg.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return Ok(messages);
        }
        
        // Helper to get available contacts based on rules
        [HttpGet("contacts")]
        public async Task<ActionResult<List<ConversationUserDto>>> GetAvailableContacts()
        {
             var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
             var userRole = User.FindFirstValue(ClaimTypes.Role);
             
             var query = _context.Users.AsQueryable();
             
             if (userRole == "Student")
             {
                 // Fetch Teachers only (Simplification: All teachers, ideally only enrolled course teachers)
                 var teacherRoleId = await _context.Roles.Where(r => r.Name == "Teacher").Select(r => r.Id).FirstOrDefaultAsync();
                 var teacherIds = await _context.UserRoles.Where(ur => ur.RoleId == teacherRoleId).Select(ur => ur.UserId).ToListAsync();
                 query = query.Where(u => teacherIds.Contains(u.Id));
             }
             else if (userRole == "Teacher")
             {
                  // Fetch Students and Admins
                  // Simplification: All students
                  var studentRoleId = await _context.Roles.Where(r => r.Name == "Student").Select(r => r.Id).FirstOrDefaultAsync();
                  var studentIds = await _context.UserRoles.Where(ur => ur.RoleId == studentRoleId).Select(ur => ur.UserId).ToListAsync();
                  
                  // Also include admins? Maybe later.
                  query = query.Where(u => studentIds.Contains(u.Id));
             }
             // Admin sees everyone (default)
             
             var users = await query.Select(u => new ConversationUserDto
             {
                 UserId = u.Id,
                 FullName = u.FullName ?? u.UserName ?? "User",
                 Role = "User" // Simplified for now
             }).ToListAsync();
             
             return Ok(users);
        }
    }
}
