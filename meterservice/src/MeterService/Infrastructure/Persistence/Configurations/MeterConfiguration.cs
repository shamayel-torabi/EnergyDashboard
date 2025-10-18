using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeterService.Domain.Entities;

namespace MeterService.Infrastructure.Persistence.Configurations;

public sealed class MeterConfiguration : IEntityTypeConfiguration<MeterEntity>
{
    public void Configure(EntityTypeBuilder<MeterEntity> builder)
    {
        builder.HasKey(o => o.MeterId);
        builder.Property(e => e.MeterId).ValueGeneratedNever();

        builder.Property(e => e.SerialNumber).HasMaxLength(50);
        builder.Property(e => e.Name).HasMaxLength(100);
        builder.Property(e => e.StationName).HasMaxLength(100);

        builder.Property(e => e.Active).HasDefaultValue(false);
        builder.Property(e => e.Dismount).HasDefaultValue(false);

        //builder.Property(e => e.StartOperationDate).HasColumnType("timestamp");
        //builder.Property(e => e.EndOperationDate).HasColumnType("datetime2");
    }
}