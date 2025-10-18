
using Domain.Common;
using System.Linq.Expressions;

namespace EnergyDashboard.Domain.Repository;

#nullable enable
public abstract class Specification<TKey,TEntity>
    where TEntity : Entity<TKey>
{
    public bool IsSplitQuery { get; protected set; }
    public Expression<Func<TEntity, bool>>? Predicate { get; protected set; }
    public List<Expression<Func<TEntity, object>>> IncludeExpression { get; } = new();
    public Expression<Func<TEntity, object>>? OrderByExpression { get; private set; }
    public Expression<Func<TEntity, object>>? OrderByDescendingExpression { get; private set; }

    protected void AddInclude(Expression<Func<TEntity, object>> includeExpression) =>
        IncludeExpression.Add(includeExpression);
    protected void AddOrderBy(Expression<Func<TEntity, object>> orderByExpression) =>
        OrderByExpression = orderByExpression;
    protected void AddOrderByDesending(Expression<Func<TEntity, object>> orderByDescendingExpression) =>
    OrderByDescendingExpression = orderByDescendingExpression;
}

#nullable disable
