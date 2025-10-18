using MediatR;

namespace MeterFailService.Application.Common.Core;

/// <summary>
/// Represents the event interface.
/// </summary>
public interface IEvent : INotification
{
}