using EnergyDashboard.Application.Interfaces;
using EnergyDashboard.Application.Models.Meters;
using MediatR;

namespace EnergyDashboard.Application.Utility.Queries
{
    public class GetSubstationMetersQuery : IRequest<IEnumerable<MeterEntity>>
    {
        public int SubstationId { get; set; }
    }

    public class GetSubstationMetersQueryHandler : IRequestHandler<GetSubstationMetersQuery, IEnumerable<MeterEntity>>
    {
        private readonly IMeterEnergyService _meterEnergyService;

        public GetSubstationMetersQueryHandler(IMeterEnergyService meterEnergyService)
        {
            _meterEnergyService = meterEnergyService;
        }

        public async Task<IEnumerable<MeterEntity>> Handle(GetSubstationMetersQuery request, CancellationToken cancellationToken)
        {
            var meters = await _meterEnergyService.GetMetersFromCash(cancellationToken);

            var ret = meters
                .Where(w => w.StationId == request.SubstationId && w.Dismount == false)
                .Select(s => new MeterEntity
                {
                    meterId = s.MeterId,
                    name = s.Name,
                    serialNumber = s.SerialNumber,
                    active = s.Dismount,
                    startOperationDate = s.StartOperationDate,
                    endOperationDate = s.EndOperationDate
                })
                .ToList();

            ret.Add(new MeterEntity
            {
                meterId = 0,
                name = "نامعلوم",
                serialNumber = "سریال",
                active = false,
                startOperationDate = DateTime.Now,
                endOperationDate = DateTime.Now
            });
            return ret.OrderBy(o => o.meterId);
        }
    }
}
