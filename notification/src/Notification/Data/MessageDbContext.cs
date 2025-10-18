using Microsoft.EntityFrameworkCore;
using System.Reflection;

#nullable disable

namespace Notification.Data;

public sealed class MessageDbContext: DbContext
{
    public MessageDbContext(DbContextOptions<MessageDbContext> options) : base(options)
    {
    }

    public DbSet<MessageEntity> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}

