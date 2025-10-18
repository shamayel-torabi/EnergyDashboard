
namespace Domain.Common;

public abstract class AgrigateRoot<T> : Entity<T>, IAgrigateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();


    protected AgrigateRoot(T id) : base(id)
    {
    }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
