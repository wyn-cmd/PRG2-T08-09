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
    }
}
