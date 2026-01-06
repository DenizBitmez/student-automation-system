using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentManagementApi.Data;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Xunit;
using FluentAssertions;
using static StudentManagementApi.Dtos.AuthDtos;
using static StudentManagementApi.Dtos.StudentDtos;

namespace StudentManagementApi.Tests.Integration;

public class StudentIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public StudentIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                // Remove all existing DbContext and Options registrations
                var serviceDescriptors = services.Where(d => 
                    d.ServiceType == typeof(AppDbContext) || 
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions)).ToList();
                
                foreach (var descriptor in serviceDescriptors)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database for testing
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"InMemoryDb_{Guid.NewGuid()}");
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task AuthFlow_RegisterAndLogin_ShouldWork()
    {
        // Register a new admin user
        var registerDto = new RegisterDto("admin@test.com", "admin@test.com", "Password123!", "Test Admin", "Admin");

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        registerResponse.Should().BeSuccessful();

        // Login with the registered user
        var loginDto = new LoginDto(registerDto.Email, registerDto.Password);

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
        loginResponse.Should().BeSuccessful();

        var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        authResponse.Should().NotBeNull();
        authResponse.Token.Should().NotBeNullOrEmpty();
        authResponse.Email.Should().Be(registerDto.Email);
        authResponse.Roles.Should().Contain("Admin");
    }

    [Fact]
    public async Task StudentCrud_WithAuthenticatedUser_ShouldWork()
    {
        // First, register and login as admin
        var token = await GetAdminTokenAsync();

        // Set authorization header
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create a student
        var createStudentDto = new StudentCreateDto("student@test.com", "Test Student", "Password123!");

        var createResponse = await _client.PostAsJsonAsync("/api/student", createStudentDto);
        createResponse.Should().BeSuccessful();

        var createdStudent = await createResponse.Content.ReadFromJsonAsync<StudentVm>();
        createdStudent.Should().NotBeNull();
        createdStudent.Email.Should().Be(createStudentDto.Email);

        // Get all students
        var getResponse = await _client.GetAsync("/api/student");
        getResponse.Should().BeSuccessful();

        var students = await getResponse.Content.ReadFromJsonAsync<IEnumerable<StudentVm>>();
        students.Should().NotBeNull();
        students.Should().HaveCountGreaterThan(0);

        // Get student by ID
        var getByIdResponse = await _client.GetAsync($"/api/student/{createdStudent.Id}");
        getByIdResponse.Should().BeSuccessful();

        var student = await getByIdResponse.Content.ReadFromJsonAsync<StudentVm>();
        student.Should().NotBeNull();
        student.Email.Should().Be(createdStudent.Email);

        // Update student
        var updateDto = new StudentUpdateDto("Updated Student");

        var updateResponse = await _client.PutAsJsonAsync($"/api/student/{createdStudent.Id}", updateDto);
        updateResponse.Should().BeSuccessful();

        var updatedStudent = await updateResponse.Content.ReadFromJsonAsync<StudentVm>();
        updatedStudent.Should().NotBeNull();
        updatedStudent.FullName.Should().Be(updateDto.FullName);

        // Delete student
        var deleteResponse = await _client.DeleteAsync($"/api/student/{createdStudent.Id}");
        deleteResponse.Should().BeSuccessful();

        // Verify student is deleted
        var getDeletedResponse = await _client.GetAsync($"/api/student/{createdStudent.Id}");
        getDeletedResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task StudentEndpoints_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Try to access protected endpoint without token
        var response = await _client.GetAsync("/api/student");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task StudentCreate_WithInvalidRole_ShouldReturnForbidden()
    {
        // Register and login as student
        var studentToken = await GetStudentTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", studentToken);

        // Try to create a student (should be forbidden for student role)
        var createStudentDto = new StudentCreateDto("newstudent@test.com", "New Student", "Password123!");

        var response = await _client.PostAsJsonAsync("/api/student", createStudentDto);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    private async Task<string> GetAdminTokenAsync()
    {
        // Register admin
        var email = $"admin{Guid.NewGuid()}@test.com";
        var registerDto = new RegisterDto(email, email, "Password123!", "Test Admin", "Admin");

        await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Login admin
        var loginDto = new LoginDto(registerDto.Email, registerDto.Password);

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        
        return authResponse.Token;
    }

    private async Task<string> GetStudentTokenAsync()
    {
        // Register student
        var email = $"student{Guid.NewGuid()}@test.com";
        var registerDto = new RegisterDto(email, email, "Password123!", "Test Student", "Student");

        await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        // Login student
        var loginDto = new LoginDto(registerDto.Email, registerDto.Password);

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        
        return authResponse.Token;
    }
}