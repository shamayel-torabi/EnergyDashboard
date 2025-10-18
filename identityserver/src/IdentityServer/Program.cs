using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using IdentityServer.Models;
using IdentityServer.Services;
using IdentityServer.Data;
using EnergyDashboard.Common.Data;
using EnergyDashboard.Common.Logging;
using Duende.IdentityServer.Services;
using HealthCheck;
using Serilog;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var seed = args.Contains("/seed");
if (seed)
{
    args = args.Except(new[] { "/seed" }).ToArray();
}

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
string identityServerUrl = builder.Configuration.GetValue<string>("ExternalIdentityServerUrl");

builder.Host.UseSerilog(SerilogConfig.LogConfigure);

builder.Services.AddHealthChecks()
    .AddCheck("سرویس احراز هویت کاربران", () => HealthCheckResult.Healthy("سرویس احراز هویت کاربران آماده خدمات رسانی است"), new string[] { "self" })
    .AddMemoryHealthCheck("بررسی حافظه", HealthStatus.Unhealthy, 1L * 1024L * 1024L * 1024L, new string[] { "self" })
    .AddDbContextCheck<IdentityDbContext>("بررسی اتصال به بانک اطلاعاتی", HealthStatus.Unhealthy, new string[] { "service" });

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));


builder.Services.AddDbContext<IdentityDbContext>(options =>
{
    options.UseNpgsql(connectionString, sqlOptions =>
    {
        sqlOptions.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName);
        sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
    })
    .UseSnakeCaseNamingConvention();
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.RequireHeaderSymmetry = false;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddDataProtection()
        .PersistKeysToDbContext<IdentityDbContext>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddErrorDescriber<FarsiIdentityErrorDescriber>()
    .AddDefaultTokenProviders();
builder.Services.AddIdentityServer(options =>
{
    options.IssuerUri = identityServerUrl;    
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = false;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = false;
}).AddApiAuthorization<ApplicationUser, IdentityDbContext>();

builder.Services.AddControllers();
builder.Services.AddRazorPages();

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IProfileService, IdentityClaimsProfileService>();


builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
}));

var app = builder.Build();
app.UseForwardedHeaders();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
}


app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

app.UseStaticFiles();
app.UseRouting();
app.UseCors("MyPolicy");

app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/liveness", new HealthCheckOptions  { Predicate = r => r.Tags.Contains("self") });
app.MapHealthChecks("/healthz", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteResponse
});

app.MapRazorPages();

var cultureInfo = new CultureInfo("fa-IR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;


try
{
    if (seed)
    {
        app.MigrateDbContext<IdentityDbContext>((context, services) =>
        {
            new IdentityDbContextSeed().SeedAsync(context, services).Wait();
        });

        return 0;
    }

    app.Logger.LogInformation($"Starting IdentityServer at {DateTime.Now}...");
    app.Run();

    return 0;
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Host terminated unexpectedly");
    Console.WriteLine(ex.Message);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

