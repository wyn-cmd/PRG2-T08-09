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


        Console.WriteLine();
        ShowMenu(terminal, boardingGates, flights, airlines, airlinesByCode);
    }


    static void SearchFlightsByDestination(Dictionary<string, Flight> flights)
    {
        Console.WriteLine("Format: Tokyo (NRT)");
        Console.Write("Enter Destination: ");
        string destination = Console.ReadLine().Trim().ToUpper();

        var matchingFlights = flights.Values
            .Where(flight => flight.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (matchingFlights.Count == 0)
        {
            Console.WriteLine($"No flights found for destination: {destination}");
        }
        else
        {
            Console.WriteLine($"Flights to {destination}:");
            Console.WriteLine("======================================================================");
            Console.WriteLine(string.Format("{0,-15} {1,-20} {2,-20} {3,-20} {4}",
                "Flight Number", "Origin", "Destination", "Expected Time", "Special Request"));
            Console.WriteLine("----------------------------------------------------------------------");

            foreach (var flight in matchingFlights)
            {
                Console.WriteLine(string.Format("{0,-15} {1,-20} {2,-20} {3:dd/MM/yyyy HH:mm} {4}",
                    flight.FlightNumber,
                    flight.Origin,
                    flight.Destination,
                    flight.ExpectedTime,
                    flight.SpecialRequestCode ?? "None"));
            }

            Console.WriteLine("======================================================================");
            Console.WriteLine($"Total Flights Found: {matchingFlights.Count}");
        }
        Console.WriteLine();
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
            Console.WriteLine("9. Calculate Airline Fees");
            Console.WriteLine("10. Search Flights by Destination");
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
                case "9":
                    CalculateAirlineFees(flights, airlines);
                    break;
                case "10":
                    SearchFlightsByDestination(flights);
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
        Console.WriteLine("{0,-10} {1,-25} {2,-20} {3,-20} {4,-15} {5}",
                          "Flight No.", "Origin", "Destination", "Time", "Status", "Boarding Gate");
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");

        foreach (Flight flight in sortedFlights)
        {
            // Ensure the expected time is displayed consistently in the format "hh:mm tt"
            string formattedTime = flight.ExpectedTime.ToString("hh:mm tt");

            // Print each flight with proper formatting, excluding SpecialRequest
            Console.WriteLine("{0,-10} {1,-25} {2,-20} {3,-20} {4,-15} {5}",
                              flight.FlightNumber,
                              flight.Origin,
                              flight.Destination,
                              formattedTime,
                              flight.Status,
                              flight.BoardingGate);
        }

        Console.WriteLine("===============================================================================================");
    }





    static void ListAllFlights(Dictionary<string, Flight> flights, Dictionary<string, Airline> airlines)
    {
        Console.WriteLine("===================================================================================");
        Console.WriteLine("List of Flights for Changi Airport Terminal 5");
        Console.WriteLine("===================================================================================");
        Console.WriteLine(string.Format("{0,-15} {1,-30} {2,-20} {3,-20} {4}",
            "Flight Number", "Airline", "Origin", "Destination", "Expected Time"));
        Console.WriteLine("-----------------------------------------------------------------------------------");

        foreach (var flight in flights.Values)
        {
            string airlineName = GetAirlineName(flight.FlightNumber, airlines);

            //unformatted flight number
            string formattedFlightNumber = flight.FlightNumber;

            Console.WriteLine(string.Format("{0,-15} {1,-30} {2,-20} {3,-20} {4:dd/MM/yyyy HH:mm}",
                formattedFlightNumber,
                airlineName,
                flight.Origin,
                flight.Destination,
                flight.ExpectedTime));
        }
        Console.WriteLine("\nTotal Flights: " + flights.Count);
        Console.WriteLine();
    }

    // Get the airline name from the flight number
    static string GetAirlineName(string flightNumber, Dictionary<string, Airline> airlines)
    {
        // Extract the first two characters (airline code) from the flight number
        string airlineCode = flightNumber.Substring(0, 2);

        // Loop through all airlines in the dictionary to find a match with the airline code
        foreach (var airline in airlines.Values)
        {
            // If the airline code matches the first two characters of the flight number, return the airline name
            if (airline.Name.Equals(airlineCode, StringComparison.OrdinalIgnoreCase))
            {
                return airline.Code;
            }
        }

        // If no match is found, return "Unknown"
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
        string flightNumber = Console.ReadLine().Trim().ToUpper();

        if (!flights.ContainsKey(flightNumber))
        {
            Console.WriteLine("Error: Flight not found.");
            return;
        }

        Flight selectedFlight = flights[flightNumber];
        Console.WriteLine("Selected Flight Information:");
        Console.WriteLine(selectedFlight.ToString());

        // Display available boarding gates
        Console.WriteLine("\nAvailable Boarding Gates:");
        foreach (var gate in boardingGates.Values)
        {
            if (gate.AssignedFlight == null)
            {
                Console.WriteLine($"Gate {gate.GateName} - DDJB: {gate.SupportsDDJB}, CFFT: {gate.SupportsCFFT}, LWTT: {gate.SupportsLWTT}");
            }
        }

        Console.Write("\nEnter Boarding Gate: ");
        string boardingGate = Console.ReadLine().Trim().ToUpper();

        if (!boardingGates.ContainsKey(boardingGate))
        {
            Console.WriteLine($"Error: Boarding Gate {boardingGate} does not exist.");
            return;
        }

        BoardingGate selectedGate = boardingGates[boardingGate];

        // Check if gate is already assigned
        if (selectedGate.AssignedFlight != null)
        {
            Console.WriteLine($"Error: Gate {boardingGate} is already assigned to flight {selectedGate.AssignedFlight.FlightNumber}");
            return;
        }

        // Check if gate supports the flight's special requirements
        bool isCompatible = false;
        if (selectedFlight is DDJBFlight && selectedGate.SupportsDDJB ||
            selectedFlight is CFFTFlight && selectedGate.SupportsCFFT ||
            selectedFlight is LWTTFlight && selectedGate.SupportsLWTT ||
            selectedFlight is NORMFlight)
        {
            isCompatible = true;
        }

        if (!isCompatible)
        {
            Console.WriteLine($"Error: Gate {boardingGate} does not support the flight's special requirements.");
            return;
        }

        // Update flight's boarding gate
        selectedFlight.BoardingGate = boardingGate;

        // Update gate's assigned flight
        selectedGate.AssignedFlight = selectedFlight;

        Console.Write("Would you like to update the Status of the Flight? (Y/N): ");
        string updateStatus = Console.ReadLine().Trim().ToUpper();

        if (updateStatus == "Y")
        {
            Console.Write("Enter new Status (Delayed, Boarding, On Time): ");
            selectedFlight.Status = Console.ReadLine().Trim();
        }

        Console.WriteLine("\nBoarding Gate assignment successful!");
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
                    Console.Write("Enter new Special Request Code (DDJB, CFFT, LWTT, or NONE): ");
                    string specialRequestCode = Console.ReadLine().Trim().ToUpper();

                    // Create a new flight object based on the special request code
                    Flight updatedFlight;
                    switch (specialRequestCode)
                    {
                        case "DDJB":
                            updatedFlight = new DDJBFlight(selectedFlight.FlightNumber,
                                selectedFlight.Origin, selectedFlight.Destination,
                                selectedFlight.ExpectedTime);
                            break;
                        case "CFFT":
                            updatedFlight = new CFFTFlight(selectedFlight.FlightNumber,
                                selectedFlight.Origin, selectedFlight.Destination,
                                selectedFlight.ExpectedTime);
                            break;
                        case "LWTT":
                            updatedFlight = new LWTTFlight(selectedFlight.FlightNumber,
                                selectedFlight.Origin, selectedFlight.Destination,
                                selectedFlight.ExpectedTime);
                            break;
                        case "NONE":
                            updatedFlight = new NORMFlight(selectedFlight.FlightNumber,
                                selectedFlight.Origin, selectedFlight.Destination,
                                selectedFlight.ExpectedTime, selectedFlight.Status);
                            break;
                        default:
                            Console.WriteLine("Invalid special request code. No changes made.");
                            return;
                    }

                    // Copy over any existing properties
                    updatedFlight.Status = selectedFlight.Status;
                    updatedFlight.BoardingGate = selectedFlight.BoardingGate;

                    // Replace the old flight with the updated one
                    flights[selectedFlight.FlightNumber] = updatedFlight;
                    Console.WriteLine("Flight special request code updated successfully!");
                    break;

                case "4":
                    Console.Write("Enter new Boarding Gate: ");
                    string boardingGate = Console.ReadLine().Trim().ToUpper();

                    // Validate that the boarding gate is not empty
                    if (string.IsNullOrWhiteSpace(boardingGate))
                    {
                        Console.WriteLine("Boarding gate cannot be empty.");
                        return;
                    }

                    // Update the boarding gate
                    selectedFlight.BoardingGate = boardingGate;
                    Console.WriteLine("Flight boarding gate updated successfully!");
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

    static void CalculateAirlineFees(Dictionary<string, Flight> flights, Dictionary<string, Airline> airlines)
    {
        //First check if all flights have boarding gates assigned
        bool allFlightsAssigned = true;
        List<string> unassignedFlights = new List<string>();

        foreach (var flight in flights.Values)
        {
            if (string.IsNullOrEmpty(flight.BoardingGate) || flight.BoardingGate == "Unassigned")
            {
                allFlightsAssigned = false;
                unassignedFlights.Add(flight.FlightNumber);
            }
        }

        if (!allFlightsAssigned)
        {
            Console.WriteLine("\nWARNING: Some flights do not have boarding gates assigned:");
            foreach (var flightNumber in unassignedFlights)
            {
                Console.WriteLine($"- Flight {flightNumber}");
            }
            Console.WriteLine("\nPlease assign boarding gates to all flights before calculating fees.");
            return;
        }

        // Dictionary to store fees per airline
        Dictionary<string, (decimal baseFees, decimal discounts)> airlineFees = new Dictionary<string, (decimal, decimal)>();

        //Calculate fees for each airline
        foreach (var airline in airlines.Values)
        {
            decimal subtotalFees = 0;
            decimal subtotalDiscounts = 0;
            int flightCount = 0;

            var airlineFlights = flights.Values.Where(f => f.FlightNumber.StartsWith(airline.Code)).ToList();
            flightCount = airlineFlights.Count;

            foreach (var flight in airlineFlights)
            {
                decimal flightFee = 0;

                //Base fee based on origin/destination
                if (flight.Origin.Contains("SIN"))
                    flightFee += 800; // Departure fee
                else if (flight.Destination.Contains("SIN"))
                    flightFee += 500; // Arrival fee

                //Boarding gate base fee
                flightFee += 300;

                //Special request fees
                switch (flight.SpecialRequestCode)
                {
                    case "DDJB":
                        flightFee += 300; //DDJB fee
                        break;
                    case "CFFT":
                        flightFee += 150; //CFFT fee
                        break;
                    case "LWTT":
                        flightFee += 500; //LWTT fee
                        break;
                }

                subtotalFees += flightFee;

                //Time-based discount (before 11am or after 9pm)
                if (flight.ExpectedTime.Hour < 11 || flight.ExpectedTime.Hour >= 21)
                {
                    subtotalDiscounts += 110;
                }

                //Origin-based discount
                if (flight.Origin == "Dubai (DXB)" || flight.Origin == "Bangkok (BKK)" || flight.Origin == "Tokyo (NRT)")
                {
                    subtotalDiscounts += 25;
                }

                //No special request code discount
                if (string.IsNullOrEmpty(flight.SpecialRequestCode) || flight.SpecialRequestCode == "None")
                {
                    subtotalDiscounts += 50;
                }
            }

            int groupsOfThree = flightCount / 3;
            subtotalDiscounts += groupsOfThree * 350;

            //More than 5 flights discount (3% off total before other discounts)
            if (flightCount > 5)
            {
                subtotalDiscounts += subtotalFees * 0.03m;
            }

            airlineFees[airline.Code] = (subtotalFees, subtotalDiscounts);
        }

        Console.WriteLine("\n=================================================================");
        Console.WriteLine("Terminal 5 - Daily Airline Fees Report");
        Console.WriteLine("=================================================================");
        Console.WriteLine(string.Format("{0,-20} {1,15} {2,15} {3,15}",
            "Airline", "Base Fees", "Discounts", "Final Fee"));
        Console.WriteLine("-----------------------------------------------------------------");

        decimal totalFees = 0;
        decimal totalDiscounts = 0;

        foreach (var airlineFee in airlineFees)
        {
            var airline = airlines.Values.First(a => a.Code == airlineFee.Key);
            decimal finalFee = airlineFee.Value.baseFees - airlineFee.Value.discounts;

            Console.WriteLine(string.Format("{0,-20} {1,15:C2} {2,15:C2} {3,15:C2}",
                airline.Name,
                airlineFee.Value.baseFees,
                airlineFee.Value.discounts,
                finalFee));

            totalFees += airlineFee.Value.baseFees;
            totalDiscounts += airlineFee.Value.discounts;
        }

        decimal finalTotalFees = totalFees - totalDiscounts;
        decimal discountPercentage = (totalDiscounts / totalFees) * 100;

        Console.WriteLine("=================================================================");
        Console.WriteLine($"Total Base Fees: {totalFees:C2}");
        Console.WriteLine($"Total Discounts: {totalDiscounts:C2}");
        Console.WriteLine($"Final Total Fees: {finalTotalFees:C2}");
        Console.WriteLine($"Discount Percentage: {discountPercentage:F2}%");
        Console.WriteLine("=================================================================\n");

        //Display detailed breakdown of discounts applied
        Console.WriteLine("\nDiscount Breakdown:");
        Console.WriteLine("-----------------------------------------------------------------");
        foreach (var airline in airlines.Values)
        {
            var airlineFlights = flights.Values.Where(f => f.FlightNumber.StartsWith(airline.Code)).ToList();
            if (airlineFlights.Any())
            {
                Console.WriteLine($"\n{airline.Name}:");
                int flightCount = airlineFlights.Count;

                //Groups of 3 flights discount
                int groupsOfThree = flightCount / 3;
                if (groupsOfThree > 0)
                    Console.WriteLine($"- Volume Discount ({groupsOfThree} groups of 3): ${groupsOfThree * 350:F2}");

                //More than 5 flights discount
                if (flightCount > 5)
                    Console.WriteLine($"- Over 5 Flights Discount (3%): ${airlineFees[airline.Code].baseFees * 0.03m:F2}");

                //Per-flight discounts
                int offPeakFlights = airlineFlights.Count(f => f.ExpectedTime.Hour < 11 || f.ExpectedTime.Hour >= 21);
                if (offPeakFlights > 0)
                    Console.WriteLine($"- Off-Peak Timing Discount ({offPeakFlights} flights): ${offPeakFlights * 110:F2}");

                int specialOriginFlights = airlineFlights.Count(f =>
                    f.Origin == "Dubai (DXB)" || f.Origin == "Bangkok (BKK)" || f.Origin == "Tokyo (NRT)");
                if (specialOriginFlights > 0)
                    Console.WriteLine($"- Special Origin Discount ({specialOriginFlights} flights): ${specialOriginFlights * 25:F2}");

                int noSpecialRequestFlights = airlineFlights.Count(f =>
                    string.IsNullOrEmpty(f.SpecialRequestCode) || f.SpecialRequestCode == "None");
                if (noSpecialRequestFlights > 0)
                    Console.WriteLine($"- No Special Request Discount ({noSpecialRequestFlights} flights): ${noSpecialRequestFlights * 50:F2}");
            }
        }
    }
}