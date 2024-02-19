using FonTech.Domain.Interfaces.Repositories;

namespace FonTech.DAL.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _context;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            await _context.AddAsync(entity);
            return entity;
        }

        public IQueryable<TEntity> GetAll()
        {
            return _context.Set<TEntity>();
        }

        public void Remove(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            _context.Set<TEntity>().Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public TEntity Update(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            _context.Set<TEntity>().Update(entity);
            return entity;
        }
    }
}
