using BusinessAccessLayer.DTOS.TrainerDtos;
using BusinessAccessLayer.Exceptions;
using DataAccessLayer.Data.Models;
using DataAccessLayer.Repositories.TrainerRepo;

namespace BusinessAccessLayer.Services.TrainerService
{
    public class TrainerService : ITrainerService
    {
        private readonly ITrainerRepo trainerRepo;

        public TrainerService(ITrainerRepo trainerRepo)
        {
            this.trainerRepo = trainerRepo;
        }

        public async Task<bool> AddTrainer(CreateTrainerDto createTrainerDto)
        {
            await trainerRepo.AddAsync(new Trainer()
            {
                CreatedAt = DateTime.UtcNow,
                Email = createTrainerDto.Email,
                PhoneNumber = createTrainerDto.PhoneNumber,
                FullName = createTrainerDto.FullName,
            });
            //var r = await trainerRepo.Complete();
            var isSaved = (await trainerRepo.Complete()) > 0;
            return isSaved;
        }
        public async Task<bool> UpdateTrainer(CreateTrainerDto createTrainerDto, int id)
        {
            var Trainer = await trainerRepo.GetByIdAsync(id);
            if (Trainer == null)
                throw new NotFountException("Trainer is Not Found");
            Trainer.PhoneNumber = createTrainerDto.PhoneNumber;
            Trainer.FullName = createTrainerDto.FullName;
            Trainer.Email = createTrainerDto.Email;
            trainerRepo.Update(Trainer, nameof(Trainer.PhoneNumber), nameof(Trainer.FullName), nameof(Trainer.Email));
            var isSaved = await trainerRepo.Complete() > 0;
            return isSaved;
        }
        public async Task<bool> DeleteTrainer(int id)
        {
            var trainer = await trainerRepo.GetByIdAsync(id);
            if (trainer == null)
                throw new NotFountException("Trainer is Not Found");

            trainerRepo.Delete(trainer);
            var isSaved = await trainerRepo.Complete() > 0;
            return isSaved;
        }
        public async Task<TrainerViewDto> ViewTrainer(int id)
        {
            var trainer = await trainerRepo.GetByIdAsync(id);
            if (trainer == null)
                throw new NotFountException("Trainer is Not Found");
            return new TrainerViewDto()
            {
                Id = trainer.Id,
                FullName = trainer.FullName,
                Email = trainer.Email,
                PhoneNumber = trainer.PhoneNumber,
                CreatedAt = trainer.CreatedAt,
                UpdatedAt = trainer.UpdatedAt,
            };
        }
        public async Task<IEnumerable<TrainerViewDto>> GetAllTrainers()
        {
            var trainers = await trainerRepo.GetAll();
            return trainers.Select(trainer => new TrainerViewDto
            {
                Id = trainer.Id,
                FullName = trainer.FullName,
                Email = trainer.Email,
                PhoneNumber = trainer.PhoneNumber,
                CreatedAt = trainer.CreatedAt,
                UpdatedAt = trainer.UpdatedAt
            }).ToList();
        }



    }
}
