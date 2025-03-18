using DataAccessLayer.Data;
using DataAccessLayer.Data.Models;
using DataAccessLayer.Repositories.GenericRepo;


namespace DataAccessLayer.Repositories.TrainerRepo
{
    public class TrainerRepo : GenericRepo<Trainer>, ITrainerRepo
    {
        public TrainerRepo(ApplicationDbContext context) : base(context)
        {
        }
    }
}
