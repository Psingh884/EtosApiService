using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Model
{
    public class PreHotelDetails
    {
        public int Id { get; set; }
        public string? HotelDetailResponse { get; set; }
        public int AgentID { get; set; }
        public int UserID { get; set; }
        public string? Section { get; set; }
        public string? QuoteId { get; set; }
        public string? Btoken { get; set; }
        public int BookingMasterID { get; set; }
        public string? SearchTraceID { get; set; }
        public int HotelID { get; set; }
        public int IsSplitStay { get; set; }
    }

    public class PreHotelServiceCart
    {
        public int Id { get; set; }
        public int ServiceTypeID { get; set; }
        public int AgentID { get; set; }
        public int UserID { get; set; }
        public int AgentContracedID { get; set; }
    }
}
