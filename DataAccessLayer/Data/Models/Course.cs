
namespace DataAccessLayer.Data.Models
{
    public class Course : BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        // Navigation Properties
        public ICollection<CourseTrainer> CourseTrainers { get; set; } = new List<CourseTrainer>();

    }
}
