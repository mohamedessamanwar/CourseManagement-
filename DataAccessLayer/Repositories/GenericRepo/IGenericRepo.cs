
namespace DataAccessLayer.Repositories.GenericRepo
{
    public interface IGenericRepo<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetByIdAsync(int id);
        Task<int> AddAsync(T entity);
        void Update(T entity, params string[] properties);
        void Delete(T entity);
        void DeleteBulk(IEnumerable<T> entities);
        Task<int> Complete();
    }
}
