using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Infrastructure.Persistence.Configurations;

public class EquipmentOperationConfiguration : IEntityTypeConfiguration<EquipmentOperation>
{
    public void Configure(EntityTypeBuilder<EquipmentOperation> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EquipmentId).IsRequired();
        builder.Property(e => e.DateOfOpereation).IsRequired();
        builder.Property(e => e.Description).IsRequired().HasMaxLength(250);
    }
}