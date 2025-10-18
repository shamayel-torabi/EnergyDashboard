using MediatR;

namespace Application.Common.Core;

/// <summary>
/// Represents the marker interface for an integration event.
/// </summary>
public interface IIntegrationEvent : INotification
{
}
