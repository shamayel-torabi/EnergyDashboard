using MeterFailService.Application.Interfaces;
using MeterFailService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MeterFailService.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{

    public ApplicationDbContext(
        DbContextOptions options) : base(options)
    {
    }


    public DbSet<MeterEnergyEntity> MeterEnergys { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}
