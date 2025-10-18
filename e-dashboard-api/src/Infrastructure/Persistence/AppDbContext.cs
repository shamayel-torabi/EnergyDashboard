using System.Reflection;
using Microsoft.EntityFrameworkCore;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Infrastructure.Persistence.Interceptors;
using MediatR;
using EnergyDashboard.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace EnergyDashboard.Infrastructure.Persistence;

public class AppDbContext : DbContext, IUnitOfWork
{
    private readonly IMediator _mediator;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;
    private readonly ILogger<AppDbContext> _logger;

    public AppDbContext( DbContextOptions<AppDbContext> options) : base(options)
    { }

    public AppDbContext(
        ILogger<AppDbContext> logger,
        DbContextOptions<AppDbContext> options,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor,
        IMediator mediator) : this(options)
    {
        _logger = logger;
        _mediator = mediator;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEvents(this);

        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                _logger.LogInformation(entry.DebugView.LongView);
            }
            throw ex;
        }
    }


    public DbSet<Network> Networks { get; set; }
    public DbSet<Area> Areas { get; set; }
    public DbSet<Zone> Zones { get; set; }
    public DbSet<Substation> Substations { get; set; }

    public DbSet<EnergyProfile> EnergyProfiles { get; set; }
    public DbSet<DailyEnergy> DailyEnergys { get; set; }

    public DbSet<Equipment> Equipments { get; set; }
    public DbSet<EquipmentOperation> EquipmentOperations { get; set; }
    public DbSet<EquipmentOperationType> EquipmentOperationTypes { get; set; }

    public DbSet<GeneratorFeederEquipment> GeneratorFeeders { get; set; }
    public DbSet<LoadFeederEquipment> LoadFeeders { get; set; }
    public DbSet<LineFeederEquipment> LineFeeders { get; set; }
    public DbSet<TransformerFeederEquipment> TransformerFeeders { get; set; }


    public DbSet<MeterReadingFailure> MeterReadingFailures { get; set; }
    public DbSet<PowerplantType> PowerplantTypes { get; set; }
    public DbSet<PowerplantOperator> PowerplantOperators { get; set; }
    public DbSet<LoadFeederType> LoadFeederTypes { get; set; }

    public DbSet<EnergyTariff> EnergyTariffs { get; set; }
}
