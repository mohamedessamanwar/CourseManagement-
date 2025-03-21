using BusinessAccessLayer.DTOS;
using BusinessAccessLayer.DTOS.PaymentDtos;
using BusinessAccessLayer.DTOS.TrainerCourseDtos;
using BusinessAccessLayer.Services.PaymentServise;
using CourseManagementAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

public class PaymentControllerTests
{
    private readonly Mock<IPaymentService> _mockPaymentService;
    private readonly PaymentController _controller;

    public PaymentControllerTests()
    {
        _mockPaymentService = new Mock<IPaymentService>();
        _controller = new PaymentController(_mockPaymentService.Object);
    }

    [Fact]
    public async Task AddPayment_Success_ReturnsOk()
    {
        // Arrange
        var paymentDto = new AddCourseToTrainerDto { TrainerId = 1, CourseId = 2 };
        _mockPaymentService.Setup(s => s.AddPaymentToCourse(paymentDto)).ReturnsAsync(true);

        // Act
        var result = await _controller.AddPayment(paymentDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task AddPayment_Failure_ReturnsBadRequest()
    {
        // Arrange
        var paymentDto = new AddCourseToTrainerDto { TrainerId = 1, CourseId = 2 };
        _mockPaymentService.Setup(s => s.AddPaymentToCourse(paymentDto)).ReturnsAsync(false);

        // Act
        var result = await _controller.AddPayment(paymentDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task GetTrainerPayments_ReturnsOkWithPayments()
    {
        // Arrange
        var trainerId = 1;
        var courseId = 2;
        var payments = new List<TrainerPaymentsDto>
        {
            new TrainerPaymentsDto { Id = 101, TrainerId = trainerId, Amount = 100 },
            new TrainerPaymentsDto { Id = 102, TrainerId = trainerId, Amount = 200 }
        };

        _mockPaymentService.Setup(s => s.GetTrainerPayments(trainerId, courseId)).ReturnsAsync(payments);

        // Act
        var result = await _controller.GetTrainerPayments(trainerId, courseId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        var response = Assert.IsType<Response<IEnumerable<TrainerPaymentsDto>>>(okResult.Value);
        Assert.NotNull(response.Data);
        Assert.Equal(2, response.Data.Count());
    }
}
