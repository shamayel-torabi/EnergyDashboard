using Infrastructure.Common.Services;
using MeterService.Application.Interfaces;
using MeterService.Domain.Common;
using MeterService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MeterService.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly IDateTimeService _dateTime;
    private readonly IDomainEventService _domainEventService;

    public ApplicationDbContext(
        DbContextOptions options) : base(options)
    {
    }

    public ApplicationDbContext(
        DbContextOptions options,
        IDateTimeService dateTime,
        IDomainEventService domainEventService) : this(options)
    {
        _domainEventService = domainEventService;
        _dateTime = dateTime;
    }

    public DbSet<MeterEntity> Meters { get; set; }

    public DbSet<MeterEnergyEntity> MeterEnergys { get; set; }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Created = _dateTime.Now;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModified = _dateTime.Now;
                    break;
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchEvents();

        return result;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    private async Task DispatchEvents()
    {
        while (true)
        {
            var domainEventEntity = ChangeTracker.Entries<IHasDomainEvent>()
                .Select(x => x.Entity.DomainEvents)
                .SelectMany(x => x)
                .Where(domainEvent => !domainEvent.IsPublished)
                .FirstOrDefault();
            if (domainEventEntity == null) break;

            domainEventEntity.IsPublished = true;
            await _domainEventService.Publish(domainEventEntity);
        }
    }
}
