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
    public class NORMFlight : Flight
    {
        public NORMFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string status)
        {
            FlightNumber = flightNumber;
            Origin = origin;
            Destination = destination;
            ExpectedTime = expectedTime;
            Status = status;
        }

        public override double CalculateFees()
        {
            double boardingGateBaseFee = 300.0;
            double additionalFee = (Destination == "Singapore (SIN)") ? 500.0 : (Origin == "Singapore (SIN)") ? 800.0 : 0.0;
            return boardingGateBaseFee + additionalFee;
        }

        public override string ToString()
        {
            return $"{FlightNumber}: {Origin} to {Destination}, {ExpectedTime}, Status: {Status}";
        }
    }
}
