using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StudentManagementApi.Controllers;
using StudentManagementApi.Data;
using StudentManagementApi.Domain;
using StudentManagementApi.Services;
using Xunit;
using FluentAssertions;
using static StudentManagementApi.Dtos.AuthDtos;

namespace StudentManagementApi.Tests.Controllers;

public class AuthControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly Mock<IJwtTokenService> _jwtServiceMock;
    private readonly AuthController _controller;
    private readonly ServiceProvider _serviceProvider;

    public AuthControllerTests()
    {
        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddLogging();

        _serviceProvider = services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<AppDbContext>();
        _userManager = _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        _jwtServiceMock = new Mock<IJwtTokenService>();
        _controller = new AuthController(_userManager, _roleManager, _jwtServiceMock.Object);
        
        // Ensure roles exist for tests
        _roleManager.CreateAsync(new IdentityRole("Student")).Wait();
        _roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _serviceProvider.Dispose();
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsOk()
    {
        // Arrange
        var registerDto = new RegisterDto("testuser", "test@test.com", "Passw0rd1!", "Test User", "Student");

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        result.Should().BeOfType<OkResult>();
        
        var user = await _userManager.FindByNameAsync("testuser");
        user.Should().NotBeNull();
        var roles = await _userManager.GetRolesAsync(user!);
        roles.Should().Contain("Student");
    }

    [Fact]
    public async Task Register_InvalidRole_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto("testuser", "test@test.com", "Passw0rd1!", "Test User", "InvalidRole");

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest!.Value.Should().Be("Invalid role");
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var password = "Passw0rd1!";
        var user = new ApplicationUser
        {
            UserName = "testuser",
            Email = "test@test.com",
            FullName = "Test User"
        };
        await _userManager.CreateAsync(user, password);
        await _userManager.AddToRoleAsync(user, "Student");

        var loginDto = new LoginDto("testuser", password);

        _jwtServiceMock.Setup(x => x.CreateToken(It.IsAny<ApplicationUser>()))
            .ReturnsAsync("fake-jwt-token");

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as AuthResponse;
        response.Should().NotBeNull();
        response!.Token.Should().Be("fake-jwt-token");
        response.Roles.Should().Contain("Student");
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto("nonexistent", "wrongpassword");

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }
}