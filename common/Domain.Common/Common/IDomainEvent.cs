using MediatR;

namespace Domain.Common;

public interface IDomainEvent : INotification
{
    Guid Id { get; }
    bool IsPublished { get; set; }
    DateTimeOffset DateOccurred { get; }
}
