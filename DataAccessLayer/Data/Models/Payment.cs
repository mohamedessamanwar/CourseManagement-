
namespace DataAccessLayer.Data.Models
{
    public class Payment : BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CourseId { get; set; }  // FK to Course
        public int TrainerId { get; set; } // FK to Trainer
        public decimal Amount { get; set; }
        public PaymentStatusForCourse PaymentStatusForCourse { get; set; }
        public decimal RemainingAmount { get; set; }

        // Navigation Property
        public CourseTrainer CourseTrainer { get; set; } = null!;
    }

    public enum PaymentStatusForCourse
    {
        Pending = 0,   // لم يتم الدفع بالكامل
        PartiallyPaid = 1, // تم الدفع جزئيًا
        FullyPaid = 2  // تم الدفع بالكامل
    }

}
