using BusinessAccessLayer.DTOS.PaymentDtos;
using BusinessAccessLayer.DTOS.TrainerCourseDtos;
using BusinessAccessLayer.Exceptions;
using DataAccessLayer.Data.Models;
using DataAccessLayer.Repositories.CourseRepo;
using DataAccessLayer.Repositories.CourseTrainerRepo;
using DataAccessLayer.Repositories.PaymentRepo;
using DataAccessLayer.Repositories.TrainerRepo;

namespace BusinessAccessLayer.Services.PaymentServise
{
    public class PaymentServise : IPaymentService
    {
        private readonly IPaymentRepo paymentRepo;
        private readonly ICourseTrainerRepo courseTrainerRepo;
        private readonly ICourseRepo courseRepo;
        private readonly ITrainerRepo trainerRepo;
        public PaymentServise(IPaymentRepo paymentRepo, ICourseTrainerRepo courseTrainerRepo, ICourseRepo courseRepo, ITrainerRepo trainerRepo)
        {
            this.paymentRepo = paymentRepo;
            this.courseTrainerRepo = courseTrainerRepo;
            this.courseRepo = courseRepo;
            this.trainerRepo = trainerRepo;
        }

        public async Task<bool> AddPaymentToCourse(AddCourseToTrainerDto courseTrainerDto)
        {
            var course = await ValidateTrainerCourse(courseTrainerDto);
            var LastPayment = await ValidatePaymentStatus(courseTrainerDto);
            var payment = new Payment()
            {
                CreatedAt = DateTime.UtcNow,
                TrainerId = courseTrainerDto.TrainerId,
                CourseId = courseTrainerDto.CourseId,
                Amount = courseTrainerDto.Amount,
                RemainingAmount = await GetReamingOfPayment(LastPayment.RemainingAmount, courseTrainerDto.Amount),
                PaymentStatusForCourse = await GetStatusOfPayment(LastPayment.RemainingAmount, courseTrainerDto.Amount),
            };
            await paymentRepo.AddAsync(payment);
            return await paymentRepo.Complete() > 0;
        }
        private async Task<Payment> ValidatePaymentStatus(AddCourseToTrainerDto courseTrainerDto)
        {
            var Payment = await paymentRepo.GetLastPayment(courseTrainerDto.TrainerId, courseTrainerDto.CourseId);
            if (Payment.PaymentStatusForCourse == PaymentStatusForCourse.FullyPaid)
                throw new BadRequstExeption("Trainer Is Aready FullyPaid");
            return Payment;
        }
        private async Task<PaymentStatusForCourse> GetStatusOfPayment(decimal price, decimal amount)
        {
            var Reamining = price - amount;
            //if (amount == 0)
            //    return PaymentStatusForCourse.Pending;
            if (Reamining == 0)
                return PaymentStatusForCourse.FullyPaid;
            if (Reamining > 0)
                return PaymentStatusForCourse.PartiallyPaid;

            return PaymentStatusForCourse.Pending;
        }
        private async Task<decimal> GetReamingOfPayment(decimal price, decimal amount)
        {
            var Reamining = price - amount;
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
            if (trainerCource == null)
                throw new BadRequstExeption("Trainer Is Not Have Course");
            return course;
        }
        public async Task<IEnumerable<TrainerPaymentsDto>> GetTrainerPayments(int trainerId, int courseId)
        {
            var result = await paymentRepo.GetTrainerPayments(trainerId, courseId);
            return result.Select(e => new TrainerPaymentsDto()
            {
                Id = e.Id,
                PaymentStatusForCourse = e.PaymentStatusForCourse,
                TrainerId = e.TrainerId,
                CourseId = e.CourseId,
                RemainingAmount = e.RemainingAmount,
                Amount = e.Amount,
                CreatedAt = e.CreatedAt,
            }).ToList();
        }
    }
}
