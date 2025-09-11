using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using StudentManagementFrontend.Models.Auth;

namespace StudentManagementFrontend.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly ILocalStorageService _localStorage;
    private const string AuthTokenKey = "authToken";
    private const string UserIdKey = "userId";
    private const string UserRolesKey = "userRoles";

    public AuthService(
        HttpClient httpClient,
        AuthenticationStateProvider authStateProvider,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authStateProvider = authStateProvider;
        _localStorage = localStorage;
    }

    public async Task<bool> LoginAsync(LoginModel loginModel)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginModel);
            
            if (!response.IsSuccessStatusCode)
                return false;

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            
            if (result == null || string.IsNullOrEmpty(result.Token))
                return false;

            await _localStorage.SetItemAsync(AuthTokenKey, result.Token);
            await _localStorage.SetItemAsync(UserIdKey, result.UserId);
            await _localStorage.SetItemAsync(UserRolesKey, result.Roles);
            
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Token);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RegisterAsync(RegisterModel registerModel)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerModel);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Registration error: {ex.Message}");
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync(AuthTokenKey);
        await _localStorage.RemoveItemAsync(UserIdKey);
        await _localStorage.RemoveItemAsync(UserRolesKey);
        ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
    }

    public async Task<string> GetCurrentUserIdAsync()
    {
        return await _localStorage.GetItemAsync<string>(UserIdKey) ?? string.Empty;
    }

    public async Task<string[]> GetCurrentUserRolesAsync()
    {
        return await _localStorage.GetItemAsync<string[]>(UserRolesKey) ?? Array.Empty<string>();
    }

    public async Task<bool> IsUserInRoleAsync(string role)
    {
        var roles = await GetCurrentUserRolesAsync();
        return roles.Contains(role);
    }

    public async Task<bool> IsUserInAnyRoleAsync(IEnumerable<string> roles)
    {
        var userRoles = await GetCurrentUserRolesAsync();
        return roles.Any(role => userRoles.Contains(role));
    }
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string[] Roles { get; set; } = Array.Empty<string>();
}
