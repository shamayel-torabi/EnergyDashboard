using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeterService.Domain.Entities;

namespace MeterService.Infrastructure.Persistence.Configurations;

public sealed class MeterEnergyConfiguration : IEntityTypeConfiguration<MeterEnergyEntity>
{
    public void Configure(EntityTypeBuilder<MeterEnergyEntity> builder)
    {
        builder.Ignore(e => e.DomainEvents);

        builder.HasKey(o => o.MeterEnergyId);
        builder.Property(e => e.RecordDate).IsRequired();
        builder.Property(e => e.EnergyActiveExport).IsRequired().HasPrecision(18, 0);
        builder.Property(e => e.EnergyActiveImport).IsRequired().HasPrecision(18, 0);
        builder.Property(e => e.EnergyReactiveExport).IsRequired().HasPrecision(18, 0);
        builder.Property(e => e.EnergyReactiveImport).IsRequired().HasPrecision(18, 0);

        builder.HasIndex(p => new { p.MeterId });
        builder.HasIndex(p => new { p.RecordDate });
    }
}