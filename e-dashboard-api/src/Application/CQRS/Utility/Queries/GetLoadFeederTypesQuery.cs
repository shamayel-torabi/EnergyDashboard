using MediatR;
using Microsoft.EntityFrameworkCore;
using EnergyDashboard.Domain.Enums;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.Utility.Queries;

public class GetLoadFeederTypesQuery : IRequest<IEnumerable<LoadFeederTypesDto>>
{
}

public class GetLoadFeederTypesQueryHandler : IRequestHandler<GetLoadFeederTypesQuery, IEnumerable<LoadFeederTypesDto>>
{
    private readonly ILoadFeederTypeRepository _loadFeederTypeRepository;

    public GetLoadFeederTypesQueryHandler(ILoadFeederTypeRepository loadFeederTypeRepository)
    {
        _loadFeederTypeRepository = loadFeederTypeRepository;
    }

    public async Task<IEnumerable<LoadFeederTypesDto>> Handle(GetLoadFeederTypesQuery request, CancellationToken cancellationToken)
    {
        var lfTypes = await _loadFeederTypeRepository.GetAllAsync(cancellationToken);

        var ret = lfTypes.GroupBy(g => g.Type)
            .Select(g => new LoadFeederTypesDto
            {
                Value = g.Key,
                Title = GetTitle(g.Key),
                Items = g
            }).ToList();


        return ret;
    }

    private string GetTitle(int type)
    {
        string r = string.Empty;
        switch (type)
        {
            case 1:
                r = "توزیع";
                break;
            case 2:
                r = "صنایع";
                break;
            case 3:
                r = "مصرف داخلی";
                break;
        }

        return r;
    }
}
