using System;
using System.Collections.Generic;
using System.Text;

namespace EnergyDashboard.Application.Models.Meters
{
    public class MeterEnergyList
    {
        public int MeterId { get; set; }
        public string SerialNumber { get; set; }
        public string Name { get; set; }
        public int StationId { get; set; }
        public string StationName { get; set; }
        public bool Anomal { get; set; }
        public List<DailyEnergyEntity> DailyEnergy { get; set; }
    }

    public class DailyEnergyEntity
    {
        public int Hour { get; set; }
        public decimal ActiveEnergy { get; set; }
    }

    public class SubstationMeterEnergyList
    {
        public int StationId { get; set; }
        public string StationName { get; set; }
        public List<EquipmentsList> Equipments { get; set; }
    }

    public class EquipmentsList
    {
        public int MeterId { get; set; }
        public string SerialNumber { get; set; }
        public string Name { get; set; }
        public bool Anomal { get; set; }
        public List<DailyEnergyEntity> DailyEnergy { get; set; }
    }
}
