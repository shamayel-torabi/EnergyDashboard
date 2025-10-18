using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Domain.Repository;

public interface IDailyEnergyRepository : IRepository<Guid, DailyEnergy>
{
    Task<IEnumerable<DailyEnergy>> GetLoadFeedersMonthlyEnergyAsync(int year, int month, int loadFeederTypeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DailyEnergy>> GetLoadFeedersYearlyEnergyAsync(int year, int loadFeederTypeId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DailyEnergy>> GetLoadFeedersByDateEnergyAsync(DateTime date, int loadFeederTypeId, CancellationToken cancellationToken = default);

    Task<IEnumerable<DailyEnergy>> GetGeneratorFeedersMonthlyEnergyAsync(int year, int month, CancellationToken cancellationToken = default);
    Task<IEnumerable<DailyEnergy>> GetGeneratorFeedersYearlyEnergyAsync(int year, CancellationToken cancellationToken = default);
    Task<IEnumerable<DailyEnergy>> GetGeneratorFeedersByDateEnergyAsync(DateTime date, CancellationToken cancellationToken = default);

    Task<IEnumerable<DailyEnergy>> GetEnergyByDateAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<IEnumerable<DailyEnergy>> GetSubstationEnergyByDateAsync(Guid substationId, DateTime date, CancellationToken cancellationToken = default);
}
