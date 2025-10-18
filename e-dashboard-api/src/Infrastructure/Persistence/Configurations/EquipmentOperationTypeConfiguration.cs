using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Infrastructure.Persistence.Configurations;

public class EquipmentOperationTypeConfiguration : IEntityTypeConfiguration<EquipmentOperationType>
{
    public void Configure(EntityTypeBuilder<EquipmentOperationType> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EquipmentType).IsRequired();
        builder.Property(e => e.OperationName).IsRequired().HasMaxLength(250);
    }
}