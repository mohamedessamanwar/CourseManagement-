using DataAccessLayer.Data;
using DataAccessLayer.Data.Models;
using DataAccessLayer.Repositories.GenericRepo;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories.PaymentRepo
{
    public class PaymentRepo : GenericRepo<Payment>, IPaymentRepo
    {
        public PaymentRepo(ApplicationDbContext context) : base(context)
        {

        }
        public async Task<Payment?> GetLastPayment(int TrainerId, int CourseId)
        {
            return await dbContext.Payments.AsNoTracking()
                 .Where(predicate: c => c.TrainerId == TrainerId && c.CourseId == CourseId)
                 .OrderByDescending(e => e.CreatedAt).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Payment>> GetTrainerPayments(int trainerId, int courseId)
        {
            return await dbContext.Payments.AsNoTracking()
                     .Where(predicate: c => c.TrainerId == trainerId && c.CourseId == courseId)
                 .ToListAsync();
        }
    }
}
