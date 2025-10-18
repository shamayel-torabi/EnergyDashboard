using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Infrastructure.Persistence.Configurations;

public class TransformerFeederEquipmentConfiguration : IEntityTypeConfiguration<TransformerFeederEquipment>
{
    public void Configure(EntityTypeBuilder<TransformerFeederEquipment> builder)
    {
        builder.ToTable("transformer_feeders");
        builder.Property(e => e.TransofmerType).IsRequired();
    }
}
