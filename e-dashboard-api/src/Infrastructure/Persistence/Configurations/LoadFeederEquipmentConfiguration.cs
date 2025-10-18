using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Infrastructure.Persistence.Configurations;

public class LoadFeederEquipmentConfiguration : IEntityTypeConfiguration<LoadFeederEquipment>
{
    public void Configure(EntityTypeBuilder<LoadFeederEquipment> builder)
    {
        builder.ToTable("load_feeders");
        builder.Property(e => e.LoadFeederTypeId).IsRequired();

        builder.Navigation(e => e.LoadFeederType).AutoInclude();
    }
}
