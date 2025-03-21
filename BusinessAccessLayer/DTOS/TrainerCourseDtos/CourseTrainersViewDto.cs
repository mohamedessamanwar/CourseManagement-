

namespace BusinessAccessLayer.DTOS.TrainerCourseDtos
{
    public class CourseTrainersViewDto
    {
        public int CourseId { get; set; }
        public int TrainerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }

    }
}
