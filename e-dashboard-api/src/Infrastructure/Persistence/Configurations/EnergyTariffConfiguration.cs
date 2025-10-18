using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Infrastructure.Persistence.Configurations;

public class EnergyTariffConfiguration : IEntityTypeConfiguration<EnergyTariff>
{
    public void Configure(EntityTypeBuilder<EnergyTariff> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired();
        builder.Property(e => e.Name).IsRequired().HasMaxLength(50);
        builder.Property(e => e.StartDate).IsRequired();
        builder.Property(e => e.EndDate).IsRequired();

        builder.OwnsMany(e => e.EnergyTariffRates, em =>
        {
            em.HasKey(e => new { e.Id, e.EnergyTariffId });
            em.Property(e => e.StartTime).IsRequired();
            em.Property(e => e.EndTime).IsRequired();
            em.Property(e => e.Rate).IsRequired();
            em.WithOwner().HasForeignKey(e => e.EnergyTariffId);
        });
    }
}
