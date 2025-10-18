using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Infrastructure.Persistence.Configurations;

public class NetworkConfiguration : IEntityTypeConfiguration<Network>
{
    public void Configure(EntityTypeBuilder<Network> builder)
    {
        builder.Ignore(e => e.Attributes);
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);

        builder.Property(e => e.Diagram).IsRequired().HasColumnType("jsonb");
        //builder.Property(e => e.Diagram).IsRequired().HasJsonConversion<DiagramModel>();

        builder.HasMany(e => e.Areas);
    }
}
