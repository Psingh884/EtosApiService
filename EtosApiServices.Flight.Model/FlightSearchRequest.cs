using EtosApiServices.Model.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Flight.Model
{
    public class FlightSearchRequest
    {
        [Required]
        public string? DepartureCity { get; set; }
        [Required]
        public string? DestinationCity { get; set; }
        [Required]
        public string? DepartureDate { get; set; }
        [Required]
        public string? DestinationDate { get; set; }
        [Required]
        public int Adults { get; set; }
        [Required]
        public int Childs { get; set; }
        [Required]
        public int Infants { get; set; }
        [Required]
        public string? Cabinclass { get; set; }
        [Required]
        public string? Currency { get; set; }
        [Required]
        public int IsDomestic { get; set; }
        [Required]
        public int JourneyTypeId { get; set; }
        public int AgentID { get; set; }

        [SwaggerExclude]
        public int CompanyID { get; set; }

        [SwaggerExclude]
        public int PlayerId { get; set; }

        [SwaggerExclude]
        public int ApiServiceId { get; set; } = 5;

        [SwaggerExclude]
        public string? DBConnectioString { get; set; }
        [SwaggerExclude]
        public string? XmlDataId { get; set; }
    }
}
