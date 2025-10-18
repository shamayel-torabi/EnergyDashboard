using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Application.Interfaces;
using EnergyDashboard.Domain.Diagram;

namespace EnergyDashboard.Infrastructure.Persistence;

public class AppDbContextSeed
{
    public async Task SeedAsync(AppDbContext context, IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILogger<AppDbContextSeed>>();
        var cryptoService = services.GetRequiredService<ICryptoService>();

        try
        {
            using (context)
            {
                await context.Database.MigrateAsync();
                await SeedNetworkData(context, logger, cryptoService);
                await SeedMetersData(context, logger);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
        }
    }

    private static async Task SeedNetworkData(AppDbContext context, ILogger<AppDbContextSeed> logger, ICryptoService crypto)
    {
        if (!context.Networks.Any())
        {
            logger.LogInformation("Networks being populated");

            string name = "شبکه ایران";
            var network = new Network(crypto.GetDeterministicGuid(name));

            var diagram = new DiagramModel(network.Id.ToString(), "", "network");
            diagram.Properties = new DiagramModelProperties();
            diagram.Properties.Title = name;
            diagram.Properties.Description = name;

            network.Update(diagram);
            await context.Networks.AddAsync(network);
            await context.SaveChangesAsync();
        }
        else
        {
            logger.LogInformation("Networks already populated");
        }

    }

    private static async Task SeedMetersData(AppDbContext context, ILogger<AppDbContextSeed> logger)
    {
        string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string metersFile = Path.Combine(currentDirectory, "NetworkConfig.json");

        string jsonString = File.ReadAllText(metersFile);
        SeedConfig config = System.Text.Json.JsonSerializer.Deserialize<SeedConfig>(jsonString);

        if (!context.LoadFeederTypes.Any())
        {
            try
            {
                await context.LoadFeederTypes.AddRangeAsync(config.LoadFeederTypes);
                //context.Database.OpenConnection();
                //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT \"LoadFeederTypes\" ON");
                await context.SaveChangesAsync();
                //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT \"LoadFeederTypes\" OFF");
                logger.LogInformation("LoadFeederTypes being populated");
            }
            catch (DbUpdateException ex)
            {
                logger.LogInformation($"Error LoadFeederTypes populations:{ex.Message}");
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }
        else
        {
            logger.LogInformation("LoadFeederTypes already populated");
        }

        if (!context.PowerplantOperators.Any())
        {
            try
            {
                await context.PowerplantOperators.AddRangeAsync(config.PowerplantOperators);
                //context.Database.OpenConnection();
                //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT \"PowerplantOperators\" ON");
                await context.SaveChangesAsync();
                //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT \"PowerplantOperators\" OFF");
                logger.LogInformation("PowerplantOperators being populated");
            }
            catch (DbUpdateException ex)
            {
                logger.LogInformation($"Error PowerplantOperators populations:{ex.Message}");
            }
            finally
            {
                context.Database.CloseConnection();
            }
        }
        else
        {
            logger.LogInformation("PowerplantOperator already populated");
        }

        if (!context.PowerplantTypes.Any())
        {
            try
            {
                foreach(var ppt in config.PowerplantTypes)
                {
                    var pp = new PowerplantType() { Name = ppt.Name};
                    await context.PowerplantTypes.AddAsync(pp);
                }
                //await context.PowerplantTypes.AddRangeAsync(config.PowerplantTypes);
                //context.Database.OpenConnection();
                //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT \"PowerplantTypes\" ON");
                await context.SaveChangesAsync();
                //context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT \"PowerplantTypes\" OFF");
                logger.LogInformation("PowerplantTypes being populated");
            }
            catch (DbUpdateException ex)
            {
                logger.LogInformation($"Error PowerplantTypes populations:{ex.Message}");
            }
            //finally
            //{
            //    //context.Database.CloseConnection();
            //}
        }
        else
        {
            logger.LogInformation("PowerplantTypes already populated");
        }
    }
}
