using BusinessAccessLayer.DTOS.TrainerCourseDtos;
using BusinessAccessLayer.Exceptions;
using DataAccessLayer.Data.Models;
using DataAccessLayer.Repositories.CourseRepo;
using DataAccessLayer.Repositories.CourseTrainerRepo;
using DataAccessLayer.Repositories.TrainerRepo;

namespace BusinessAccessLayer.Services.TrainerCourseService
{
    public class TrainerCourseService : ITrainerCourseService
    {
        private readonly ICourseTrainerRepo courseTrainerRepo;
        private readonly ICourseRepo courseRepo;
        private readonly ITrainerRepo trainerRepo;

        public TrainerCourseService(ICourseTrainerRepo courseTrainerRepo, ICourseRepo courseRepo, ITrainerRepo trainerRepo)
        {
            this.courseTrainerRepo = courseTrainerRepo;
            this.courseRepo = courseRepo;
            this.trainerRepo = trainerRepo;
        }

        public async Task<bool> AddCourseToTrainer(AddCourseToTrainerDto courseTrainerDto)
        {
            var course = await ValidateTrainerCourse(courseTrainerDto);
            var trainerCourse = new CourseTrainer()
            {
                TrainerId = courseTrainerDto.TrainerId,
                CourseId = courseTrainerDto.CourseId,
                Payments = new List<Payment>() // Initialize the list before adding items
            };

            trainerCourse.Payments.Add(new Payment()
            {
                Amount = courseTrainerDto.Amount,
                RemainingAmount = await GetReamingOfPayment(course.Price, courseTrainerDto.Amount),
                PaymentStatusForCourse = await GetStatusOfPayment(course.Price, courseTrainerDto.Amount),
                CreatedAt = DateTime.UtcNow,
            });
            await courseTrainerRepo.Add(trainerCourse);
            return await courseTrainerRepo.CompleteAsync() > 0;
        }
        private async Task<PaymentStatusForCourse> GetStatusOfPayment(decimal price, decimal amount)
        {
            var Reamining = price - amount;
            if (amount == 0)
                return PaymentStatusForCourse.Pending;
            if (Reamining == 0)
                return PaymentStatusForCourse.FullyPaid;
            if (Reamining > 0)
                return PaymentStatusForCourse.PartiallyPaid;

            return PaymentStatusForCourse.Pending;
        }
        private async Task<decimal> GetReamingOfPayment(decimal price, decimal amount)
        {
            var Reamining = price - amount;
            if (Reamining == 0)
                return 0;
            else
                return Reamining;
        }

        private async Task<Course> ValidateTrainerCourse(AddCourseToTrainerDto courseTrainerDto)
        {
            var course = await courseRepo.GetByIdAsync(courseTrainerDto.CourseId);
            if (course == null)
                throw new NotFoundException("Course Is Not Found");
            var Trainer = await trainerRepo.GetByIdAsync(courseTrainerDto.TrainerId);
            if (Trainer == null)
                throw new NotFoundException("Trainer Is Not Found");
            var trainerCource = await courseTrainerRepo
                .GetCourseTrainer(courseTrainerDto.TrainerId, courseTrainerDto.CourseId);
            if (trainerCource != null)
                throw new BadRequstExeption("Trainer Is Aready have Course");
            return course;
        }


    }
}

