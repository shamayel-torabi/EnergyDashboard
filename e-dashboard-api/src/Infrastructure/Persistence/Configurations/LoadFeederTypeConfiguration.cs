using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Infrastructure.Persistence.Configurations;

public class LoadFeederTypeConfiguration : IEntityTypeConfiguration<LoadFeederType>
{
    public void Configure(EntityTypeBuilder<LoadFeederType> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
    }
}
