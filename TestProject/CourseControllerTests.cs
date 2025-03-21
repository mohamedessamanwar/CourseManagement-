using BusinessAccessLayer.DTOS;
using BusinessAccessLayer.DTOS.CourceDtos;
using BusinessAccessLayer.Services.CacheService;
using BusinessAccessLayer.Services.CourseService;
using CourseManagementAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

public class CourseControllerTests
{
    private readonly Mock<ICourseService> _mockCourseService;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<ILogger<CourseController>> _mockLogger;
    private readonly CourseController _controller;

    public CourseControllerTests()
    {
        _mockCourseService = new Mock<ICourseService>();
        _mockCacheService = new Mock<ICacheService>();
        _mockLogger = new Mock<ILogger<CourseController>>();
        _controller = new CourseController(_mockCourseService.Object, _mockLogger.Object, _mockCacheService.Object);
    }

    [Fact]
    public async Task CreateCourse_ReturnsOk_WhenCreationIsSuccessful()
    {
        // Arrange
        var courseDto = new CreateCourseDto();
        _mockCourseService.Setup(s => s.CreateCourse(courseDto)).ReturnsAsync(true);

        // Act
        var result = await _controller.CreateCourse(courseDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<Response<string>>(okResult.Value);

        Assert.Equal("Course created successfully", response.Message);
    }

    [Fact]
    public async Task CreateCourse_ReturnsBadRequest_WhenCreationFails()
    {
        // Arrange
        var courseDto = new CreateCourseDto();
        _mockCourseService.Setup(s => s.CreateCourse(courseDto)).ReturnsAsync(false);

        // Act
        var result = await _controller.CreateCourse(courseDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<Response<string>>(badRequestResult.Value);

        Assert.Equal("Failed to create course", response.Message);
    }

    [Fact]
    public async Task UpdateCourse_ReturnsOk_WhenUpdateIsSuccessful()
    {
        // Arrange
        var courseDto = new CreateCourseDto();
        _mockCourseService.Setup(s => s.UpdateCourse(courseDto, 1)).ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateCourse(1, courseDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<Response<string>>(okResult.Value);

        Assert.Equal("Course updated successfully", response.Message);
    }

    [Fact]
    public async Task UpdateCourse_ReturnsNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        var courseDto = new CreateCourseDto();
        _mockCourseService.Setup(s => s.UpdateCourse(courseDto, 1)).ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateCourse(1, courseDto);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<Response<string>>(notFoundResult.Value);

        Assert.Equal("Course not found", response.Message);
    }

    [Fact]
    public async Task DeleteCourse_ReturnsOk_WhenDeletionIsSuccessful()
    {
        // Arrange
        _mockCourseService.Setup(s => s.DeleteCourse(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteCourse(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<Response<string>>(okResult.Value);

        Assert.Equal("Course deleted successfully", response.Message);
    }

    [Fact]
    public async Task DeleteCourse_ReturnsNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        _mockCourseService.Setup(s => s.DeleteCourse(1)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteCourse(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<Response<string>>(notFoundResult.Value);
        // Assert.False(response.Success);
        Assert.Equal("Course not found", response.Message);
    }

    [Fact]
    public async Task GetCourseById_ReturnsCourse_WhenFound()
    {
        // Arrange
        var courseDto = new ViewCourseDto { Id = 1, Title = "Test Course" };
        _mockCourseService.Setup(s => s.GetCourseById(1)).ReturnsAsync(courseDto);
        _mockCacheService.Setup(c => c.GetCache<ViewCourseDto>("Course_1")).Returns((ViewCourseDto)null);

        // Act
        var result = await _controller.GetCourseById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<Response<ViewCourseDto>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Equal(1, response.Data.Id);

    }

    [Fact]
    public async Task GetCourseById_ReturnsNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        _mockCourseService.Setup(s => s.GetCourseById(1)).ReturnsAsync((ViewCourseDto)null);
        _mockCacheService.Setup(c => c.GetCache<ViewCourseDto>("Course_1")).Returns((ViewCourseDto)null);

        // Act
        var result = await _controller.GetCourseById(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<Response<string>>(notFoundResult.Value);
        //  Assert.False(response.Success);
        Assert.Equal("Course not found", response.Message);
    }

    [Fact]
    public async Task GetAllCourses_ReturnsCourses_WhenAvailable()
    {
        // Arrange
        var courses = new List<ViewCourseDto>
        {
            new ViewCourseDto { Id = 1, Title = "Course 1" },
            new ViewCourseDto { Id = 2, Title = "Course 2" }
        };
        _mockCourseService.Setup(s => s.GetAllCourses()).ReturnsAsync(courses);
        _mockCacheService.Setup(c => c.GetCache<IEnumerable<ViewCourseDto>>("AllCourses")).Returns((IEnumerable<ViewCourseDto>)null);

        // Act
        var result = await _controller.GetAllCourses();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<Response<IEnumerable<ViewCourseDto>>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Equal(2, response.Data.Count());
    }

    [Fact]
    public async Task GetAllCourses_ReturnsNotFound_WhenNoCoursesExist()
    {
        // Arrange
        _mockCourseService.Setup(s => s.GetAllCourses()).ReturnsAsync(new List<ViewCourseDto>());
        _mockCacheService.Setup(c => c.GetCache<IEnumerable<ViewCourseDto>>("AllCourses")).Returns((IEnumerable<ViewCourseDto>)null);

        // Act
        var result = await _controller.GetAllCourses();

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<Response<string>>(notFoundResult.Value);
        /// Assert.False(response.Success);
        Assert.Equal("No courses found", response.Message);
    }
}
