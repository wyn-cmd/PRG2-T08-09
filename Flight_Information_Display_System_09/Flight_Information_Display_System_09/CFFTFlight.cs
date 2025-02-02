using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//==========================================================
// Student Number : S10266219
// Student Name : Wynston Wong
// Partner Name : Ravin Nagpal
//==========================================================

namespace S10265740_PRG2Assignment
{
    public class CFFTFlight : Flight
    {
        public CFFTFlight(string flightNumber, string origin, string destination, DateTime expectedTime)
            : base(flightNumber, origin, destination, expectedTime, "On Time", "CFFT")
        {
            FlightNumber = flightNumber;
            Origin = origin;
            Destination = destination;
            ExpectedTime = expectedTime;
            Status = "On Time";
        }

        public override double CalculateFees()
        {
            // Calculation function to be added
            return 0; 
        }
    }
}
