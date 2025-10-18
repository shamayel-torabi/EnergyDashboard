
using System.Xml;

namespace MeterService.gRPC.Services;

public interface IMeterClient
{
    Task<Meter> GetMeter(string serialNumber);
    Task<IEnumerable<Meter>> GetMeters();
    Task DeleteMeter(int id);
    Task UpdateMeter(Meter meter);
    Task<IEnumerable<Substation>> GetSubstations();
    Task<IEnumerable<Meter>> GetSubstationMeters(int substationId);
    Task<bool> UpdateMeterTable();
    Task<IEnumerable<MeterEnergy>> GetMetersEnergy(DateTimeOffset date);
    Task<IEnumerable<MeterEnergy>> GetMeterEnergy(DateTimeOffset date, string serialNumber);
    Task DownloadMetersEnergy(DateTimeOffset startDate, DateTimeOffset endDate, bool update = false);
    Task DownloadMeterEnergy(DateTimeOffset startDate, DateTimeOffset endDate, string serialNumber, bool update = false);
}
