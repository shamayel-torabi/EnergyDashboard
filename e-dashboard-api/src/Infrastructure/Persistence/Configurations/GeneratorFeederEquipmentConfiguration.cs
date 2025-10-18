using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Infrastructure.Persistence.Configurations;

public class GeneratorFeederEquipmentConfiguration : IEntityTypeConfiguration<GeneratorFeederEquipment>
{
    public void Configure(EntityTypeBuilder<GeneratorFeederEquipment> builder)
    {
        builder.ToTable("powerplant_feeders");
        builder.Property(e => e.PowerplantTypeId).IsRequired();
        builder.Property(e => e.PowerplantOperatorId).IsRequired();
        builder.Property(e => e.PowerplantScale).IsRequired();

        builder.Navigation(e => e.PowerplantOperator).AutoInclude();
        builder.Navigation(e => e.PowerplantType).AutoInclude();
    }
}
