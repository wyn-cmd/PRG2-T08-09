using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//==========================================================
// Student Number : S10265740
// Student Name : Ravin Nagpal
// Partner Name : Wynston Wong
//==========================================================

namespace S10265740_PRG2Assignment
{
    public abstract class Flight
    {
        public string FlightNumber { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime ExpectedTime { get; set; }
        public string Status { get; set; }




        public abstract double CalculateFees();
        public override string ToString()
        {
            return $"FlightNumber: {FlightNumber}, Origin: {Origin}, Destination: {Destination}, ExpectedTime: {ExpectedTime}, Status: {Status}";
        }
    }
}
