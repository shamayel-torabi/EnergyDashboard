using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EnergyDashboard.Application.Models.Meters
{
    public class Tool : IEquatable<Tool>
    {
        public int Id { get; set; }
        public int ToolId { get; set; }
        public string Name { get; set; }
        public int ToolTypeId { get; set; }

        public int StationId { get; set; }
        public string StationName { get; set; }
        public int StationTypeId { get; set; }
        public int MeterId { get; set; }
        public string SerialNumber { get; set; }
        public int OperatorId { get; set; }
        public string DispatchingCode { get; set; }
        public string SiemensCode { get; set; }
        public DateTime? FormulaDismountDate { get; set; }
        public DateTime FormulaMountDate { get; set; }
        public bool ActiveStatus { get; set; }
        public bool Alternative { get; set; }
        public double PrimaryVoltage { get; set; }
        public double SecondaryVoltage { get; set; }
        public double TernaryVoltage { get; set; }

        public double CT_Ratio { get; set; }
        public double CT_RatioSec { get; set; }

        public double PT_Ratio { get; set; }
        public double PT_RatioSec { get; set; }

        public int FormulaTypeId { get; set; }
        public string FormulaString { get; set; }

        public int LineTypeId { get; set; }

        public bool Equals(Tool other)
        {

            //Check whether the compared object is null. 
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data. 
            if (Object.ReferenceEquals(this, other)) return true;

            //Check whether the products' properties are equal. 
            return ToolId.Equals(other.ToolId);
        }

        // If Equals() returns true for a pair of objects  
        // then GetHashCode() must return the same value for these objects. 

        public override int GetHashCode()
        {

            //Get hash code for the Code field. 
            int hashToolId = ToolId.GetHashCode();


            //Calculate the hash code for the product. 
            return hashToolId;
        }

        public override string ToString()
        {
            //return $"{},{},{},{},{},{},{},{},{},{},{},{},{},{},{},{}";
            return $"{ToolId},{Name},{StationId},{StationName},{StationTypeId},{MeterId},{SerialNumber},{OperatorId},{DispatchingCode},{SiemensCode},{FormulaMountDate},{FormulaDismountDate},{ActiveStatus},{Alternative},{PrimaryVoltage}\n";
        }
    }
}
