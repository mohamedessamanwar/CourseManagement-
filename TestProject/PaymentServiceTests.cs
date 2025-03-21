using BusinessAccessLayer.DTOS.TrainerCourseDtos;
using BusinessAccessLayer.Exceptions;
using BusinessAccessLayer.Services.PaymentServise;
using DataAccessLayer.Data.Models;
using DataAccessLayer.Repositories.CourseRepo;
using DataAccessLayer.Repositories.CourseTrainerRepo;
using DataAccessLayer.Repositories.PaymentRepo;
using DataAccessLayer.Repositories.TrainerRepo;
using Moq;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepo> _paymentRepoMock;
    private readonly Mock<ICourseTrainerRepo> _courseTrainerRepoMock;
    private readonly Mock<ICourseRepo> _courseRepoMock;
    private readonly Mock<ITrainerRepo> _trainerRepoMock;
    private readonly PaymentServise _paymentService;

    public PaymentServiceTests()
    {
        _paymentRepoMock = new Mock<IPaymentRepo>();
        _courseTrainerRepoMock = new Mock<ICourseTrainerRepo>();
        _courseRepoMock = new Mock<ICourseRepo>();
        _trainerRepoMock = new Mock<ITrainerRepo>();

        _paymentService = new PaymentServise(
            _paymentRepoMock.Object,
            _courseTrainerRepoMock.Object,
            _courseRepoMock.Object,
            _trainerRepoMock.Object);
    }

    [Fact]
    public async Task AddPaymentToCourse_ShouldThrowException_WhenCourseNotFound()
    {
        // Arrange
        var courseTrainerDto = new AddCourseToTrainerDto { CourseId = 1, TrainerId = 1, Amount = 100 };
        _courseRepoMock.Setup(repo => repo.GetByIdAsync(courseTrainerDto.CourseId))
            .ReturnsAsync((Course)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _paymentService.AddPaymentToCourse(courseTrainerDto));
    }

    [Fact]
    public async Task AddPaymentToCourse_ShouldThrowException_WhenTrainerNotFound()
    {
        // Arrange
        var courseTrainerDto = new AddCourseToTrainerDto { CourseId = 1, TrainerId = 1, Amount = 100 };
        _courseRepoMock.Setup(repo => repo.GetByIdAsync(courseTrainerDto.CourseId))
            .ReturnsAsync(new Course());
        _trainerRepoMock.Setup(repo => repo.GetByIdAsync(courseTrainerDto.TrainerId))
            .ReturnsAsync((Trainer)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _paymentService.AddPaymentToCourse(courseTrainerDto));
    }

    [Fact]
    public async Task AddPaymentToCourse_ShouldThrowException_WhenTrainerHasNoCourse()
    {
        // Arrange
        var courseTrainerDto = new AddCourseToTrainerDto { CourseId = 1, TrainerId = 1, Amount = 100 };
        _courseRepoMock.Setup(repo => repo.GetByIdAsync(courseTrainerDto.CourseId))
            .ReturnsAsync(new Course());
        _trainerRepoMock.Setup(repo => repo.GetByIdAsync(courseTrainerDto.TrainerId))
            .ReturnsAsync(new Trainer());
        _courseTrainerRepoMock.Setup(repo => repo.GetCourseTrainer(courseTrainerDto.TrainerId, courseTrainerDto.CourseId))
            .ReturnsAsync((CourseTrainer)null);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequstExeption>(async () =>
            await _paymentService.AddPaymentToCourse(courseTrainerDto));
    }

    [Fact]
    public async Task GetTrainerPayments_ShouldReturnListOfPayments()
    {
        // Arrange
        int trainerId = 1, courseId = 1;
        var payments = new List<Payment>
        {
            new Payment { Id = 1, TrainerId = trainerId, CourseId = courseId, Amount = 100, RemainingAmount = 50, PaymentStatusForCourse = PaymentStatusForCourse.PartiallyPaid, CreatedAt = DateTime.UtcNow }
        };
        _paymentRepoMock.Setup(repo => repo.GetTrainerPayments(trainerId, courseId))
            .ReturnsAsync(payments);

        // Act
        var result = await _paymentService.GetTrainerPayments(trainerId, courseId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(payments.First().Amount, result.First().Amount);
    }
}
