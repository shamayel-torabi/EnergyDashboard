using System;
using System.Collections.Generic;
using EnergyDashboard.Application.Common.Mappings;
using EnergyDashboard.Application.Substations;
using EnergyDashboard.Domain.Entities;

namespace EnergyDashboard.Application.Zones;

public class ZoneDto : IMapFrom<Zone>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<SubstationDto> Substations { get; set; }
}
