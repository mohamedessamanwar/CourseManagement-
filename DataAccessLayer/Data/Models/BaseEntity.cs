namespace DataAccessLayer.Data.Models
{

    public interface BaseEntity
    {
        int Id { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
    }



}
