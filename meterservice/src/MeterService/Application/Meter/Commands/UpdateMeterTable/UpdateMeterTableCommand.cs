using MediatR;
using MeterService.Application.Interfaces;

namespace MeterService.Application.Meters.Commands;

public sealed record UpdateMeterTableCommand : IRequest<bool>
{
}

public sealed class UpdateMeterTableCommandHandler : IRequestHandler<UpdateMeterTableCommand, bool>
{
    private readonly ISepacService _sepacService;

    public UpdateMeterTableCommandHandler(ISepacService sepacService)
    {
        _sepacService = sepacService;
    }

    public async Task<bool> Handle(UpdateMeterTableCommand request, CancellationToken cancellationToken)
    {
        return await _sepacService.UpdateMetersTableAsync(cancellationToken);
    }
}
