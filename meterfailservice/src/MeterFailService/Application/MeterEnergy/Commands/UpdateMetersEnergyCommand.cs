using Application.Common.Core;
using Domain.Common.Result;
using MeterFailService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MeterFailService.Application.MeterEnergy.Commands;

public sealed record UpdateMetersEnergyCommand : ICommand<Result>
{
    public Guid Id { get; set; }
    public bool Anomaly { get; set; }
}

public sealed class UpdateMetersEnergyCommanddHandler : ICommandHandler<UpdateMetersEnergyCommand, Result>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMemoryCache _cache;

    public UpdateMetersEnergyCommanddHandler(IApplicationDbContext dbContext, IMemoryCache cache)
    {
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(cache, nameof(cache));

        _dbContext = dbContext;
        _cache = cache;

    }

    public async Task<Result> Handle(UpdateMetersEnergyCommand request, CancellationToken cancellationToken)
    {
        var metersEnergy = await _dbContext.MeterEnergys.FirstOrDefaultAsync(f => f.Id == request.Id);

        if (metersEnergy is not null)
        {
            metersEnergy.Anomaly = request.Anomaly;
            _dbContext.MeterEnergys.Update(metersEnergy);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
