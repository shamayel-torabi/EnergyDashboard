

namespace Domain.Common;

public abstract class AuditableEntity<T> : Entity<T>, IAuditableEntity
{
    protected AuditableEntity(T id) : base(id)
    {
    }
    public DateTime Created { get; set; }
    public DateTime? LastModified { get; set; }
}
