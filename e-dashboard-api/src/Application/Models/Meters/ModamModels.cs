using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnergyDashboard.Application.Models.Meters
{
    public class OnlineEnergyParam
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SerialNumber { get; set; }
    }

    public class OperationError
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class MeterInstantParameter : IEquatable<MeterInstantParameter>
    {
        public string SerialNumber { get; set; }
        public DateTime MeterTime { get; set; }
        public decimal? Power_Active_Export { get; set; }
        public decimal? Power_Reactive_Export { get; set; }
        public decimal? Power_Active_Import { get; set; }
        public decimal? Power_Reactive_Import { get; set; }
        public decimal? Voltage_A { get; set; }
        public decimal? Voltage_B { get; set; }
        public decimal? Voltage_C { get; set; }
        public decimal? Current_A { get; set; }
        public decimal? Current_B { get; set; }
        public decimal? Current_C { get; set; }
        public bool? EstimateStatus { get; set; }

        public bool Equals(MeterInstantParameter other)
        {

            //Check whether the compared object is null.
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (Object.ReferenceEquals(this, other)) return true;

            //Check whether the products' properties are equal.
            return SerialNumber.Equals(other.SerialNumber);
        }

        public override int GetHashCode()
        {
            //Get hash code for the SerialNumber field if it is not null.
            int hashSerialNumber = SerialNumber == null ? 0 : SerialNumber.GetHashCode();
            return hashSerialNumber;
        }
    }

    public class ModamResponse
    {
        public IList<OperationError> Errors { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public IList<MeterInstantParameter> Result { get; set; }
    }

    public class UnitInstantPower
    {
        public string Name { get; set; }
        public double Capacity { get; set; }
        public double ActivePower { get; set; }
    }

    public class PowerPlantInstantPower
    {
        public PowerPlantInstantPower()
        {
            UnitInstantPowers = new List<UnitInstantPower>();
        }
        public int PowerPlantId { get; set; }
        public string PowerPlantName { get; set; }
        public List<UnitInstantPower> UnitInstantPowers { get; set; }
    }

    public class TotalInstantPower
    {
        public double Generation { get; set; }
        public double Distribution { get; set; }
        public double Industrial { get; set; }
        public double Tabadol { get; set; }
    }

}
