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
    // Represents a normal flight with standard fee structures.
    public class NORMFlight : Flight
    {
        public NORMFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status = "On Time", string specialRequestCode = "None", string boardingGate = "Unassigned")
            : base(flightNumber, origin, destination, expectedTime, status, specialRequestCode, boardingGate)
        {
        }

        public override double CalculateFees()
        {
            double boardingGateBaseFee = 300.0;
            double additionalFee = 0.0;

            if (Destination == "Singapore (SIN)")
            {
                additionalFee = 500.0;
            }
            else if (Origin == "Singapore (SIN)")
            {
                additionalFee = 800.0;
            }

            return boardingGateBaseFee + additionalFee;
        }

        public override string ToString()
        {
            return $"{FlightNumber}: {Origin} to {Destination}, {ExpectedTime}, Status: {Status}";
        }
    }
}