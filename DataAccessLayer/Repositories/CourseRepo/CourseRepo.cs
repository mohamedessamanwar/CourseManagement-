using DataAccessLayer.Data;
using DataAccessLayer.Data.Models;
using DataAccessLayer.Repositories.GenericRepo;


namespace DataAccessLayer.Repositories.CourseRepo
{
    public class CourseRepo : GenericRepo<Course>, ICourseRepo
    {
        public CourseRepo(ApplicationDbContext context) : base(context)
        {
        }
    }
}
