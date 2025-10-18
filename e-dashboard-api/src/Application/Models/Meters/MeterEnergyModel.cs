using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EnergyDashboard.Application.Models.Meters
{
    public class MeterEnergyModel
    {
        [Display(Name = "شناسه میتر")]
        public int MeterId { get; set; }

        [Display(Name = "نوع تجهیز")]
        public int ToolTypeId { get; set; }

        [Display(Name = "تاریخ")]
        public DateTimeOffset RecordDate { get; set; }

        [Display(Name = "انرژی خروجی اکتیو")]
        [DisplayFormat(DataFormatString = "{0:0.000000}")]
        public decimal EnergyActiveExport {get; set; }

        [Display(Name = "انرژی ورودی اکتیو")]
        [DisplayFormat(DataFormatString = "{0:0.000000}")]
        public decimal EnergyActiveImport { get; set; }

        [Display(Name = "انرژی خروجی راکتیو")]
        [DisplayFormat(DataFormatString = "{0:0.000000}")]
        public decimal EnergyReactiveExport { get; set; }

        [Display(Name = "انرژی ورودی راکتیو")]
        [DisplayFormat(DataFormatString = "{0:0.000000}")]
        public decimal EnergyReactiveImport { get; set; }
    }
}
