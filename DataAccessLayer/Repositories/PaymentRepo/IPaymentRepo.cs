using DataAccessLayer.Data.Models;
using DataAccessLayer.Repositories.GenericRepo;


namespace DataAccessLayer.Repositories.PaymentRepo
{
    public interface IPaymentRepo : IGenericRepo<Payment>
    {
        Task<Payment?> GetLastPayment(int TrainerId, int CourseId);
    }
}
