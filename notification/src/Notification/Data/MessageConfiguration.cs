using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notification.Data;

namespace MeterService.Infrastructure.Persistence.Configurations;

public sealed class MeterConfiguration : IEntityTypeConfiguration<MessageEntity>
{
    public void Configure(EntityTypeBuilder<MessageEntity> builder)
    {
        builder.HasKey(o => o.Id);
        builder.HasIndex(p => new { p.Date});
    }
}