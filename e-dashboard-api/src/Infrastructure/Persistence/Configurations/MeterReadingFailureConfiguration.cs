using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Infrastructure.Persistence.Configurations;

public class MeterReadingFailureConfiguration : IEntityTypeConfiguration<MeterReadingFailure>
{
    public void Configure(EntityTypeBuilder<MeterReadingFailure> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(e => e.EquipmentId).IsRequired();
        builder.Property(e => e.RecordDate).IsRequired();
    }
}
