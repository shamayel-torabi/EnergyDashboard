using MediatR;

namespace MeterFailService.Application.Common.Core;

/// <summary>
/// Represents the marker interface for an integration event.
/// </summary>
public interface IIntegrationEvent : INotification
{
}
