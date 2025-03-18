using BusinessAccessLayer.DTOS.TrainerDtos;

namespace BusinessAccessLayer.Services.TrainerService
{
    public interface ITrainerService
    {
        Task<bool> AddTrainer(CreateTrainerDto createTrainerDto);
        Task<bool> UpdateTrainer(CreateTrainerDto createTrainerDto, int id);
        Task<bool> DeleteTrainer(int id);
        Task<TrainerViewDto> ViewTrainer(int id);
        Task<IEnumerable<TrainerViewDto>> GetAllTrainers();
    }
}
