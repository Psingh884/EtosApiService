using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Flight.Model
{
    public class FlightSerachResponse
    {
        public class FlightShoppingResponse
        {
            public string? ServiceName { get; set; }
            public int ServiceId { get; set; }
            public int SupplierId { get; set; }
            public string? JourneyRefID { get; set; }
            public string? AirlineCode { get; set; }
            public string? DepartureTime { get; set; }
            public string? ArriveTime { get; set; }
            public int IsRefunddable { get; set; }
            public string? CabinClass { get; set; }
            public int Adult { get; set; }
            public int Child { get; set; }
            public int Infant { get; set; }
            public string? Currency { get; set; }
            public decimal TotalPrice { get; set; }
            public List<Flights> Flights { get; set; } = new List<Flights>();
            public List<PaxSegment> PaxSegments { get; set; } = new List<PaxSegment>();
        }

        public class Flights
        {
            public int FlightIndex { get; set; }
            public string? DepartureTime { get; set; }
            public string? ArriveTime { get; set; }
            public string? Duration { get; set; }
            public string? Stops { get; set; }
            public string? Refundable { get; set; }
            public string? Airline { get; set; }
            public string? FlightNumber { get; set; }
            public string? AirlineCode { get; set; }
            public string? CabinClass { get; set; }
            public string? ClassOfService { get; set; }
            public string? AirportCodeOrigin { get; set; }
            public string? AirportCodeDestination { get; set; }
            public List<FlightSegment> FlightSegments { get; set; } = new List<FlightSegment>();
        }

        public class PaxSegment
        {
            public int PaxIndex { get; set; }
            public string? PaxType { get; set; }
            public int PaxCount { get; set; }
            public string? FareBasisCode { get; set; }
            public string? Currency { get; set; }
            public decimal BasePrice { get; set; }
            public decimal Tax { get; set; }
            public decimal IPP { get; set; }
            public decimal ServiceCharges { get; set; }
            public decimal TotalPrice { get; set; }
        }

        public class FlightSegment
        {
            public int FlightIndex { get; set; }
            public int SegmentIndex { get; set; }
            public string? FlightNumber { get; set; }
            public string? Carrier { get; set; }
            public string? OperatingCarrier { get; set; }
            public string? DepartureDate { get; set; }
            public string? ArrivalDate { get; set; }
            public string? DepartureTime { get; set; }
            public string? ArrivalTime { get; set; }
            public string? OriginTerminal { get; set; }
            public string? SegmentDuration { get; set; }
            public string? DestinationTerminal { get; set; }
            public string? AirportCodeOrigin { get; set; }
            public string? AirportCodeDestination { get; set; }
            public string? AirportNameOrigin { get; set; }
            public string? AirportNameDestination { get; set; }
            public string? AirLineCityOrigin { get; set; }
            public string? AirLineCityDestination { get; set; }
            public string? CabinClass { get; set; }
            public string? ClassOfService { get; set; }
            public string? AirLineNames { get; set; }
            public string? Refundable { get; set; }
            public string? Baggage { get; set; }
        }
    }
}
