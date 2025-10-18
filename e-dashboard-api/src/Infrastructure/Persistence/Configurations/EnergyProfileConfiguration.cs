using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Infrastructure.Persistence.Configurations;

public class EnergyProfileConfiguration : IEntityTypeConfiguration<EnergyProfile>
{
    public void Configure(EntityTypeBuilder<EnergyProfile> builder)
    {
        builder.HasKey(o => o.Id);
        //builder.Property(o => o.EquipmentId).IsRequired();
        builder.Property(e => e.RecordDate).IsRequired();
        builder.Property(e => e.ImportWatt).IsRequired().HasPrecision(18, 0);
        builder.Property(e => e.ImportVar).IsRequired().HasPrecision(18, 0);
        builder.Property(e => e.ExportWatt).IsRequired().HasPrecision(18, 0);
        builder.Property(e => e.ExportVar).IsRequired().HasPrecision(18, 0);
        builder.Property(e => e.ImportTotalWatt).HasPrecision(18, 0);
        builder.Property(e => e.ExportTotalWatt).HasPrecision(18, 0);

        //builder.HasOne(e => e.Equipment).WithMany().HasForeignKey(e => e.EquipmentId);
    }
}