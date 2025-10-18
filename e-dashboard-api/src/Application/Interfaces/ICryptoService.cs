using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnergyDashboard.Application.Interfaces;

public interface ICryptoService
{
    Guid GetDeterministicGuid(string input);
}
