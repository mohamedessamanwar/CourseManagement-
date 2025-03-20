

namespace BusinessAccessLayer.DTOS.TrainerCourseDtos
{
    public class AddCourseToTrainerDto
    {
        public int CourseId { get; set; }
        public int TrainerId { get; set; }
        public decimal Amount { get; set; }
    }
}
