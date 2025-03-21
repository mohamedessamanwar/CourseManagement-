using DataAccessLayer.Data.Models;

namespace DataAccessLayer.Repositories.CourseTrainerRepo
{
    public interface ICourseTrainerRepo
    {
        Task<CourseTrainer?> GetCourseTrainer(int TrainerId, int CourseId);
        Task<int> CompleteAsync();
        Task Add(CourseTrainer courseTrainer);
        Task<IEnumerable<CourseTrainer>> GetCourseTrainers(int trainerId);
        Task<IEnumerable<CourseTrainer>> GetCourseTrainersWithPayments();
    }
}
