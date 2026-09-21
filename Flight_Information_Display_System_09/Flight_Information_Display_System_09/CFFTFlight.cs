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
        private const double BaseFee = 300.0;
        private const double ArrivalFee = 500.0;
        private const double DepartureFee = 800.0;
        private const double SpecialRequestFee = 150.0;
        private const string SingaporeHub = "Singapore (SIN)";

        public CFFTFlight(string flightNumber, string origin, string destination, DateTime expectedTime)
            : base(flightNumber, origin, destination, expectedTime, "On Time", "CFFT")
        {
        }

        public override double CalculateFees()
        {
            double locationFee = 0.0;

            if (Destination == SingaporeHub)
            {
                locationFee = ArrivalFee;
            }
            else if (Origin == SingaporeHub)
            {
                locationFee = DepartureFee;
            }

            return BaseFee + locationFee + SpecialRequestFee;
        }
    }
}