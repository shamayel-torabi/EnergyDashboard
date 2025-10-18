using System;
using System.Threading.Tasks;

namespace MeterService.Application.Interfaces;

public interface ISepacService
{
    Task<bool> UpdateMetersTableAsync(CancellationToken cancellationToken);
    Task<bool> DownloadMetersEnergy(DateTime startDate, DateTime endDate, bool update, CancellationToken cancellationToken);
    Task<bool> DownloadMeterEnergy(DateTime startDate, DateTime endDate, string serialNumber, bool update, CancellationToken cancellationToken);
}
