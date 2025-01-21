using S10265740_PRG2Assignment;
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

namespace Flight_Information_Display_System_09
{
    public class BoardingGate
    {
        public string GateName { get; set; }
        public bool SupportsCFFT { get; set; }
        public bool SupportsDDJB { get; set; }
        public bool SupportsLWTT { get; set; }
        public Flight AssignedFlight { get; set; }

        public BoardingGate(string gateName, bool supportsCFFT, bool supportsDDJB, bool supportsLWTT)
        {
            GateName = gateName;
            SupportsCFFT = supportsCFFT;
            SupportsDDJB = supportsDDJB;
            SupportsLWTT = supportsLWTT;
            AssignedFlight = null;
        }

        public double CalculateFees()
        {
            // Calculation function to be added
            return 0;
        }

        public override string ToString()
        {
            return $"GateName: {GateName}, SupportsCFFT: {SupportsCFFT}, SupportsDDJB: {SupportsDDJB}, SupportsLWTT: {SupportsLWTT}, " +
                   $"AssignedFlight: {(AssignedFlight != null ? AssignedFlight.FlightNumber : "None")}";
        }
    }
}
