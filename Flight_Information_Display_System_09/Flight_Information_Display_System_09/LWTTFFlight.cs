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
    public class LWTTFlight : Flight
    {
        public LWTTFlight(string flightNumber, string origin, string destination, DateTime expectedTime)
            : base(flightNumber, origin, destination, expectedTime, "On Time", "LWTT")
        {
            FlightNumber = flightNumber;
            Origin = origin;
            Destination = destination;
            ExpectedTime = expectedTime;
            Status = "On Time";
        }

        public override double CalculateFees()
        {
            double baseFee = 300.0;
            double locationFee = (Destination == "Singapore (SIN)") ? 500.0 : (Origin == "Singapore (SIN)") ? 800.0 : 0.0;
            double specialRequestFee = 500.0; // LWTT Fee
            return baseFee + locationFee + specialRequestFee;
        }

    }

}
