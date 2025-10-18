using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EnergyDashboard.Infrastructure.Persistence;

public class IdentityDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    AppDbContext IDesignTimeDbContextFactory<AppDbContext>.CreateDbContext(string[] args)
    {
        //var basePath = AppDomain.CurrentDomain.BaseDirectory;

        var basePath = Directory.GetCurrentDirectory() + string.Format("{0}..{0}EnergyDashboard", Path.DirectorySeparatorChar);

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = configuration.GetConnectionString("AppDbConnection");

        builder.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();

        return new AppDbContext(builder.Options);
    }
}
