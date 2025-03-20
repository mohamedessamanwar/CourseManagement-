using BusinessAccessLayer.DTOS.TrainerCourseDtos;

namespace BusinessAccessLayer.Services.TrainerCourseService
{
    public interface ITrainerCourseService
    {
        Task<bool> AddCourseToTrainer(AddCourseToTrainerDto courseTrainerDto);
    }
}
