using System;
using System.Threading;
using System.Threading.Tasks;

namespace MeterService.Application.Interfaces;

public interface IModamService
{
    Task UpdateMeterInstantAsync(DateTime startDate, DateTime endDate, string serialNumber, CancellationToken cancellationToken);
}
