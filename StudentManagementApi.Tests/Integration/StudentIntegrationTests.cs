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
using Microsoft.AspNetCore.TestHost;
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
            builder.UseSetting("DbProvider", "InMemory");
            builder.UseEnvironment("Testing");
        });

        _client = _factory.CreateClient();

        // Seed roles and initial data
        using (var scope = _factory.Services.CreateScope())
        {
            var sp = scope.ServiceProvider;
            Seed.InitializeAsync(sp).GetAwaiter().GetResult();
        }
    }

    [Fact]
    public async Task AuthFlow_RegisterAndLogin_ShouldWork()
    {
        // Arrange
        var email = $"user{Guid.NewGuid()}@test.com";
        var registerDto = new RegisterDto(email, email, "Passw0rd1!", "Test User", "Student");

        // Act
        var regResponse = await _client.PostAsJsonAsync("api/auth/register", registerDto);
        if (!regResponse.IsSuccessStatusCode)
        {
            var err = await regResponse.Content.ReadAsStringAsync();
            throw new Exception($"Reg failed: {regResponse.StatusCode} - {err}");
        }
        regResponse.Should().BeSuccessful();

        var loginDto = new LoginDto(email, "Passw0rd1!");
        var loginResponse = await _client.PostAsJsonAsync("api/auth/login", loginDto);
        if (!loginResponse.IsSuccessStatusCode)
        {
            var err = await loginResponse.Content.ReadAsStringAsync();
            throw new Exception($"Login failed: {loginResponse.StatusCode} - {err}");
        }

        // Assert
        loginResponse.Should().BeSuccessful();
        var result = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.Roles.Should().Contain("Student");
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

        var createResponse = await _client.PostAsJsonAsync("api/student", createStudentDto);
        createResponse.Should().BeSuccessful();

        var createdStudent = await createResponse.Content.ReadFromJsonAsync<StudentVm>();
        createdStudent.Should().NotBeNull();
        createdStudent.Email.Should().Be(createStudentDto.Email);

        // Get all students
        var getResponse = await _client.GetAsync("api/student");
        getResponse.Should().BeSuccessful();

        var students = await getResponse.Content.ReadFromJsonAsync<IEnumerable<StudentVm>>();
        students.Should().NotBeNull();
        students.Should().HaveCountGreaterThan(0);

        // Get student by ID
        var getByIdResponse = await _client.GetAsync($"api/student/{createdStudent.Id}");
        getByIdResponse.Should().BeSuccessful();

        var student = await getByIdResponse.Content.ReadFromJsonAsync<StudentVm>();
        student.Should().NotBeNull();
        student.Email.Should().Be(createdStudent.Email);

        // Update student
        var updateDto = new StudentUpdateDto("Updated Student");

        var updateResponse = await _client.PutAsJsonAsync($"api/student/{createdStudent.Id}", updateDto);
        updateResponse.Should().BeSuccessful();

        // Verify update in database
        var getUpdatedResponse = await _client.GetAsync($"api/student/{createdStudent.Id}");
        getUpdatedResponse.Should().BeSuccessful();
        var updatedStudent = await getUpdatedResponse.Content.ReadFromJsonAsync<StudentVm>();
        updatedStudent.FullName.Should().Be("Updated Student");

        // Delete student
        var deleteResponse = await _client.DeleteAsync($"api/student/{createdStudent.Id}");
        deleteResponse.Should().BeSuccessful();

        // Verify student is deleted
        var getDeletedResponse = await _client.GetAsync($"api/student/{createdStudent.Id}");
        getDeletedResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task StudentEndpoints_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Try to access protected endpoint without token
        var response = await _client.GetAsync("api/student");
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

        var response = await _client.PostAsJsonAsync("api/student", createStudentDto);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    private async Task<string> GetAdminTokenAsync()
    {
        // Register admin
        var email = $"admin{Guid.NewGuid()}@test.com";
        var registerDto = new RegisterDto(email, email, "Password123!", "Test Admin", "Admin");

        await _client.PostAsJsonAsync("api/auth/register", registerDto);

        // Login admin
        var loginDto = new LoginDto(registerDto.Email, registerDto.Password);

        var response = await _client.PostAsJsonAsync("api/auth/login", loginDto);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        
        return authResponse.Token;
    }

    private async Task<string> GetStudentTokenAsync()
    {
        // Register student
        var email = $"student{Guid.NewGuid()}@test.com";
        var registerDto = new RegisterDto(email, email, "Password123!", "Test Student", "Student");

        await _client.PostAsJsonAsync("api/auth/register", registerDto);

        // Login student
        var loginDto = new LoginDto(registerDto.Email, registerDto.Password);

        var response = await _client.PostAsJsonAsync("api/auth/login", loginDto);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        
        return authResponse.Token;
    }
}