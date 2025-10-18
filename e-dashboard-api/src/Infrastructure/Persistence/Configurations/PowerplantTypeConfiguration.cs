using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Infrastructure.Persistence.Configurations;

public class PowerplantTypeConfiguration : IEntityTypeConfiguration<PowerplantType>
{
    public void Configure(EntityTypeBuilder<PowerplantType> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
    }
}
