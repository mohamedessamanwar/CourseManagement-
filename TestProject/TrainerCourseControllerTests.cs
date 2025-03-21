using BusinessAccessLayer.DTOS.TrainerCourseDtos;
using BusinessAccessLayer.Services.TrainerCourseService;
using CourseManagementAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

public class TrainerCourseControllerTests
{
    private readonly Mock<ITrainerCourseService> _mockService;
    private readonly TrainerCourseController _controller;

    public TrainerCourseControllerTests()
    {
        _mockService = new Mock<ITrainerCourseService>();
        _controller = new TrainerCourseController(_mockService.Object);
    }

    [Fact]
    public async Task AddCourseToTrainer_ReturnsOk_WhenSuccess()
    {
        // Arrange
        var dto = new AddCourseToTrainerDto();
        _mockService.Setup(s => s.AddCourseToTrainer(dto)).ReturnsAsync(true);

        // Act
        var result = await _controller.AddCourseToTrainer(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task AddCourseToTrainer_ReturnsBadRequest_WhenFailed()
    {
        // Arrange
        var dto = new AddCourseToTrainerDto();
        _mockService.Setup(s => s.AddCourseToTrainer(dto)).ReturnsAsync(false);

        // Act
        var result = await _controller.AddCourseToTrainer(dto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetCourseToTrainer_ReturnsOk_WithData()
    {
        // Arrange
        int trainerId = 1;
        var courses = new List<CourseTrainersViewDto> { new CourseTrainersViewDto() };
        _mockService.Setup(s => s.GetCourseTrainers(trainerId)).ReturnsAsync(courses);

        // Act
        var result = await _controller.GetCourseToTrainer(trainerId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
    }
}
