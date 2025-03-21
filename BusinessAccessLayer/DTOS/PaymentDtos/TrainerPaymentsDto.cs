using DataAccessLayer.Data.Models;

namespace BusinessAccessLayer.DTOS.PaymentDtos
{
    public class TrainerPaymentsDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CourseId { get; set; }  // FK to Course
        public int TrainerId { get; set; } // FK to Trainer
        public decimal Amount { get; set; }
        public PaymentStatusForCourse PaymentStatusForCourse { get; set; }
        public decimal RemainingAmount { get; set; }
    }
}
