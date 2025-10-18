
using System;
#nullable enable

namespace MeterService.Application.Models;

public sealed class QueueWorkItem
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool Update { get; set; }
    public string? SerialNumber { get; set; } = null;
}
