using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace StudentManagementApi.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public async Task JoinUserGroup(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
    }

    public async Task JoinRoleGroup(string role)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Role_{role}");
    }

    public async Task JoinCourseGroup(string courseId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Course_{courseId}");
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRoles = Context.User?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList() ?? new List<string>();

        if (!string.IsNullOrEmpty(userId))
        {
            // Join user-specific group
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");

            // Join role-specific groups
            foreach (var role in userRoles)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Role_{role}");
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRoles = Context.User?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList() ?? new List<string>();

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");

            foreach (var role in userRoles)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Role_{role}");
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
}