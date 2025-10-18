using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EnergyDashboard.Application.Models.Meters
{
    public class EnergyProfileImport
    {
        public DateTime RecordDate { get; set; }

        public decimal ImportWatt { get; set; }

        public decimal ImportVar { get; set; }

        public decimal ExportWatt { get; set; }

        public decimal ExportVar { get; set; }

        public decimal ImportTotalWatt { get; set; }

        public decimal ExportTotalWatt { get; set; }
    }

    public class MeterImportVM
    {
        [StringLength(15)]
        public string SerialNo { get; set; }

        public EnergyProfileImport[] EnergyProfile { get; set; }
    }
}
