using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace StudentManagementFrontend.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;
    private const string AuthTokenKey = "authToken";
    private AuthenticationState _anonymous;
    private bool _initialized;

    public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            if (_initialized)
            {
                 // If we successfully set the Authorization header, we assume the state is valid
                 // However, we rely on NotifyUserAuthentication to update the state in-memory if needed.
                 // But wait, the previous implementation reconstructed it every time.
                 // Let's try to fetch token from LocalStorage only if we haven't checked yet?
                 // NO. In Blazor Server, we should avoid calling JS Interop in this method if possible.
                 // BUT, we don't have a way to know if we are "initialized" with the correct token without checking storage.
                 // The safe pattern is: Return Anonymous initially. Then Check Storage.
                 // But Check Storage cannot happen inside GetAuthenticationStateAsync reliably on 1st render.
                 // So we return _anonymous unless we have a cached principal.
                 return _anonymous;
            }
            
            // On first call (or subsequent checks), we try to read local storage 
            // BUT only if we are connected. Since we can't easily know, we wrap in try-catch.
            // AND we update _anonymous (which acts as our cache) if found.
            
            var token = await _localStorage.GetItemAsync<string>(AuthTokenKey);

            if (string.IsNullOrWhiteSpace(token))
            {
                return _anonymous;
            }

            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            var user = new ClaimsPrincipal(identity);
            
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
            var authState = new AuthenticationState(user);
            _anonymous = authState; // Cache it
            _initialized = true;

            return authState;
        }
        catch
        {
            // If JS Interop fails (Pre-rendering), return anonymous
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public void NotifyUserAuthentication(string token)
    {
        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        var user = new ClaimsPrincipal(identity);
        var authState = new AuthenticationState(user);

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        
        _anonymous = authState; // Update cache
        _initialized = true;

        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }

    public void NotifyUserLogout()
    {
        var authState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        _anonymous = authState; // Update cache
        _initialized = false;
        
        _httpClient.DefaultRequestHeaders.Authorization = null;
        
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        
        if (keyValuePairs == null)
            return new List<Claim>();
        
        // Map claims to Claim types
        var claims = new List<Claim>();
        foreach (var kvp in keyValuePairs)
        {
             // Handle standard claim mapping if necessary, or just add as is
             claims.Add(new Claim(kvp.Key, kvp.Value?.ToString() ?? string.Empty));
        }

        // Improve name mapping if 'unique_name' or 'name' exists to Identity.Name
        if (keyValuePairs.TryGetValue("unique_name", out var name) || keyValuePairs.TryGetValue("name", out name))
        {
             claims.Add(new Claim(ClaimTypes.Name, name?.ToString() ?? string.Empty));
        }
        
        // Improve role mapping
         if (keyValuePairs.TryGetValue("role", out var roles) && roles != null)
        {
            var rolesStr = roles.ToString();
            if (!string.IsNullOrEmpty(rolesStr))
            {
                if (rolesStr.Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(rolesStr);
                    if (parsedRoles != null)
                    {
                        foreach (var parsedRole in parsedRoles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                        }
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, rolesStr));
                }
            }
        }

        return claims;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
