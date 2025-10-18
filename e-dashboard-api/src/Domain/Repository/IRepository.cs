
using Domain.Common;

namespace EnergyDashboard.Domain.Repository;

public interface IRepository<TKey,TEntity>
    where TEntity : Entity<TKey>
{
    Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetBySpecificationAsync(Specification<TKey,TEntity> specification, CancellationToken cancellationToken = default);
    TEntity Add(TEntity entity);
    TEntity Remove(TEntity entity);
    void Update(TEntity entity);
}
