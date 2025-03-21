using BusinessAccessLayer.DTOS.PaymentDtos;
using BusinessAccessLayer.DTOS.TrainerCourseDtos;


namespace BusinessAccessLayer.Services.PaymentServise
{
    public interface IPaymentService
    {
        Task<bool> AddPaymentToCourse(AddCourseToTrainerDto courseTrainerDto);
        Task<IEnumerable<TrainerPaymentsDto>> GetTrainerPayments(int trainerId, int courseId);

    }
}
