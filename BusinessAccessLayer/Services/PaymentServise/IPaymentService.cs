using BusinessAccessLayer.DTOS.TrainerCourseDtos;


namespace BusinessAccessLayer.Services.PaymentServise
{
    public interface IPaymentService
    {
        Task<bool> AddPaymentToCourse(AddCourseToTrainerDto courseTrainerDto);
    }
}
