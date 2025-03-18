namespace DataAccessLayer.Data.Models
{
    public class Trainer : BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public ICollection<CourseTrainer> CourseTrainers { get; set; } = new List<CourseTrainer>();
        //  public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
