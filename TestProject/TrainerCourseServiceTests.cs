using BusinessAccessLayer.DTOS.TrainerCourseDtos;
using BusinessAccessLayer.Exceptions;
using BusinessAccessLayer.Services.TrainerCourseService;
using DataAccessLayer.Data.Models;
using DataAccessLayer.Repositories.CourseRepo;
using DataAccessLayer.Repositories.CourseTrainerRepo;
using DataAccessLayer.Repositories.TrainerRepo;
using Moq;

public class TrainerCourseServiceTests
{
    private readonly Mock<ICourseTrainerRepo> _mockCourseTrainerRepo;
    private readonly Mock<ICourseRepo> _mockCourseRepo;
    private readonly Mock<ITrainerRepo> _mockTrainerRepo;
    private readonly TrainerCourseService _service;

    public TrainerCourseServiceTests()
    {
        _mockCourseTrainerRepo = new Mock<ICourseTrainerRepo>();
        _mockCourseRepo = new Mock<ICourseRepo>();
        _mockTrainerRepo = new Mock<ITrainerRepo>();

        _service = new TrainerCourseService(_mockCourseTrainerRepo.Object, _mockCourseRepo.Object, _mockTrainerRepo.Object);
    }

    [Fact]
    public async Task AddCourseToTrainer_ShouldReturnTrue_WhenCourseIsAssignedSuccessfully()
    {
        // Arrange
        var dto = new AddCourseToTrainerDto { TrainerId = 1, CourseId = 101, Amount = 50m };
        var course = new Course { Id = 101, Title = "C# Basics", Price = 200m };
        var trainer = new Trainer { Id = 1, FullName = "John Doe" };

        _mockCourseRepo.Setup(repo => repo.GetByIdAsync(dto.CourseId)).ReturnsAsync(course);
        _mockTrainerRepo.Setup(repo => repo.GetByIdAsync(dto.TrainerId)).ReturnsAsync(trainer);
        _mockCourseTrainerRepo.Setup(repo => repo.GetCourseTrainer(dto.TrainerId, dto.CourseId)).ReturnsAsync((CourseTrainer)null);
        _mockCourseTrainerRepo.Setup(repo => repo.Add(It.IsAny<CourseTrainer>())).Returns(Task.CompletedTask);
        _mockCourseTrainerRepo.Setup(repo => repo.CompleteAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.AddCourseToTrainer(dto);

        // Assert
        Assert.True(result);
        _mockCourseTrainerRepo.Verify(repo => repo.Add(It.IsAny<CourseTrainer>()), Times.Once);
    }

    [Fact]
    public async Task AddCourseToTrainer_ShouldThrowNotFoundException_WhenCourseDoesNotExist()
    {
        // Arrange
        var dto = new AddCourseToTrainerDto { TrainerId = 1, CourseId = 101, Amount = 50m };

        _mockCourseRepo.Setup(repo => repo.GetByIdAsync(dto.CourseId)).ReturnsAsync((Course)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.AddCourseToTrainer(dto));
    }

    [Fact]
    public async Task AddCourseToTrainer_ShouldThrowNotFoundException_WhenTrainerDoesNotExist()
    {
        // Arrange
        var dto = new AddCourseToTrainerDto { TrainerId = 1, CourseId = 101, Amount = 50m };
        var course = new Course { Id = 101, Title = "C# Basics", Price = 200m };

        _mockCourseRepo.Setup(repo => repo.GetByIdAsync(dto.CourseId)).ReturnsAsync(course);
        _mockTrainerRepo.Setup(repo => repo.GetByIdAsync(dto.TrainerId)).ReturnsAsync((Trainer)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.AddCourseToTrainer(dto));
    }

    [Fact]
    public async Task AddCourseToTrainer_ShouldThrowBadRequestException_WhenTrainerAlreadyHasCourse()
    {
        // Arrange
        var dto = new AddCourseToTrainerDto { TrainerId = 1, CourseId = 101, Amount = 50m };
        var course = new Course { Id = 101, Title = "C# Basics", Price = 200m };
        var trainer = new Trainer { Id = 1, FullName = "John Doe" };
        var existingCourseTrainer = new CourseTrainer { TrainerId = 1, CourseId = 101 };

        _mockCourseRepo.Setup(repo => repo.GetByIdAsync(dto.CourseId)).ReturnsAsync(course);
        _mockTrainerRepo.Setup(repo => repo.GetByIdAsync(dto.TrainerId)).ReturnsAsync(trainer);
        _mockCourseTrainerRepo.Setup(repo => repo.GetCourseTrainer(dto.TrainerId, dto.CourseId)).ReturnsAsync(existingCourseTrainer);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequstExeption>(() => _service.AddCourseToTrainer(dto));
    }

    [Fact]
    public async Task GetCourseTrainers_ShouldReturnListOfCourses_WhenTrainerHasCourses()
    {
        // Arrange
        int trainerId = 1;
        var courseTrainers = new List<CourseTrainer>
        {
            new CourseTrainer
            {
                TrainerId = 1,
                CourseId = 101,
                Course = new Course { Id = 101, Title = "C# Basics", Description = "Learn C#", Price = 200m }
            }
        };

        _mockCourseTrainerRepo.Setup(repo => repo.GetCourseTrainers(trainerId)).ReturnsAsync(courseTrainers);

        // Act
        var result = await _service.GetCourseTrainers(trainerId);

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(1, result.Count());
        Assert.Equal("C# Basics", result.First().Title);
    }

    [Fact]
    public async Task GetCourseTrainersWithPayments_ShouldReturnListOfCoursesWithPayments()
    {
        // Arrange
        var courseTrainers = new List<CourseTrainer>
        {
            new CourseTrainer
            {
                TrainerId = 1,
                CourseId = 101,
                Trainer = new Trainer { Id = 1, FullName = "John Doe", Email = "john@example.com", PhoneNumber = "1234567890" },
                Course = new Course { Id = 101, Title = "C# Basics", Description = "Learn C#", Price = 200m },
                Payments = new List<Payment>
                {
                    new Payment { Id = 1, Amount = 50m, RemainingAmount = 150m, PaymentStatusForCourse = PaymentStatusForCourse.PartiallyPaid, CreatedAt = DateTime.UtcNow }
                }
            }
        };

        _mockCourseTrainerRepo.Setup(repo => repo.GetCourseTrainersWithPayments()).ReturnsAsync(courseTrainers);

        // Act
        var result = await _service.GetCourseTrainersWithPayments();

        // Assert
        Assert.NotEmpty(result);
        Assert.Single(result);
        Assert.Equal("John Doe", result.First().Trainer.FullName);
        Assert.Equal(50m, result.First().Payments.First().Amount);
    }
}
