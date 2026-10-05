using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Flight.Model
{
    public class FlightSupplierDetails
    {
        public int AgentId { get; set; }
        public int IsCorporate { get; set; }
        public string? SupplierName { get; set; }
        public int APIMSupplierID { get; set; }
        public int IsActive { get; set; }
    }

    public class FlightSupplierCredential
    {
        public int PlayerId { get; set; }
        public int APISupplerId { get; set; }
        public string? APISupplierName { get; set; }
        public string? APICredential { get; set; }
    }
}
