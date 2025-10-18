using Domain.Common;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnergyDashboard.Infrastructure.Repository;

public abstract class Repository<TKey,TEntity> : IRepository<TKey,TEntity>
    where TEntity : Entity<TKey>
{
    protected readonly AppDbContext _context;
    private DbSet<TEntity> entities;

    public Repository(AppDbContext context)
    {
        _context = context;
        entities = _context.Set<TEntity>();
    }
    public async Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await entities.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await entities.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetBySpecificationAsync(Specification<TKey,TEntity> specification, CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public void Update(TEntity entity)
    {
        //entities.Attach(entity);
        //_context.Entry(entity).State = EntityState.Modified;
        entities.Update(entity);
    }

    public TEntity Add(TEntity entity)
    {
        var a = entities.Add(entity);
        return a.Entity;
    }

    public TEntity Remove(TEntity entity)
    {
        var a = entities.Remove(entity);
        return a.Entity;
    }

    private IQueryable<TEntity> ApplySpecification(Specification<TKey,TEntity> specification)
    {
        IQueryable<TEntity> queryable = entities;

        if (specification.IsSplitQuery)
        {
            queryable = queryable.AsSplitQuery();
        }

        if (specification.Predicate is not null)
        {
            queryable = queryable.Where(specification.Predicate);
        }

        specification.IncludeExpression.Aggregate(
            queryable,
            (current, includeExpression) => current.Include(includeExpression));

        if (specification.OrderByExpression is not null)
        {
            queryable = queryable.OrderBy(specification.OrderByExpression);
        }
        else if (specification.OrderByDescendingExpression is not null)
        {
            queryable = queryable.OrderByDescending(specification.OrderByDescendingExpression);
        }

        return queryable;
    }
}
