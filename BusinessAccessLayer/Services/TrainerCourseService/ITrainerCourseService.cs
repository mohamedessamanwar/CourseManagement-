using BusinessAccessLayer.DTOS.TrainerCourseDtos;

namespace BusinessAccessLayer.Services.TrainerCourseService
{
    public interface ITrainerCourseService
    {
        Task<bool> AddCourseToTrainer(AddCourseToTrainerDto courseTrainerDto);
        Task<IEnumerable<CourseTrainersViewDto>> GetCourseTrainers(int trainerId);
        Task<IEnumerable<CourseTrainersWithPaymentsView>> GetCourseTrainersWithPayments();
    }
}
