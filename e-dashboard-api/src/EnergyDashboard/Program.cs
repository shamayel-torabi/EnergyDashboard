using System.Globalization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using EnergyDashboard.Application;
using EnergyDashboard.Infrastructure;
using EnergyDashboard.Infrastructure.Persistence;
using EnergyDashboard.Common.Data;
using EnergyDashboard.Common.Logging;
using HealthCheck;
using Serilog;
using Microsoft.OpenApi.Models;
using EnergyDashboard.Middleware;
using EnergyDashboard.Endpoints;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var seed = args.Contains("/seed");
if (seed)
{
    args = args.Except(new[] { "/seed" }).ToArray();
}

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SerilogConfig.LogConfigure);

string identityServerUrl = builder.Configuration.GetValue<string>("IdentityServerUrl");

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(o => o.AddPolicy("ApiCorsPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));


builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EnergyDashboard HTTP API",
        Version = "v1",
        Description = "The EnergyDashboard Service HTTP API"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAuthentication("Bearer")
.AddJwtBearer("Bearer", options =>
{
    options.Authority = identityServerUrl;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "EnergyDashboardAPI");
    });
});

builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

app.UseExceptionHandler(ExceptionHandler.Handle);
app.UseStatusCodePages(async statusCodeContext
    => await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
                 .ExecuteAsync(statusCodeContext.HttpContext));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EnergyDashboard.API v1"));
}

app.UseRouting();
app.UseCors("ApiCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapPowerplantTypeEndpoints();
app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/liveness", new HealthCheckOptions  { Predicate = r => r.Tags.Contains("self") });
app.MapHealthChecks("/healthz", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteResponse
});
app.MapGet("/", () => "Healthy");

var cultureInfo = new CultureInfo("fa-IR");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;


try
{
    if (seed)
    {
        app.Logger.LogInformation("Starting Seeding ...");

        app.MigrateDbContext<AppDbContext>((context, services) =>
        {
            new AppDbContextSeed().SeedAsync(context, services).Wait();
        });

        app.Logger.LogInformation($"End Seeding ...");

        return 0;
    }

    app.Logger.LogInformation($"Starting EnergyDashboard API at {DateTime.Now} ...");
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


