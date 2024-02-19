
using FonTech.Domain.Interfaces.Databases;

namespace FonTech.Domain.Interfaces.Repositories
{
    public interface IBaseRepository<TEntity> : IStateSaveChangesAsync
    {
        IQueryable<TEntity> GetAll();

        Task<TEntity> CreateAsync(TEntity entity);

        TEntity Update(TEntity entity);

        void Remove(TEntity entity);
    }
}
