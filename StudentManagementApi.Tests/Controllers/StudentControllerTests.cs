using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
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
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly StudentController _controller;

    public StudentControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _userManagerMock = MockUserManager<ApplicationUser>();
        _controller = new StudentController(_context, _userManagerMock.Object);
    }

    [Fact]
    public async Task Get_ReturnsAllStudents()
    {
        // Arrange
        var user1 = new ApplicationUser { Id = "1", Email = "student1@test.com", FullName = "Student 1" };
        var user2 = new ApplicationUser { Id = "2", Email = "student2@test.com", FullName = "Student 2" };
        
        var student1 = new Student { Id = 1, UserId = "1", User = user1, EnrolledAt = DateTime.UtcNow };
        var student2 = new Student { Id = 2, UserId = "2", User = user2, EnrolledAt = DateTime.UtcNow };

        _context.Users.AddRange(user1, user2);
        _context.Students.AddRange(student1, student2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Get();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var students = okResult.Value as IEnumerable<StudentVm>;
        students.Should().HaveCount(2);
    }

    [Fact]
    public async Task Create_ValidStudent_ReturnsCreatedStudent()
    {
        // Arrange
        var createDto = new StudentCreateDto("newstudent@test.com", "New Student", "Password123!");

        var user = new ApplicationUser
        {
            Id = "new-user-id",
            Email = createDto.Email,
            FullName = createDto.FullName
        };

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success)
            .Callback<ApplicationUser, string>((u, p) => 
            {
                u.Id = user.Id;
            });

        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Student"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        var studentVm = createdResult.Value as StudentVm;
        studentVm.Should().NotBeNull();
        studentVm.Email.Should().Be(createDto.Email);
        studentVm.FullName.Should().Be(createDto.FullName);
    }

    [Fact]
    public async Task Create_UserCreationFails_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new StudentCreateDto("invalid@test.com", "Test Student", "weak");

        var identityError = new IdentityError { Description = "Password too weak" };
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetById_ExistingStudent_ReturnsStudent()
    {
        // Arrange
        var user = new ApplicationUser { Id = "1", Email = "student@test.com", FullName = "Test Student" };
        var student = new Student { Id = 1, UserId = "1", User = user, EnrolledAt = DateTime.UtcNow };

        _context.Users.Add(user);
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetById(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var studentVm = okResult.Value as StudentVm;
        studentVm.Should().NotBeNull();
        studentVm.Id.Should().Be(1);
        studentVm.Email.Should().Be("student@test.com");
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
    public async Task Update_ExistingStudent_ReturnsUpdatedStudent()
    {
        // Arrange
        var user = new ApplicationUser { Id = "1", Email = "student@test.com", FullName = "Old Name" };
        var student = new Student { Id = 1, UserId = "1", User = user, EnrolledAt = DateTime.UtcNow };

        _context.Users.Add(user);
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        var updateDto = new StudentUpdateDto("Updated Name");

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify update in database
        var updatedStudent = await _context.Students.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == 1);
        updatedStudent.Should().NotBeNull();
        updatedStudent.User.FullName.Should().Be("Updated Name");
        updatedStudent.User.Email.Should().Be("student@test.com");

    }

    [Fact]
    public async Task Delete_ExistingStudent_ReturnsNoContent()
    {
        // Arrange
        var user = new ApplicationUser { Id = "1", Email = "student@test.com", FullName = "Test Student" };
        var student = new Student { Id = 1, UserId = "1", User = user, EnrolledAt = DateTime.UtcNow };

        _context.Users.Add(user);
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        _userManagerMock.Setup(x => x.FindByIdAsync("1"))
            .ReturnsAsync(user);

        _userManagerMock.Setup(x => x.DeleteAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();

        // Verify student is deleted
        var deletedStudent = await _context.Students.FindAsync(1);
        deletedStudent.Should().BeNull();
    }

    private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        mgr.Object.UserValidators.Add(new UserValidator<TUser>());
        mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());
        return mgr;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}