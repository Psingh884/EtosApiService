using EtosApiServices.Model;
using EtosApiServices.Model.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Model
{
    public class EHotelPreBookingRequest
    {
        public EHotelPreBookingRequest()
        {

        }
        public EHotelPreBookingRequest(int ClientID_)
        {
            this.ClientID = ClientID_;
        }
        public int ClientID { get; set; }
        public string HRDID { get; set; }
        public int RoomIndex { get; set; }
        public int SID { get; set; }
        public string ApiKey { get; set; }
        public decimal subagentmarkupvalue { get; set; }       

        [SwaggerExclude]
        public int PlayerId { get; set; }

        [SwaggerExclude]
        public string? DBConnectioString { get; set; }
    }

    public class EHotelPreBookingResponse
    {
        public EHotelPreBookingResponse(EHotelPreBookingRequest req_)
        {
            this.Req = req_;
        }
        public EHotelPreBookingRequest Req { get; set; }
        public bool AvailableForBook { get; set; }
        public bool IsPriceChange { get; set; }
        public decimal Before_Rate { get; set; }
        public decimal After_Rate { get; set; }
        public string Norms { get; set; }
        public Dictionary<string, string> RequestLog { get; set; }
        public Dictionary<string, string> ResponseLog { get; set; }
        public string CancellationPolicy { get; set; }
        public string CancellationDate { get; set; }
    }

    public class APIPreBookingInfo
    {
        public APIPreBookingInfo()
        {
            this.PreBookingStatus = BookingStatus.None;
        }      

        public string ClientReferenceNo { get; set; }
        public string PreBookingReferenceNo { get; set; }
        public BookingStatus PreBookingStatus { get; set; }
        public bool AvailableForBook { get; set; }
        public bool IsPriceChange { get; set; }
        public bool IsCancellationPolicyChange { get; set; } = false;
        public bool IsFreeCostCancellationDate { get; set; } = false;
        public decimal Before_Rate { get; set; }
        public decimal After_Rate { get; set; }
        public string Norms { get; set; }
        public bool IsCancellation { get; set; }
        public string CancellationPolicy { get; set; }
        public string CancellationDate { get; set; }
        public Dictionary<string, string> RequestLog { get; set; }
        public Dictionary<string, string> ResponseLog { get; set; }
        public object RawData { get; set; }
    }
}
