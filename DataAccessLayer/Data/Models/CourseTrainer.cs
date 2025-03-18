namespace DataAccessLayer.Data.Models
{
    public class CourseTrainer
    {

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; } = null!;

        // Navigation Property for Payments
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
