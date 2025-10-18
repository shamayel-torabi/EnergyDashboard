
namespace EnergyDashboard.Application.MeterEnergys
{
    public class MeterEnergyDto
    {
        public Guid MeterEnergyId { get; set; }
        public int MeterId { get; set; }
        public string SerialNumber { get; set; }
        public int? ToolTypeId { get; set; }
        public DateTimeOffset RecordDate { get; set; }
        public decimal EnergyActiveExport { get; set; }
        public decimal EnergyActiveImport { get; set; }
        public decimal EnergyReactiveExport { get; set; }
        public decimal EnergyReactiveImport { get; set; }
    }
}
