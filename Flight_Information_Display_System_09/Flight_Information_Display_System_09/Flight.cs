using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//==========================================================
// Student Number : S10265740
// Student Name : Ravin Nagpal
// Partner Name : Wynston Wong
//==========================================================


namespace S10265740_PRG2Assignment
{
    public abstract class Flight : IComparable<Flight>
    {
        public string FlightNumber { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime ExpectedTime { get; set; }
        public string Status { get; set; }
        public string SpecialRequestCode { get; set; } = "None";
        public string BoardingGate { get; set; } = "Unassigned";

        public Flight(string flightNumber, string origin, string destination, DateTime expectedTime, string status, string specialRequestCode = "None", string boardingGate = "Unassigned")
        {
            FlightNumber = flightNumber;
            Origin = origin;
            Destination = destination;
            ExpectedTime = expectedTime;
            Status = status;
            SpecialRequestCode = specialRequestCode;
            BoardingGate = boardingGate;
        }

        public abstract double CalculateFees();

        // Compare flights based on ExpectedTime for sorting
        public int CompareTo(Flight other)
        {
            return this.ExpectedTime.CompareTo(other.ExpectedTime);
        }

        public override string ToString()
        {
            return $"{FlightNumber,-10} {Origin,-20} {Destination,-20} {ExpectedTime:dd/MM/yyyy hh:mm tt,-25} {Status,-10} {SpecialRequestCode,-15} {BoardingGate,-10}";
        }
    }
}
