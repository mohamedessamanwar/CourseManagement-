using DataAccessLayer.Data;
using DataAccessLayer.Data.Models;
using DataAccessLayer.Repositories.GenericRepo;

namespace DataAccessLayer.Repositories.PaymentRepo
{
    public class PaymentRepo : GenericRepo<Payment>, IPaymentRepo
    {
        public PaymentRepo(ApplicationDbContext context) : base(context)
        {
        }
    }
}
