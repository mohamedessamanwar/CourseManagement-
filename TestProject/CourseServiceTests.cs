using BusinessAccessLayer.DTOS.CourceDtos;
using BusinessAccessLayer.Exceptions;
using BusinessAccessLayer.Services.CourseService;
using DataAccessLayer.Data.Models;
using DataAccessLayer.Repositories.CourseRepo;
using Moq;

public class CourseServiceTests
{
    private readonly Mock<ICourseRepo> _mockRepo;
    private readonly CourseService _courseService;

    public CourseServiceTests()
    {
        _mockRepo = new Mock<ICourseRepo>();
        _courseService = new CourseService(_mockRepo.Object);
    }

    [Fact]
    public async Task CreateCourse_ShouldReturnTrue_WhenCourseIsAdded()
    {
        // Arrange
        var newCourse = new CreateCourseDto { Title = "Test Course", Description = "Test Description", Price = 100 };
        //   _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Course>())).Returns(Task.CompletedTask);
        _mockRepo.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _courseService.CreateCourse(newCourse);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateCourse_ShouldReturnTrue_WhenCourseIsUpdated()
    {
        // Arrange
        var existingCourse = new Course { Id = 1, Title = "Old Title", Description = "Old Desc", Price = 50 };
        var updatedCourseDto = new CreateCourseDto { Title = "New Title", Description = "New Desc", Price = 100 };
        _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingCourse);
        _mockRepo.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _courseService.UpdateCourse(updatedCourseDto, 1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateCourse_ShouldThrowNotFoundException_WhenCourseDoesNotExist()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Course)null);
        var updatedCourseDto = new CreateCourseDto { Title = "New Title", Description = "New Desc", Price = 100 };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _courseService.UpdateCourse(updatedCourseDto, 1));
    }

    [Fact]
    public async Task DeleteCourse_ShouldReturnTrue_WhenCourseIsDeleted()
    {
        // Arrange
        var existingCourse = new Course { Id = 1, Title = "Test Course", Description = "Test Desc", Price = 50 };
        _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingCourse);
        _mockRepo.Setup(repo => repo.Complete()).ReturnsAsync(1);

        // Act
        var result = await _courseService.DeleteCourse(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteCourse_ShouldThrowNotFoundException_WhenCourseDoesNotExist()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Course)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _courseService.DeleteCourse(1));
    }

    [Fact]
    public async Task GetCourseById_ShouldReturnCourse_WhenCourseExists()
    {
        // Arrange
        var existingCourse = new Course { Id = 1, Title = "Test Course", Description = "Test Desc", Price = 100 };
        _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingCourse);

        // Act
        var result = await _courseService.GetCourseById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Course", result.Title);
    }

    [Fact]
    public async Task GetCourseById_ShouldThrowNotFoundException_WhenCourseDoesNotExist()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Course)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _courseService.GetCourseById(1));
    }

    [Fact]
    public async Task GetAllCourses_ShouldReturnListOfCourses()
    {
        // Arrange
        var courses = new List<Course>
        {
            new Course { Id = 1, Title = "Course 1", Description = "Desc 1", Price = 100 },
            new Course { Id = 2, Title = "Course 2", Description = "Desc 2", Price = 200 }
        };
        _mockRepo.Setup(repo => repo.GetAll()).ReturnsAsync(courses);

        // Act
        var result = await _courseService.GetAllCourses();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
}
