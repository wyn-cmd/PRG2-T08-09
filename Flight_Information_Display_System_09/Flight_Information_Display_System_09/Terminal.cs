using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S10265740_PRG2Assignment;

//==========================================================
// Student Number : S10265740
// Student Name : Ravin Nagpal
// Partner Name : Wynston Wong
//==========================================================

namespace Flight_Information_Display_System_09
{
    public class Terminal
    {
        //-terminalName: string
        //-airlines: Dictionary<string, Airline>
        //-flights: Dictionary<string, Flight>
        //-boardingGates: Dictionary<string, BoardingGate>
        //-gateFees: Dictionary<string, double>

        public string TerminalName { get; set; }
        public Dictionary<string, Airline> Airlines { get; set; }
        public Dictionary<string, Flight> Flights { get; set; }
        public Dictionary<string, BoardingGate> BoardingGates { get; set; }
        public Dictionary<string, double> GateFees { get; set; }

        //+AddAirline(Airline): bool
        //+AddBoardingGate(BoardingGate): bool
        //+GetAirlineFromFlight(Flight) : Airline
        //+PrintAirlineFees()
        //+ToString() : string

        public Terminal(string terminalName)
        {
            TerminalName = terminalName;
            Airlines = new Dictionary<string, Airline>();
            Flights = new Dictionary<string, Flight>();
            BoardingGates = new Dictionary<string, BoardingGate>();
            GateFees = new Dictionary<string, double>();
        }

        public bool AddAirline(Airline airline)
        {
            if (!Airlines.ContainsKey(airline.Name))
            {
                Airlines.Add(airline.Name, airline);
                return true;
            }
            return false;
        }

        public bool AddBoardingGate(BoardingGate boardingGate)
        {
            if (!BoardingGates.ContainsKey(boardingGate.GateName))
            {
                BoardingGates.Add(boardingGate.GateName, boardingGate);
                return true;
            }
            return false;
        }

        public Airline GetAirlineFromFlight(Flight flight)
        {
            foreach (var airline in Airlines.Values)
            {
                if (airline.Flights.ContainsKey(flight.FlightNumber))
                {
                    return airline;
                }
            }
            return null;
        }

        public void PrintAirlineFees()
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("Airline Fees for Changi Airport Terminal 5");
            Console.WriteLine("=============================================");
            Console.WriteLine("Airline Name         Total Fees\n");

            foreach (var airline in Airlines.Values)
            {
                Console.WriteLine(string.Format("{0,-20} {1:C}", airline.Name, airline.CalculateFees()));
            }
        }

        public override string ToString()
        {
            return $"Terminal: {TerminalName}, Airlines Count: {Airlines.Count}, Flights Count: {Flights.Count}, " +
                   $"Boarding Gates Count: {BoardingGates.Count}";
        }

    }
}
