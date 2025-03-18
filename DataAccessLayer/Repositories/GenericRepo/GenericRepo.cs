using DataAccessLayer.Data;
using DataAccessLayer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;


namespace DataAccessLayer.Repositories.GenericRepo
{
    public class GenericRepo<T> : IGenericRepo<T> where T : class, BaseEntity
    {
        protected readonly ApplicationDbContext dbContext;
        public GenericRepo(ApplicationDbContext context)
        {
            this.dbContext = context;
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            return await dbContext.Set<T>().AsNoTracking().ToListAsync();
        }
        public async Task<T?> GetByIdAsync(int id)
        {
            return await dbContext.Set<T>().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }
        public async Task<int> AddAsync(T entity)
        {
            await dbContext.Set<T>().AddAsync(entity);
            // await dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public void Update(T entity, params string[] properties)
        {
            // dbEntity is object entity . 
            var dbEntity = dbContext.Set<T>().Local.FirstOrDefault(e => e.Id == entity.Id);
            EntityEntry entryEntry;
            if (dbEntity == null)
            {
                entryEntry = dbContext.Set<T>().Attach(entity);  // Attach the entity to the context , the entity’s state will typically be Unchanged.
            }
            else
            {
                entryEntry = dbContext.ChangeTracker.Entries<T>().Where
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
            dbContext.Set<T>().Remove(entity);

        }
        public void DeleteBulk(IEnumerable<T> entities)
        {
            dbContext.Set<T>().RemoveRange(entities);
        }

        public async Task<int> Complete()
        {
            return await dbContext.SaveChangesAsync();
        }
    }
}
