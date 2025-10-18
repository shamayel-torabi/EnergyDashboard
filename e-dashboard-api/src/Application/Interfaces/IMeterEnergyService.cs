using EnergyDashboard.Application.Meters;
using EnergyDashboard.Application.Models.Meters;

namespace EnergyDashboard.Application.Interfaces;

public interface IMeterEnergyService
{
    Task<List<MeterEnergyList>> GetMeterEnergyFromCacheByDate(DateTime date, CancellationToken cancellationToken);
    Task<List<MeterEnergyList>> GetTransformersMeterEnergyFromCacheByDate(DateTime date, CancellationToken cancellationToken);
    Task<List<MeterDto>> GetMetersFromCash(CancellationToken cancellationToken);
}
