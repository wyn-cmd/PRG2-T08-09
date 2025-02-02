using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Flight_Information_Display_System_09;
using S10265740_PRG2Assignment;

class Program
{
    static void Main(string[] args)
    {
        Terminal terminal = new Terminal("Terminal 5");
        Dictionary<string, Airline> airlines = new Dictionary<string, Airline>();
        Dictionary<string, BoardingGate> boardingGates = new Dictionary<string, BoardingGate>();
        Dictionary<string, Flight> flights = new Dictionary<string, Flight>();
        Dictionary<string, Airline> airlinesByCode = new Dictionary<string, Airline>();

        string filePath = "airlines.csv";

        try
        {
            Console.WriteLine("Loading Airlines...");
            //load the airlines.csv file
            var lines = File.ReadAllLines(filePath);
            //Skip the header

            // Skip the header
            for (int i = 1; i < lines.Length; i++)
            {
                //Split each line by comma
                var columns = lines[i].Split(',');
                if (columns.Length >= 2)
                {
                    string code = columns[1].Trim().ToUpper();
                    string name = columns[0].Trim();

                    var airline = new Airline(code, name);

                    // Add to both dictionaries
                    if (!airlines.ContainsKey(name))
                    {
                        airlines.Add(name, airline);
                    }

                    if (!airlinesByCode.ContainsKey(code))
                    {
                        airlinesByCode.Add(code, airline);
                    }
                }
            }

            Console.WriteLine($"{airlines.Count} Airlines Loaded!");

        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while loading airlines: " + ex.Message);
            return; // Exit if we can't load airlines
        }

        //Load the boardinggates.csv file
        filePath = "boardinggates.csv";

        try
        {
            Console.WriteLine("Loading Boarding Gates...");
            var lines = File.ReadAllLines(filePath);

            // Skip the header and handle empty lines
            for (int i = 1; i < lines.Length; i++)
            {
                // Skip empty lines
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                // Split each line by comma
                var columns = lines[i].Split(',');

                // Check if the line has the expected number of columns
                if (columns.Length != 4)
                {
                    Console.WriteLine($"Skipping invalid line: {lines[i]}");
                    continue;
                }

                // Create BoardingGate objects
                string gateName = columns[0].Trim();
                bool supportsCFFT = columns[1].Trim().ToLower() == "true";
                bool supportsDDJB = columns[2].Trim().ToLower() == "true";
                bool supportsLWTT = columns[3].Trim().ToLower() == "true";

                var gate = new BoardingGate(gateName, supportsCFFT, supportsDDJB, supportsLWTT);

                // Add the BoardingGate objects into the BoardingGate Dictionary
                if (!boardingGates.ContainsKey(gate.GateName))
                {
                    boardingGates.Add(gate.GateName, gate);
                }
            }

            Console.WriteLine($"{boardingGates.Count} Boarding Gates Loaded!");
        }
        catch (Exception ex)
        {
            // Exception Error
            Console.WriteLine("An error occurred: " + ex.Message);
            // Log stack trace for detailed debugging
            Console.WriteLine(ex.StackTrace);
        }

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
            Console.WriteLine("Loading Flights...");
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
            Console.WriteLine($"{flights.Count} Flights Loaded!");
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

        Console.WriteLine();
        ShowMenu(terminal, boardingGates, flights, airlines, airlinesByCode);
    }


