using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using StudentManagementFrontend;
using StudentManagementFrontend.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Set the base address for the HTTP client
var baseAddress = builder.Configuration["BaseUrl"] ?? builder.HostEnvironment.BaseAddress;

// Register the HTTP client with the base address
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });

// Add Blazored LocalStorage for token management
builder.Services.AddBlazoredLocalStorage();

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<StudentService>();

// Configure authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>(
    provider => new CustomAuthStateProvider(
        provider.GetRequiredService<ILocalStorageService>(),
        provider.GetRequiredService<HttpClient>()
    )
);

// Set the root component and initialize the application
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add Bootstrap Icons
builder.Services.AddBootstrapComponents();

// Build and run the application
await builder.Build().RunAsync();
