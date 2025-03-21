using BusinessAccessLayer.DTOS.TrainerDtos;
using BusinessAccessLayer.Exceptions;
using BusinessAccessLayer.Services.TrainerService;
using DataAccessLayer.Data.Models;
using DataAccessLayer.Repositories.TrainerRepo;
using Moq;

namespace TestProject
{
    public class TrainerServiceTests
    {
        private readonly Mock<ITrainerRepo> mockTrainerRepo;
        private readonly TrainerService trainerService;

        public TrainerServiceTests()
        {
            mockTrainerRepo = new Mock<ITrainerRepo>();
            trainerService = new TrainerService(mockTrainerRepo.Object);
        }

        [Fact]
        public async Task AddTrainer_ShouldReturnTrue_WhenTrainerIsAddedSuccessfully()
        {
            // Arrange
            var createTrainerDto = new CreateTrainerDto
            {
                FullName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "123456789"
            };

            //mockTrainerRepo.Setup(repo => repo.AddAsync(It.IsAny<Trainer>())).Returns(Task.CompletedTask);
            mockTrainerRepo.Setup(repo => repo.Complete()).ReturnsAsync(1); // Ensure it returns an int

            // Act
            var result = await trainerService.AddTrainer(createTrainerDto);

            // Assert
            Assert.True(result);
            mockTrainerRepo.Verify(repo => repo.AddAsync(It.IsAny<Trainer>()), Times.Once);
            mockTrainerRepo.Verify(repo => repo.Complete(), Times.Once);
        }

        [Fact]
        public async Task UpdateTrainer_ShouldThrowNotFoundException_WhenTrainerDoesNotExist()
        {
            // Arrange
            var updateTrainerDto = new CreateTrainerDto
            {
                FullName = "Updated Name",
                Email = "updated.email@example.com",
                PhoneNumber = "987654321"
            };

            mockTrainerRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Trainer)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => trainerService.UpdateTrainer(updateTrainerDto, 1));
        }

        [Fact]
        public async Task UpdateTrainer_ShouldReturnTrue_WhenTrainerIsUpdatedSuccessfully()
        {
            // Arrange
            var trainer = new Trainer
            {
                Id = 1,
                FullName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "123456789"
            };

            var updateTrainerDto = new CreateTrainerDto
            {
                FullName = "Updated Name",
                Email = "updated.email@example.com",
                PhoneNumber = "987654321"
            };

            mockTrainerRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(trainer);
            mockTrainerRepo.Setup(repo => repo.Complete()).ReturnsAsync(1);

            // Act
            var result = await trainerService.UpdateTrainer(updateTrainerDto, 1);

            // Assert
            Assert.True(result);
            mockTrainerRepo.Verify(repo => repo.Update(It.IsAny<Trainer>(), It.IsAny<string[]>()), Times.Once);
            mockTrainerRepo.Verify(repo => repo.Complete(), Times.Once);
        }

        [Fact]
        public async Task DeleteTrainer_ShouldThrowNotFoundException_WhenTrainerDoesNotExist()
        {
            // Arrange
            mockTrainerRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Trainer)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => trainerService.DeleteTrainer(1));
        }

        [Fact]
        public async Task DeleteTrainer_ShouldReturnTrue_WhenTrainerIsDeletedSuccessfully()
        {
            // Arrange
            var trainer = new Trainer { Id = 1, FullName = "John Doe" };

            mockTrainerRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(trainer);
            mockTrainerRepo.Setup(repo => repo.Complete()).ReturnsAsync(1);

            // Act
            var result = await trainerService.DeleteTrainer(1);

            // Assert
            Assert.True(result);
            mockTrainerRepo.Verify(repo => repo.Delete(It.IsAny<Trainer>()), Times.Once);
            mockTrainerRepo.Verify(repo => repo.Complete(), Times.Once);
        }

        [Fact]
        public async Task ViewTrainer_ShouldThrowNotFoundException_WhenTrainerDoesNotExist()
        {
            // Arrange
            mockTrainerRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Trainer)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => trainerService.ViewTrainer(1));
        }

        [Fact]
        public async Task ViewTrainer_ShouldReturnTrainerViewDto_WhenTrainerExists()
        {
            // Arrange
            var trainer = new Trainer
            {
                Id = 1,
                FullName = "John Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "123456789",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            mockTrainerRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(trainer);

            // Act
            var result = await trainerService.ViewTrainer(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(trainer.Id, result.Id);
            Assert.Equal(trainer.FullName, result.FullName);
            Assert.Equal(trainer.Email, result.Email);
            Assert.Equal(trainer.PhoneNumber, result.PhoneNumber);
        }

        [Fact]
        public async Task GetAllTrainers_ShouldReturnListOfTrainerViewDto()
        {
            // Arrange
            var trainers = new List<Trainer>
            {
                new Trainer { Id = 1, FullName = "John Doe", Email = "john.doe@example.com", PhoneNumber = "123456789", CreatedAt = DateTime.UtcNow },
                new Trainer { Id = 2, FullName = "Jane Smith", Email = "jane.smith@example.com", PhoneNumber = "987654321", CreatedAt = DateTime.UtcNow }
            };

            mockTrainerRepo.Setup(repo => repo.GetAll()).ReturnsAsync(trainers);

            // Act
            var result = await trainerService.GetAllTrainers();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
    }
}
