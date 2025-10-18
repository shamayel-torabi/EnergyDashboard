using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeterFailService.Domain.Entities;

namespace MeterFailService.Infrastructure.Persistence.Configurations;

public sealed class MeterEnergyConfiguration : IEntityTypeConfiguration<MeterEnergyEntity>
{
    public void Configure(EntityTypeBuilder<MeterEnergyEntity> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Ignore(e => e.MeterId);
        builder.Property(e => e.SerialNumber).IsRequired().HasMaxLength(50);
        builder.Property(e => e.RecordDate).IsRequired();
        builder.Property(e => e.HourEnergys).IsRequired().HasColumnType("jsonb");
        builder.Property(e => e.Anomaly).IsRequired().HasDefaultValue(false);
    }
}