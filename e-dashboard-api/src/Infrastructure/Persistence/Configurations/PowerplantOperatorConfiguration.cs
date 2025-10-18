using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Infrastructure.Persistence.Configurations;

public class PowerplantOperatorConfiguration : IEntityTypeConfiguration<PowerplantOperator>
{
    public void Configure(EntityTypeBuilder<PowerplantOperator> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(250);
    }
}
