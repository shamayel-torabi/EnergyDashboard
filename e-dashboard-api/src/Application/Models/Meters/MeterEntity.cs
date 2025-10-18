using System;
using System.Collections.Generic;
using System.Text;

namespace EnergyDashboard.Application.Models.Meters
{
    public class MeterEntity
    {
        public int meterId { get; set; }
        public string serialNumber { get; set; }
        public string name { get; set; }
        public bool active { get; set; }
        public DateTimeOffset startOperationDate { get; set; }
        public DateTimeOffset endOperationDate { get; set; }
    }
}
