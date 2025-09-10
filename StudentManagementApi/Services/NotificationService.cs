using Microsoft.AspNetCore.SignalR;
using StudentManagementApi.Hubs;

namespace StudentManagementApi.Services;

public interface INotificationService
{
    Task SendNotificationToUserAsync(string userId, NotificationMessage message);
    Task SendNotificationToRoleAsync(string role, NotificationMessage message);
    Task SendNotificationToCourseAsync(string courseId, NotificationMessage message);
    Task SendGradeNotificationAsync(string studentId, string courseName, double grade, string? comment = null);
    Task SendAttendanceNotificationAsync(string studentId, string courseName, bool present);
    Task SendCourseUpdateNotificationAsync(string courseId, string message);
    Task SendSystemNotificationAsync(string message, string type = "info");
}

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IHubContext<NotificationHub> hubContext, ILogger<NotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendNotificationToUserAsync(string userId, NotificationMessage message)
    {
        try
        {
            await _hubContext.Clients.Group($"User_{userId}")
                .SendAsync("ReceiveNotification", message);
                
            _logger.LogInformation("Notification sent to user {UserId}: {Title}", userId, message.Title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to user {UserId}", userId);
        }
    }

    public async Task SendNotificationToRoleAsync(string role, NotificationMessage message)
    {
        try
        {
            await _hubContext.Clients.Group($"Role_{role}")
                .SendAsync("ReceiveNotification", message);
                
            _logger.LogInformation("Notification sent to role {Role}: {Title}", role, message.Title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to role {Role}", role);
        }
    }

    public async Task SendNotificationToCourseAsync(string courseId, NotificationMessage message)
    {
        try
        {
            await _hubContext.Clients.Group($"Course_{courseId}")
                .SendAsync("ReceiveNotification", message);
                
            _logger.LogInformation("Notification sent to course {CourseId}: {Title}", courseId, message.Title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to course {CourseId}", courseId);
        }
    }

    public async Task SendGradeNotificationAsync(string studentId, string courseName, double grade, string? comment = null)
    {
        var message = new NotificationMessage
        {
            Title = "Yeni Not Girişi",
            Message = $"{courseName} dersi için notunuz girildi: {grade:F1}" + 
                     (string.IsNullOrEmpty(comment) ? "" : $"\nYorum: {comment}"),
            Type = grade >= 50 ? "success" : "warning",
            Icon = "bi-journal-check",
            Timestamp = DateTime.UtcNow,
            Data = new
            {
                Type = "grade",
                CourseId = courseName,
                Grade = grade,
                Comment = comment
            }
        };

        await SendNotificationToUserAsync(studentId, message);
    }

    public async Task SendAttendanceNotificationAsync(string studentId, string courseName, bool present)
    {
        var message = new NotificationMessage
        {
            Title = "Devamsızlık Kaydı",
            Message = $"{courseName} dersine {(present ? "katılım" : "devamsızlık")} kaydedildi",
            Type = present ? "info" : "warning",
            Icon = present ? "bi-check-circle" : "bi-exclamation-triangle",
            Timestamp = DateTime.UtcNow,
            Data = new
            {
                Type = "attendance",
                CourseId = courseName,
                Present = present
            }
        };

        await SendNotificationToUserAsync(studentId, message);
    }

    public async Task SendCourseUpdateNotificationAsync(string courseId, string message)
    {
        var notification = new NotificationMessage
        {
            Title = "Ders Güncellmesi",
            Message = message,
            Type = "info",
            Icon = "bi-book",
            Timestamp = DateTime.UtcNow,
            Data = new
            {
                Type = "course_update",
                CourseId = courseId
            }
        };

        await SendNotificationToCourseAsync(courseId, notification);
    }

    public async Task SendSystemNotificationAsync(string message, string type = "info")
    {
        var notification = new NotificationMessage
        {
            Title = "Sistem Bildirimi",
            Message = message,
            Type = type,
            Icon = "bi-info-circle",
            Timestamp = DateTime.UtcNow,
            Data = new
            {
                Type = "system"
            }
        };

        // Send to all connected users
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
    }
}

public class NotificationMessage
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "info"; // success, info, warning, error
    public string Icon { get; set; } = "bi-bell";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public object? Data { get; set; }
    public bool IsRead { get; set; } = false;
    public string Id { get; set; } = Guid.NewGuid().ToString();
}