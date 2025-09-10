using StudentManagementFrontend.Models.Auth;

namespace StudentManagementFrontend.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(LoginModel loginModel);
    Task<bool> RegisterAsync(RegisterModel registerModel);
    Task LogoutAsync();
    Task<string> GetCurrentUserIdAsync();
    Task<string[]> GetCurrentUserRolesAsync();
    Task<bool> IsUserInRoleAsync(string role);
    Task<bool> IsUserInAnyRoleAsync(IEnumerable<string> roles);
}
