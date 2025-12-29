using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace StudentManagementFrontend.Services;

public class JwtInterceptor : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;

    public JwtInterceptor(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch (Exception)
        {
            // Ignore (e.g. prerendering or no token)
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
