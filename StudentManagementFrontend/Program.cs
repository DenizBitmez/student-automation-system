using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using StudentManagementFrontend.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using StudentManagementFrontend.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Set the base address for the HTTP client
var baseAddress = builder.Configuration["BaseUrl"] ?? "http://localhost:5000/";

// Register JwtInterceptor
builder.Services.AddScoped<JwtInterceptor>();

// Register the HTTP client with the base address and message handler
builder.Services.AddHttpClient("API", client => client.BaseAddress = new Uri(baseAddress))
    .AddHttpMessageHandler<JwtInterceptor>();

// Register the default HttpClient (scoped) to use the factory
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

// Add Blazored LocalStorage for token management
builder.Services.AddBlazoredLocalStorage();

// Register services
// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<ComplaintService>();
builder.Services.AddScoped<ActivityService>();
builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<ITeacherLeaveService, TeacherLeaveService>();
builder.Services.AddScoped<AnnouncementService>();
builder.Services.AddScoped<AssignmentService>();
builder.Services.AddScoped<ScheduleService>();

// Configure authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>(
    provider => new CustomAuthStateProvider(
        provider.GetRequiredService<ILocalStorageService>(),
        provider.GetRequiredService<HttpClient>()
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();