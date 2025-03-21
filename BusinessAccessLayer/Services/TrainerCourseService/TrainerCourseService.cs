using BusinessAccessLayer.DTOS.PaymentDtos;
using BusinessAccessLayer.DTOS.TrainerCourseDtos;
using BusinessAccessLayer.DTOS.TrainerDtos;
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
                Payments = new List<Payment>()
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
        public async Task<IEnumerable<CourseTrainersViewDto>> GetCourseTrainers(int trainerId)
        {
            var CourseTrainers = await courseTrainerRepo.GetCourseTrainers(trainerId);
            return CourseTrainers.Select(e => new CourseTrainersViewDto()
            {
                TrainerId = e.TrainerId,
                CourseId = e.CourseId,
                Title = e.Course.Title,
                Description = e.Course.Description,
                Price = e.Course.Price,

            }).ToList();

        }

        public async Task<IEnumerable<CourseTrainersWithPaymentsView>> GetCourseTrainersWithPayments()
        {
            var result = await courseTrainerRepo.GetCourseTrainersWithPayments();
            List<CourseTrainersWithPaymentsView>? courseTrainersWithPayments = result.Select(e =>
            new CourseTrainersWithPaymentsView()
            {
                TrainerId = e.TrainerId,
                CourseId = e.CourseId,
                Trainer = new TrainerViewDto()
                {
                    FullName = e.Trainer.FullName,
                    Email = e.Trainer.Email,
                    PhoneNumber = e.Trainer.PhoneNumber,

                },
                Course = new DTOS.CourceDtos.ViewCourseDto()
                {
                    Title = e.Course.Title,
                    Description = e.Course.Description,
                    Price = e.Course.Price,
                },
                Payments = e.Payments.Select(e => new TrainerPaymentsDto()
                {
                    Id = e.Id,
                    PaymentStatusForCourse = e.PaymentStatusForCourse,
                    TrainerId = e.TrainerId,
                    CourseId = e.CourseId,
                    RemainingAmount = e.RemainingAmount,
                    Amount = e.Amount,
                    CreatedAt = e.CreatedAt,

                }).ToList(),

            }).ToList();

            return courseTrainersWithPayments;
        }
    }
}

