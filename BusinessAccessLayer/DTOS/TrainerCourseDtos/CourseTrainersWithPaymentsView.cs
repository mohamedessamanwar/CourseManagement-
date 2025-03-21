using BusinessAccessLayer.DTOS.CourceDtos;
using BusinessAccessLayer.DTOS.PaymentDtos;
using BusinessAccessLayer.DTOS.TrainerDtos;


namespace BusinessAccessLayer.DTOS.TrainerCourseDtos
{
    public class CourseTrainersWithPaymentsView
    {
        public int CourseId { get; set; }
        public int TrainerId { get; set; }
        public TrainerViewDto Trainer { get; set; }
        public ViewCourseDto Course { get; set; }
        public List<TrainerPaymentsDto> Payments { get; set; }

    }
}
