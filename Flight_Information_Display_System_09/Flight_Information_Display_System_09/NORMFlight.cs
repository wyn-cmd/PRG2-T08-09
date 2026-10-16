using System;

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
            const double baseFee = 300.0;
            const double singaporeDestinationFee = 500.0;
            const double singaporeOriginFee = 800.0;

            if (Destination == "Singapore (SIN)")
            {
                return baseFee + singaporeDestinationFee;
            }
            
            if (Origin == "Singapore (SIN)")
            {
                return baseFee + singaporeOriginFee;
            }

            return baseFee;
        }

        public override string ToString()
        {
            return $"{FlightNumber}: {Origin} to {Destination}, {ExpectedTime}, Status: {Status}";
        }
    }
}