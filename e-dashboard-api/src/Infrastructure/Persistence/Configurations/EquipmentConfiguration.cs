using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Infrastructure.Persistence.Configurations;

public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("equipment");

        builder.HasKey(equipment => equipment.Id);
        builder.Property(equipment => equipment.Name).IsRequired().HasMaxLength(100);
        builder.Property(equipment => equipment.Voltage).HasDefaultValue(0.0);
        builder.Property(equipment => equipment.Reverse).HasDefaultValue(false);
        builder.Property(equipment => equipment.Virtual).HasDefaultValue(false);
        builder.Property(equipment => equipment.BusbarName).HasMaxLength(50);
        builder.Property(equipment => equipment.AreaId).IsRequired();
        builder.Property(equipment => equipment.ZoneId).IsRequired();
        builder.Property(equipment => equipment.SubstationId).IsRequired();

        builder.OwnsOne(equipment => equipment.CTRatio, ctBuilder =>
        {
            ctBuilder.WithOwner();
            ctBuilder.Property(ct => ct.Primary).IsRequired().HasColumnName("CTRatio.Primary");
            ctBuilder.Property(ct => ct.Secondary).IsRequired().HasColumnName("CTRatio.Secondary");
        });

        builder.OwnsOne(equipment => equipment.PTRatio, ptBuilder =>
        {
            ptBuilder.WithOwner();
            ptBuilder.Property(pt => pt.Primary).IsRequired().HasColumnName("PTRatio.Primary");
            ptBuilder.Property(pt => pt.Secondary).IsRequired().HasColumnName("PTRatio.Secondary");
        });

        builder.OwnsMany(equipment => equipment.EquipmentMeters, emBuilder =>
        {
            emBuilder.HasKey(em => new { em.SerialNumber, em.EquipmentId });
            emBuilder.Property(em => em.EquipmentId).IsRequired();
            emBuilder.Property(em => em.SerialNumber).IsRequired().HasMaxLength(50);
            emBuilder.Property(em => em.MountDate).IsRequired();
            emBuilder.Property(em => em.DismountDate).IsRequired();
            emBuilder.Property(em => em.Active).HasDefaultValue(false);
            emBuilder.WithOwner().HasForeignKey(equipment => equipment.EquipmentId);
        });

        builder.HasMany(e => e.EnergyProfiles)
            .WithOne(energyProfile => energyProfile.Equipment)
            .HasForeignKey(energyProfile => energyProfile.EquipmentId);

        builder.HasMany(e => e.DailyEnergys).WithOne(a => a.Equipment).HasForeignKey(e => e.EquipmentId);
        builder.HasMany(e => e.EquipmentOperations);
        builder.HasMany(e => e.MeterReadingFailures);

        builder.Navigation(e => e.Substation).UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}