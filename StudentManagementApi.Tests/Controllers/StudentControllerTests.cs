using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StudentManagementApi.Controllers;
using StudentManagementApi.Data;
using StudentManagementApi.Domain;
using Xunit;
using FluentAssertions;
using static StudentManagementApi.Dtos.StudentDtos;

namespace StudentManagementApi.Tests.Controllers;

public class StudentControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly StudentController _controller;
    private readonly ServiceProvider _serviceProvider;

    public StudentControllerTests()
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

        _controller = new StudentController(_context, _userManager);
        
        // Ensure student role exists
        _roleManager.CreateAsync(new IdentityRole("Student")).Wait();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _serviceProvider.Dispose();
    }

    [Fact]
    public async Task Get_ReturnsAllStudents()
    {
        // Arrange
        var user1 = new ApplicationUser { UserName = "u1", Email = "s1@t.com", FullName = "S1" };
        var user2 = new ApplicationUser { UserName = "u2", Email = "s2@t.com", FullName = "S2" };
        await _userManager.CreateAsync(user1);
        await _userManager.CreateAsync(user2);
        
        var student1 = new Student { UserId = user1.Id, EnrolledAt = DateTime.UtcNow };
        var student2 = new Student { UserId = user2.Id, EnrolledAt = DateTime.UtcNow };

        _context.Students.AddRange(student1, student2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Get();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var students = okResult!.Value as IEnumerable<StudentVm>;
        students.Should().HaveCount(2);
    }

    [Fact]
    public async Task Create_ValidStudent_ReturnsCreatedStudent()
    {
        // Arrange
        var createDto = new StudentCreateDto("new@t.com", "New Student", "Passw0rd1!");

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        var studentVm = createdResult!.Value as StudentVm;
        studentVm.Should().NotBeNull();
        studentVm!.Email.Should().Be(createDto.Email);
    }

    [Fact]
    public async Task GetById_ExistingStudent_ReturnsStudent()
    {
        // Arrange
        var user = new ApplicationUser { UserName = "u", Email = "s@t.com", FullName = "S" };
        await _userManager.CreateAsync(user);
        var student = new Student { UserId = user.Id, EnrolledAt = DateTime.UtcNow };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetById(student.Id);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var studentVm = okResult!.Value as StudentVm;
        studentVm.Should().NotBeNull();
        studentVm!.Id.Should().Be(student.Id);
    }

    [Fact]
    public async Task GetById_NonExistingStudent_ReturnsNotFound()
    {
        // Act
        var result = await _controller.GetById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_ExistingStudent_ReturnsNoContent()
    {
        // Arrange
        var user = new ApplicationUser { UserName = "u", Email = "s@t.com", FullName = "Old" };
        await _userManager.CreateAsync(user);
        var student = new Student { UserId = user.Id };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        var updateDto = new StudentUpdateDto("Updated");

        // Act
        var result = await _controller.Update(student.Id, updateDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        var updatedUser = await _userManager.FindByIdAsync(user.Id);
        updatedUser!.FullName.Should().Be("Updated");
    }

    [Fact]
    public async Task Delete_ExistingStudent_ReturnsNoContent()
    {
        // Arrange
        var user = new ApplicationUser { UserName = "u", Email = "s@t.com" };
        await _userManager.CreateAsync(user);
        var student = new Student { UserId = user.Id };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Delete(student.Id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        var dbStudent = await _context.Students.FindAsync(student.Id);
        dbStudent.Should().BeNull();
        var dbUser = await _userManager.FindByIdAsync(user.Id);
        dbUser.Should().BeNull();
    }
}