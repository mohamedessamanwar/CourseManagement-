using BusinessAccessLayer.DTOS;
using BusinessAccessLayer.DTOS.TrainerDtos;
using BusinessAccessLayer.Services.TrainerService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

public class TrainerControllerTests
{
    private readonly Mock<ITrainerService> _trainerServiceMock;
    private readonly Mock<ILogger<TrainerController>> _loggerMock;
    private readonly TrainerController _controller;

    public TrainerControllerTests()
    {
        _trainerServiceMock = new Mock<ITrainerService>();
        _loggerMock = new Mock<ILogger<TrainerController>>();
        _controller = new TrainerController(_trainerServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task AddTrainer_ShouldReturnOk_WhenTrainerIsAdded()
    {
        var dto = new CreateTrainerDto();
        _trainerServiceMock.Setup(s => s.AddTrainer(dto)).ReturnsAsync(true);

        var result = await _controller.AddTrainer(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<Response<string>>(okResult.Value);
    }

    [Fact]
    public async Task AddTrainer_ShouldReturnBadRequest_WhenAddingFails()
    {
        var dto = new CreateTrainerDto();
        _trainerServiceMock.Setup(s => s.AddTrainer(dto)).ReturnsAsync(false);

        var result = await _controller.AddTrainer(dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<Response<string>>(badRequest.Value);
    }

    [Fact]
    public async Task UpdateTrainer_ShouldReturnOk_WhenUpdateIsSuccessful()
    {
        var dto = new CreateTrainerDto();
        _trainerServiceMock.Setup(s => s.UpdateTrainer(dto, 1)).ReturnsAsync(true);

        var result = await _controller.UpdateTrainer(1, dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<Response<string>>(okResult.Value);
    }

    [Fact]
    public async Task UpdateTrainer_ShouldReturnNotFound_WhenUpdateFails()
    {
        var dto = new CreateTrainerDto();
        _trainerServiceMock.Setup(s => s.UpdateTrainer(dto, 1)).ReturnsAsync(false);

        var result = await _controller.UpdateTrainer(1, dto);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.IsType<Response<string>>(notFound.Value);
    }

    [Fact]
    public async Task DeleteTrainer_ShouldReturnOk_WhenDeletionIsSuccessful()
    {
        _trainerServiceMock.Setup(s => s.DeleteTrainer(1)).ReturnsAsync(true);

        var result = await _controller.DeleteTrainer(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<Response<string>>(okResult.Value);
    }

    [Fact]
    public async Task DeleteTrainer_ShouldReturnNotFound_WhenTrainerDoesNotExist()
    {
        _trainerServiceMock.Setup(s => s.DeleteTrainer(1)).ReturnsAsync(false);

        var result = await _controller.DeleteTrainer(1);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.IsType<Response<string>>(notFound.Value);
    }

    [Fact]
    public async Task ViewTrainer_ShouldReturnOk_WhenTrainerExists()
    {
        var trainerDto = new TrainerViewDto();
        _trainerServiceMock.Setup(s => s.ViewTrainer(1)).ReturnsAsync(trainerDto);

        var result = await _controller.ViewTrainer(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<Response<TrainerViewDto>>(okResult.Value);
    }

    [Fact]
    public async Task ViewTrainer_ShouldReturnNotFound_WhenTrainerDoesNotExist()
    {
        _trainerServiceMock.Setup(s => s.ViewTrainer(1)).ReturnsAsync((TrainerViewDto)null);

        var result = await _controller.ViewTrainer(1);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.IsType<Response<string>>(notFound.Value);
    }

    [Fact]
    public async Task GetAllTrainers_ShouldReturnOk_WithListOfTrainers()
    {
        var trainers = new List<TrainerViewDto> { new TrainerViewDto() };
        _trainerServiceMock.Setup(s => s.GetAllTrainers()).ReturnsAsync(trainers);

        var result = await _controller.GetAllTrainers();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<Response<IEnumerable<TrainerViewDto>>>(okResult.Value);
        Assert.Single(response.Data);
    }
}
