using MeterFailService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeterFailService.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<MeterEnergyEntity> MeterEnergys { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}
