
namespace Domain.Common;

public abstract class DomainEvent : IDomainEvent
{
    protected DomainEvent()
    {
        Id= Guid.NewGuid();
        DateOccurred = DateTimeOffset.UtcNow;
    }

    public Guid Id { get; }
    public bool IsPublished { get; set; }
    public DateTimeOffset DateOccurred { get; } 
}
