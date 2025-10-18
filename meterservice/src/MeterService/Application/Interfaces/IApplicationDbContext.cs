using MeterService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeterService.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<MeterEntity> Meters { get; set; }

    DbSet<MeterEnergyEntity> MeterEnergys { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}
