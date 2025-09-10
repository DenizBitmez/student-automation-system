using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StudentManagementApi.Controllers;
using StudentManagementApi.Domain;
using StudentManagementApi.Services;
using Xunit;
using FluentAssertions;
using static StudentManagementApi.Dtos.AuthDtos;

namespace StudentManagementApi.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private readonly Mock<JwtTokenService> _jwtServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _userManagerMock = MockUserManager<ApplicationUser>();
        _roleManagerMock = MockRoleManager();
        _jwtServiceMock = new Mock<JwtTokenService>();
        _controller = new AuthController(_userManagerMock.Object, _roleManagerMock.Object, _jwtServiceMock.Object);
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsOk()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "test@test.com",
            Password = "Password123!",
            FullName = "Test User",
            Role = "Student"
        };

        _roleManagerMock.Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Register_InvalidRole_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "test@test.com",
            Password = "Password123!",
            FullName = "Test User",
            Role = "InvalidRole"
        };

        _roleManagerMock.Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@test.com",
            Password = "Password123!"
        };

        var user = new ApplicationUser
        {
            Email = "test@test.com",
            FullName = "Test User"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string> { "Student" });

        _jwtServiceMock.Setup(x => x.CreateToken(It.IsAny<ApplicationUser>()))
            .ReturnsAsync("fake-jwt-token");

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.Value.Should().BeOfType<AuthResponse>();
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@test.com",
            Password = "WrongPassword"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ApplicationUser)null);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        mgr.Object.UserValidators.Add(new UserValidator<TUser>());
        mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());
        return mgr;
    }

    private static Mock<RoleManager<IdentityRole>> MockRoleManager()
    {
        var store = new Mock<IRoleStore<IdentityRole>>();
        return new Mock<RoleManager<IdentityRole>>(store.Object, null, null, null, null);
    }
}