
using EnergyDashboard.Application.Interfaces;
using System;

namespace EnergyDashboard.Infrastructure.Services;

public class DateTimeService : IDateTimeService
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}
