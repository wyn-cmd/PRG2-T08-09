# PRG2-T08-09

Flight Information Display System, the Programming II pair assignment by Wynston Wong (S10266219) and Ravin Nagpal (S10265740).

It is a console application for Changi Airport Terminal 5. On start it loads three CSV files into memory, then drops into a menu loop where the user can list flights and boarding gates, assign gates, create and modify flights, and print a daily airline fee report with discounts applied.

## Requirements and running

The project file is `Flight_Information_Display_System_09/Flight_Information_Display_System_09/S10265740_PRG2Assignment.csproj` and it targets .NET 6.

```
cd Flight_Information_Display_System_09/Flight_Information_Display_System_09
dotnet run
```

Start it from the project folder. The loader opens `airlines.csv`, `boardinggates.csv` and `flights.csv` by relative name, so the working directory has to be the one holding them (the project file copies all three into `bin/Debug/net6.0` on build, which also works).

There is no test project; the program is exercised through the menu.

## Data files

- `airlines.csv`: `Airline Name,Airline Code`, one airline per line. Name and code are kept as two separate lookup keys.
- `boardinggates.csv`: gate name followed by three true/false flags, read in the order CFFT, DDJB, LWTT. A line without exactly four columns is reported and skipped.
- `flights.csv`: `Flight Number,Origin,Destination,Expected Departure/Arrival,Special Request Code`. The special request code decides which subclass is built, DDJB, CFFT or LWTT, and anything else becomes a normal flight. Duplicate flight numbers are skipped with a warning.

Times in the sample file are time-of-day only ("11:45 AM"), which `DateTime.Parse` resolves against today's date. Flights created through the menu are appended to `flights.csv` with a full `dd/MM/yyyy HH:mm` stamp, so both formats end up in the file side by side.

## Menu

1. List All Flights: every flight with the airline name resolved from the flight number.
2. List Boarding Gates: each gate, the flight assigned to it and the request codes it supports.
3. Assign a Boarding Gate to a Flight: one flight at a time, rejecting a gate that is already taken or does not support the flight's special request.
4. Create Flight: prompts for the details, picks the subclass from the special request code and appends the row to `flights.csv`.
5. Display Airline Flights: the flights of one airline, looked up by code.
6. Modify Flight Details: change origin, destination, time, status, special request code or gate, or delete the flight.
7. Display Flight Schedule: every flight sorted by expected time, using `Flight.CompareTo`.
8. Bulk Assign Boarding Gates: queues all flights without a gate, walks the free gates and assigns the first one that supports the flight's request, then reports what is left over.
9. Calculate Airline Fees: refuses to run until every flight has a gate, then prints the report described below.
10. Search Flights by Destination: case-insensitive match on the destination string, with a count.
0. Exit.

## Flight types and fees

`Flight` is abstract with a single `CalculateFees()` hook, and every subclass adds its own special request charge on top of the same base and location fees.

| Type | Special request charge | Total |
| --- | --- | --- |
| NORMFlight | none | 300 base, plus 500 when the destination is Singapore (SIN) or 800 when the origin is |
| DDJBFlight | 300 | base plus location plus 300 |
| CFFTFlight | 150 | base plus location plus 150 |
| LWTTFlight | 500 | base plus location plus 500 |

Location fees only apply to Singapore (SIN) traffic; any other route pays the base fee alone. A boarding gate has its own `CalculateFees()` as well, 300 base plus 150, 300 or 500 for each request code it supports.

## Airline fee report

Starting from each flight's `CalculateFees()`, the report takes off these discounts per flight:

- 110 when the expected hour is before 11:00 or 21:00 onwards.
- 25 when the origin is Dubai (DXB), Bangkok (BKK) or Tokyo (NRT).
- 50 when the flight has no special request.

It then adds 350 for every complete group of three flights an airline operates, and 3% of that airline's base fees once it has more than five flights. The footer totals base fees, discounts, the net figure and the discount percentage. Flights are grouped per airline by testing whether the flight number starts with the airline name from `airlines.csv`.

## Source layout

The classes are split over two namespaces, `S10265740_PRG2Assignment` for the flight types and `Flight_Information_Display_System_09` for the gate and terminal.

- `Program.cs`: file loading, the menu loop and every report.
- `Flight.cs`: abstract base holding flight number, origin, destination, expected time, status, special request code, gate and the `CompareTo` ordering.
- `NORMFlight.cs`, `DDJBFlight.cs`, `CFFTFlight.cs`, `LWTTFFlight.cs`: the four flight types and their fee rules.
- `Airline.cs`: airline name, code, its flight dictionary and per airline fee totals.
- `BoardingGate.cs`: gate name, the request codes it supports, the flight assigned to it and the gate fee.
- `Terminal.cs`: terminal name with airline, flight, gate and gate fee collections. `Program` constructs it as "Terminal 5" but keeps its own dictionaries, so the terminal object is not the one the menu works on.
