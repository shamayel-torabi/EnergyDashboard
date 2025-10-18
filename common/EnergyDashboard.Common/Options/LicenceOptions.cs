using System;
using System.Collections.Generic;
using System.Text;

namespace EnergyDashboard.Common.Options
{
    public class LicenceOptions
    {
        public string Owner { get; set; }
        public string Licence { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