    static void CreateNewFlight(Dictionary<string, Flight> flights, string filePath)
    {
        while (true)
        {
            Console.WriteLine("\nCreating a New Flight...");

            // Prompt for flight details
            Console.Write("Enter Flight Number: ");
            string flightNumber = Console.ReadLine().Trim().ToUpper();

            // Ensure flight number does not already exist
            if (flights.ContainsKey(flightNumber))
            {
                Console.WriteLine("Error: A flight with this number already exists.");
                continue;
            }

            Console.Write("Enter Origin: ");
            string origin = Console.ReadLine().Trim();

            Console.Write("Enter Destination: ");
            string destination = Console.ReadLine().Trim();

            Console.Write("Enter Expected Departure/Arrival Time (dd/MM/yyyy HH:mm): ");
            DateTime expectedTime;
            while (!DateTime.TryParse(Console.ReadLine(), out expectedTime))
            {
                Console.Write("Invalid date format. Please enter again (dd/MM/yyyy HH:mm): ");
            }

            // Prompt for Special Request Code
            Console.Write("Would you like to add a Special Request Code? (Y/N): ");
            string addSpecialRequest = Console.ReadLine().Trim().ToUpper();
            string specialRequestCode = "";

            if (addSpecialRequest == "Y")
            {
                Console.Write("Enter Special Request Code (DDJB, CFFT, LWTT): ");
                specialRequestCode = Console.ReadLine().Trim().ToUpper();
            }

            // Determine flight type
            Flight newFlight;
            switch (specialRequestCode)
            {
                case "DDJB":
                    newFlight = new DDJBFlight(flightNumber, origin, destination, expectedTime);
                    break;
                case "CFFT":
                    newFlight = new CFFTFlight(flightNumber, origin, destination, expectedTime);
                    break;
                case "LWTT":
                    newFlight = new LWTTFlight(flightNumber, origin, destination, expectedTime);
                    break;
                default:
                    newFlight = new NORMFlight(flightNumber, origin, destination, expectedTime, "On Time");
                    break;
            }

            // Add flight to dictionary
            flights.Add(flightNumber, newFlight);

            // Append flight details to CSV file
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine($"{flightNumber},{origin},{destination},{expectedTime:dd/MM/yyyy HH:mm},{specialRequestCode}");
            }

            Console.WriteLine($"Flight {flightNumber} successfully added!");

            // Ask user if they want to add another flight
            Console.Write("Would you like to add another flight? (Y/N): ");
            string response = Console.ReadLine().Trim().ToUpper();
            if (response != "Y")
            {
                Console.WriteLine("Returning to the main menu...");
                break;
            }
        }
    }





    static object ShowMenu(Terminal terminal, Dictionary<string, BoardingGate> boardingGates, Dictionary<string, Flight> flights, Dictionary<string, Airline> airlines, Dictionary<string, Airline> airlinesByCode)
    {
        while (true)
        {
            Console.WriteLine("=============================================");
            Console.WriteLine("Welcome to Changi Airport Terminal 5");
            Console.WriteLine("=============================================");
            Console.WriteLine("1. List All Flights");
            Console.WriteLine("2. List Boarding Gates");
            Console.WriteLine("3. Assign a Boarding Gate to a Flight");
            Console.WriteLine("4. Create Flight");
            Console.WriteLine("5. Display Airline Flights");
            Console.WriteLine("6. Modify Flight Details");
            Console.WriteLine("7. Display Flight Schedule");
            Console.WriteLine("8. Bulk Assign Boarding Gates");
            Console.WriteLine("0. Exit");
            Console.Write("Please select your option: ");
            string option = Console.ReadLine(); 

            switch (option)
            {
                case "1":
                    ListAllFlights(flights, airlines);
                    break;
                case "2":
                    ListBoardingGates(boardingGates);
                    break;
                case "3":
                    AssignBoardingGate(flights, boardingGates);
                    break;
                case "4":
                    CreateNewFlight(flights, "flights.csv");
                    break;
                case "5":
                    DisplayAirlineFlights(airlines, airlinesByCode, flights);
                    break;
                case "6":
                    ModifyFlightDetails(flights, airlinesByCode);
                    break;
                case "7":
                    DisplayScheduledFlights(flights);
                    break;
                case "8":
                    ProcessUnassignedFlights(flights, boardingGates);
                    break;
                case "0":
                    Console.WriteLine("Exiting...");
                    return null;
                default:
                    Console.WriteLine("Invalid option, please try again.");
                    break;
            }
        }
    }



    static void ProcessUnassignedFlights(Dictionary<string, Flight> flights, Dictionary<string, BoardingGate> boardingGates)
    {
        Queue<Flight> unassignedFlights = new Queue<Flight>();

        // Identify flights with no assigned gate
        foreach (var flight in flights.Values)
        {
            if (string.IsNullOrEmpty(flight.BoardingGate) || flight.BoardingGate == "Unassigned")
            {
                unassignedFlights.Enqueue(flight);
            }
        }

        // Identify unassigned boarding gates
        List<BoardingGate> availableGates = new List<BoardingGate>();
        foreach (var gate in boardingGates.Values)
        {
            if (gate.AssignedFlight == null)
            {
                availableGates.Add(gate);
            }
        }

        Console.WriteLine($"Total Flights without a Boarding Gate: {unassignedFlights.Count}");
        Console.WriteLine($"Total Boarding Gates without an assigned Flight: {availableGates.Count}");

        int processedFlights = 0, alreadyAssigned = flights.Count - unassignedFlights.Count;

        // Assign boarding gates to flights
        while (unassignedFlights.Count > 0 && availableGates.Count > 0)
        {
            Flight flight = unassignedFlights.Dequeue();

            // Find an appropriate gate
            BoardingGate selectedGate = null;
            foreach (var gate in availableGates)
            {
                if ((flight.SpecialRequestCode == "None" && !gate.SupportsCFFT && !gate.SupportsDDJB && !gate.SupportsLWTT) ||
                    (flight.SpecialRequestCode == "CFFT" && gate.SupportsCFFT) ||
                    (flight.SpecialRequestCode == "DDJB" && gate.SupportsDDJB) ||
                    (flight.SpecialRequestCode == "LWTT" && gate.SupportsLWTT))
                {
                    selectedGate = gate;
                    break;
                }
            }

            if (selectedGate != null)
            {
                flight.BoardingGate = selectedGate.GateName;
                selectedGate.AssignedFlight = flight;
                availableGates.Remove(selectedGate);
                processedFlights++;

                Console.WriteLine($"Assigned Flight {flight.FlightNumber} to Gate {selectedGate.GateName}");
            }
        }

        Console.WriteLine($"Total Flights Assigned Automatically: {processedFlights}");
        Console.WriteLine($"Total Flights Already Assigned: {alreadyAssigned}");

        double assignmentPercentage = (double)processedFlights / (processedFlights + alreadyAssigned) * 100;
        Console.WriteLine($"Automation Assignment Percentage: {assignmentPercentage:F2}%");
    }



    static void DisplayScheduledFlights(Dictionary<string, Flight> flights)
    {
        if (flights.Count == 0)
        {
            Console.WriteLine("No scheduled flights available.");
            return;
        }

        // Convert dictionary to a sorted list
        List<Flight> sortedFlights = new List<Flight>(flights.Values);
        sortedFlights.Sort(); // Uses CompareTo from Flight class

        // Display formatted flight schedule
        Console.WriteLine("===============================================================================================");
        Console.WriteLine("Scheduled Flights for Today - Ordered by Departure/Arrival Time");
        Console.WriteLine("===============================================================================================");
        Console.WriteLine("Flight No.  Origin               Destination          Time                     Status     Special Req.      Boarding Gate");
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");

        foreach (Flight flight in sortedFlights)
        {
            Console.WriteLine(flight);
        }

        Console.WriteLine("===============================================================================================");
    }



    static void ListAllFlights(Dictionary<string, Flight> flights, Dictionary<string, Airline> airlinesByCode)
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("List of Flights for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine("Flight Number      Airline Name         Origin                 Destination            Expected Departure/Arrival Time\n");

        foreach (var flight in flights.Values)
        {
            string airlineName = GetAirlineName(flight.FlightNumber, airlinesByCode);
            Console.WriteLine(string.Format("{0,-18} {1,-20} {2,-22} {3,-22} {4:dd/M/yyyy hh:mm:ss tt}",
                flight.FlightNumber, airlineName, flight.Origin, flight.Destination, flight.ExpectedTime));
        }
        Console.WriteLine();
    }


    static string GetAirlineName(string flightNumber, Dictionary<string, Airline> airlinesByCode)
    {
        // Check if flight number is valid (at least 2 characters)
        if (string.IsNullOrEmpty(flightNumber) || flightNumber.Length < 2)
        {
            Console.WriteLine($"DEBUG: Invalid flight number '{flightNumber}', returning 'Unknown'");
            return "Unknown";
        }

        // Extract airline code from the first two characters, and make sure it's uppercase
        string airlineCode = flightNumber.Substring(0, 2).ToUpper().Trim();

        Console.WriteLine($"DEBUG: Extracted airline code '{airlineCode}' from flight number '{flightNumber}'");

        // Search for the airline in the dictionary
        if (airlinesByCode.TryGetValue(airlineCode, out Airline airline))
        {
            Console.WriteLine($"DEBUG: Matched airline '{airline.Name}' for code '{airlineCode}'");
            return airline.Name;
        }

        // If no match is found, log the available airline codes in the dictionary
        Console.WriteLine($"DEBUG: No match found for airline code '{airlineCode}'. Available codes are:");
        foreach (var key in airlinesByCode.Keys)
        {
            Console.WriteLine($"  - {key}");
        }

        return "Unknown";
    }



    static void ListBoardingGates(Dictionary<string, BoardingGate> boardingGates)
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("List of Boarding Gates for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine("Gate Name   DDJB   CFFT   LWTT");

        foreach (var gate in boardingGates.Values)
        {
            Console.WriteLine($"{gate.GateName,-10} {gate.SupportsDDJB,-6} {gate.SupportsCFFT,-6} {gate.SupportsLWTT,-6}");
        }
        Console.WriteLine();
    }


    static void AssignBoardingGate(Dictionary<string, Flight> flights, Dictionary<string, BoardingGate> boardingGates)
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


        Console.Write("Enter Boarding Gate: ");
        string boardingGate = Console.ReadLine();

        if (!boardingGates.ContainsKey(boardingGate))
        {
            Console.WriteLine($"Error: Boarding Gate {boardingGate} does not exist.");
            return;
        }

        boardingGates[boardingGate] = new BoardingGate(boardingGate, false, false, false); // Example fix

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

    static void DisplayAirlineFlights(Dictionary<string, Airline> airlines, Dictionary<string, Airline> airlinesByCode, Dictionary<string, Flight> flights)
    {
        Console.WriteLine("=============================================");
        Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
        Console.WriteLine("=============================================");
        Console.WriteLine("Airline Code Airline Name");

        foreach (var airline in airlines.Values)
        {
            Console.WriteLine($"{airline.Code,-12} {airline.Name}");
        }

        Console.Write("Enter Airline Code: ");
        string airlineCode = Console.ReadLine().Trim().ToUpper();

        if (airlinesByCode.TryGetValue(airlineCode, out Airline selectedAirline))
        {
            Console.WriteLine("=============================================");
            Console.WriteLine($"List of Flights for {selectedAirline.Name}");
            Console.WriteLine("=============================================");
            Console.WriteLine("Flight Number      Airline Name         Origin                 Destination            Expected Departure/Arrival Time");

            bool foundFlights = false;
            foreach (var flight in flights.Values)
            {
                if (flight.FlightNumber.StartsWith(airlineCode, StringComparison.OrdinalIgnoreCase))
                {
                    foundFlights = true;
                    // Format flight number with space after the airline code
                    string formattedFlightNumber = flight.FlightNumber.Length >= 2
                        ? $"{flight.FlightNumber.Substring(0, 2)} {flight.FlightNumber.Substring(2)}"
                        : flight.FlightNumber;

                    // Format time with lowercase am/pm
                    string formattedTime = flight.ExpectedTime.ToString("dd/M/yyyy h:mm:ss tt").ToLower();

                    Console.WriteLine($"{formattedFlightNumber,-18} {selectedAirline.Name,-20} {flight.Origin,-22} {flight.Destination,-22} {formattedTime}");
                }
            }

            if (!foundFlights)
            {
                Console.WriteLine($"No flights found for {selectedAirline.Name}");
            }
        }
        else
        {
            Console.WriteLine($"Error: Airline with code '{airlineCode}' not found.");
        }

        Console.WriteLine();
    }


    static void ModifyFlightDetails(Dictionary<string, Flight> flights, Dictionary<string, Airline> airlinesByCode)
    {
        DisplayAirlineFlights(airlinesByCode, airlinesByCode, flights);

        Console.Write("Choose an existing Flight to modify or delete: ");
        string flightNumber = Console.ReadLine().Trim().ToUpper();
        // Check if the flight number exists
        if (!flights.ContainsKey(flightNumber))
        {
            Console.WriteLine($"Error: Flight {flightNumber} not found.");
            return;
        }

        Console.WriteLine("1. Modify Flight");
        Console.WriteLine("2. Delete Flight");
        Console.Write("Choose an option: ");
        string option = Console.ReadLine().Trim().ToUpper();

        if (option == "1")
        {
            Flight selectedFlight = flights[flightNumber];
            Console.WriteLine("1. Modify Basic Information");
            Console.WriteLine("2. Modify Status");
            Console.WriteLine("3. Modify Special Request Code");
            Console.WriteLine("4. Modify Boarding Gate");
            Console.Write("Choose an option: ");
            string modifyOption = Console.ReadLine().Trim().ToUpper();

            switch (modifyOption)
            {
                case "1":
                    Console.Write("Enter new Origin: ");
                    string origin = Console.ReadLine();
                    Console.Write("Enter new Destination: ");
                    string destination = Console.ReadLine();
                    Console.Write("Enter new Expected Departure/Arrival Time (dd/mm/yyyy hh:mm): ");
                    DateTime expectedTime = DateTime.Parse(Console.ReadLine());
                    selectedFlight.Origin = origin;
                    selectedFlight.Destination = destination;
                    selectedFlight.ExpectedTime = expectedTime;
                    Console.WriteLine("Flight updated!");
                    break;
                case "2":
                    Console.Write("Enter new Status: ");
                    string status = Console.ReadLine();
                    selectedFlight.Status = status;
                    Console.WriteLine("Flight updated!");
                    break;
                case "3":
                    Console.Write("Enter new Special Request Code: ");
                    string specialRequestCode = Console.ReadLine();
                    //selectedFlight.SpecialRequestCode = specialRequestCode;
                    Console.WriteLine("Flight updated!");
                    break;
                case "4":
                    Console.Write("Enter new Boarding Gate: ");
                    string boardingGate = Console.ReadLine();
                    //selectedFlight.BoardingGate = boardingGate;
                    Console.WriteLine("Flight updated!");
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
        else if (option == "2")
        {
            Console.Write("Are you sure you want to delete this flight? (Y/N): ");
            string confirm = Console.ReadLine().Trim().ToUpper();
            if (confirm == "Y")
            {
                flights.Remove(flightNumber);
                Console.WriteLine("Flight deleted.");
            }
        }
        else
        {
            Console.WriteLine("Invalid option.");
        }


    }
}
