using DataAccessLayer.Data;
using DataAccessLayer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;


namespace DataAccessLayer.Repositories.GenericRepo
{
    public class GenericRepo<T> : IGenericRepo<T> where T : class, BaseEntity
    {
        protected readonly ApplicationDbContext restaurantContext;
        public GenericRepo(ApplicationDbContext context)
        {
            this.restaurantContext = context;
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            return await restaurantContext.Set<T>().AsNoTracking().ToListAsync();
        }
        public async Task<T?> GetByIdAsync(int id)
        {
            return await restaurantContext.Set<T>().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }
        public async Task<int> AddAsync(T entity)
        {
            await restaurantContext.Set<T>().AddAsync(entity);
            await restaurantContext.SaveChangesAsync();
            return entity.Id;
        }

        public void Update(T entity, params string[] properties)
        {
            // dbEntity is object entity . 
            var dbEntity = restaurantContext.Set<T>().Local.FirstOrDefault(e => e.Id == entity.Id);
            EntityEntry entryEntry;
            if (dbEntity == null)
            {
                entryEntry = restaurantContext.Set<T>().Attach(entity);  // Attach the entity to the context , the entity’s state will typically be Unchanged.
            }
            else
            {
                entryEntry = restaurantContext.ChangeTracker.Entries<T>().Where
                    (e => e.Entity.Id == entity.Id).FirstOrDefault();
            }


            foreach (var property in entryEntry.Properties)
            {
                if (properties.Contains(property.Metadata.Name))
                {
                    property.CurrentValue = entity.GetType().GetProperty(property.Metadata.Name).GetValue(entity);
                    property.IsModified = true;
                }
            }
        }

        public void Delete(T entity)
        {
            restaurantContext.Set<T>().Remove(entity);

        }
        public void DeleteBulk(IEnumerable<T> entities)
        {
            restaurantContext.Set<T>().RemoveRange(entities);
        }

        public async Task<int> Complete()
        {
            return await restaurantContext.SaveChangesAsync();
        }
    }
}
