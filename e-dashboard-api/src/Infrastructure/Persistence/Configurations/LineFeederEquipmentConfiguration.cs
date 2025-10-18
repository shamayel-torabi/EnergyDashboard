using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Infrastructure.Persistence.Configurations;

public class LineFeederEquipmentConfiguration : IEntityTypeConfiguration<LineFeederEquipment>
{
    public void Configure(EntityTypeBuilder<LineFeederEquipment> builder)
    {
        builder.ToTable("line_feeders");
        builder.Property(e => e.TransmissionLineId).IsRequired();
        builder.Property(e => e.LineFeederTypeId).IsRequired();
    }
}
