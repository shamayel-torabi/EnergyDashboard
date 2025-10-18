using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Infrastructure.Persistence.Configurations;

public class SubstationConfiguration : IEntityTypeConfiguration<Substation>
{
    public void Configure(EntityTypeBuilder<Substation> builder)
    {
        builder.Ignore(e => e.Attributes);
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Type).IsRequired().HasMaxLength(50);

        builder.Property(e => e.Diagram).IsRequired().HasColumnType("jsonb");
        //builder.Property(e => e.Diagram).IsRequired().HasJsonConversion<DiagramModel>();

        builder.Property(e => e.IGMCCode).HasMaxLength(50);
        builder.Property(e => e.DispatchingCode).HasMaxLength(50);
        builder.Property(e => e.IGMCStationId).IsRequired().HasDefaultValue(0);

        builder.HasMany(x => x.Equipments);

        builder.Metadata.FindNavigation("Equipments").SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
