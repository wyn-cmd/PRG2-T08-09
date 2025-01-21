using System;
using System.Collections.Generic;
using System.IO;
using S10265740_PRG2Assignment;

class Program
{
    static void Main(string[] args)
    {
        //Dictionary to store Airlines
        Dictionary<string, Airline> airlines = new Dictionary<string, Airline>();
        string filePath = "airlines.csv";

        try
        {
            Console.WriteLine("Loading Airlines...");
            //load the airlines.csv file
            var lines = File.ReadAllLines(filePath);
            //Skip the header
            for (int i = 1; i < lines.Length; i++)
            {
                //Split each line by comma
                var columns = lines[i].Split(',');
                //add the Airlines objects into an Airline Dictionary
                var airline = new Airline(columns[0].Trim(), columns[1].Trim());

                //Use the airline's name as the key
                if (!airlines.ContainsKey(airline.Name))
                {
                    airlines.Add(airline.Name, airline);
                }
            }

            Console.WriteLine($"{airlines.Count} Airlines Loaded!");
        }
        catch (Exception ex)
        {
            //Exception Error
            Console.WriteLine("An error occurred: " + ex.Message);
        }



        // Dictionary to store Flight objects
        Dictionary<string, Flight> flights = new Dictionary<string, Flight>();

        // File path to flights.csv
        filePath = "flights.csv";

        // Load flights from the CSV file
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Error: File not found at {filePath}");
            return;
        }

        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;

                // Skip the header line
                reader.ReadLine();

                while ((line = reader.ReadLine()) != null)
                {
                    // Parse each line into flight data
                    string[] columns = line.Split(',');



                    string flightNumber = columns[0].Trim();
                    string origin = columns[1].Trim();
                    string destination = columns[2].Trim();
                    string expectedTime = columns[3].Trim();
                    string specialRequestCode = columns.Length > 4 ? columns[4].Trim() : null;


                    // Determine the type of Flight based on the special request code
                    Flight flight;
                    if (specialRequestCode == "DDJB")
                    {
                        flight = new DDJBFlight(flightNumber, origin, destination, DateTime.Parse(expectedTime));
                    }
                    else if (specialRequestCode == "CFFT")
                    {
                        flight = new CFFTFlight(flightNumber, origin, destination, DateTime.Parse(expectedTime));
                    }
                    else if (specialRequestCode == "LWTT")
                    {
                        flight = new LWTTFlight(flightNumber, origin, destination, DateTime.Parse(expectedTime));
                    }
                    else
                    {
                        flight = new NORMFlight(flightNumber, origin, destination, DateTime.Parse(expectedTime), "On Time");
                    }

                    // Add the Flight object to the dictionary
                    if (!flights.ContainsKey(flightNumber))
                    {
                        flights.Add(flightNumber, flight);
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Duplicate flight number {flightNumber} skipped.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading flights: {ex.Message}");
        }


        // List flight details
        //ListAllFlights(flights);


        // Assign Boarding Gates
        // Also need to initalise boarding gates
        //AssignBoardingGate(flights, boardingGates);


    }


    static void ListAllFlights(Dictionary<string, Flight> flights)
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("List of Flights for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine("Flight Number      Airline Name         Origin                 Destination            Expected Departure/Arrival Time\n");

        foreach (var flight in flights.Values)
        {
            string airlineName = GetAirlineName(flight.FlightNumber); // Placeholder for airline name lookup
            Console.WriteLine(string.Format("{0,-18} {1,-20} {2,-22} {3,-22} {4:dd/M/yyyy hh:mm:ss tt}",
                flight.FlightNumber, airlineName, flight.Origin, flight.Destination, flight.ExpectedTime));
        }
    }


    static void AssignBoardingGate(Dictionary<string, Flight> flights, Dictionary<string, string> boardingGates)
    {
        Console.Write("Enter Flight Number: ");
        string flightNumber = Console.ReadLine();

        if (!flights.ContainsKey(flightNumber))
        {
            Console.WriteLine("Error: Flight not found.");
            return;
        }

        Flight selectedFlight = flights[flightNumber];
        Console.WriteLine("Selected Flight Information:");
        Console.WriteLine(selectedFlight.ToString());

        string boardingGate;
        while (true)
        {
            Console.Write("Enter Boarding Gate: ");
            boardingGate = Console.ReadLine();

            if (boardingGates.ContainsKey(boardingGate))
            {
                Console.WriteLine($"Error: Boarding Gate {boardingGate} is already assigned to Flight {boardingGates[boardingGate]}.");
            }
            else
            {
                boardingGates[boardingGate] = flightNumber;
                break;
            }
        }

        Console.WriteLine($"Flight {selectedFlight.FlightNumber} assigned to Boarding Gate {boardingGate}.");

        Console.Write("Would you like to update the Status of the Flight? (Y/N): ");
        string updateStatus = Console.ReadLine().Trim().ToUpper();

        if (updateStatus == "Y")
        {
            Console.Write("Enter new Status (Delayed, Boarding, On Time): ");
            selectedFlight.Status = Console.ReadLine();
        }
        else
        {
            selectedFlight.Status = "On Time";
        }

        Console.WriteLine("Boarding Gate assignment successful!");
        Console.WriteLine(selectedFlight.ToString());
    }


    static string GetAirlineName(string flightNumber)
    {
        // Implement a proper lookup based on airline data
        return "(TBD)";
    }
}
