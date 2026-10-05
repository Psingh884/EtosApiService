using EtosApiServices.Model;
using EtosApiServices.Model.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Model
{
    public class EHotelCancelRequest
    {
        public EHotelCancelRequest()
        {

        }
        public EHotelCancelRequest(int ClientID_)
        {
            this.ClientID = ClientID_;
        }
        public int ClientID { get; set; }
        public string BToken { get; set; }
        public string reason { get; set; }
        public string ApiKey { get; set; }
        public int Agent_ContactId { get; set; }
        public int AgentId { get; set; }

        [SwaggerExclude]
        public string? DBConnectioString { get; set; }
    }

    public class EHotelCancelResponse
    {
        public EHotelCancelResponse(EHotelCancelRequest Req_)
        {
            this.HotelCancelRequest = Req_;
        }
        public EHotelCancelResponse(EHotelCancelRequest Req_, APICancelationInfo cinfo)
        {
            this.HotelCancelRequest = Req_;
            this.CancellationStatus = cinfo.CancellationStatus;
            this.CancelReferenceNo = cinfo.CancellationNo;
            this.RequestLog = cinfo.RequestLog;
            this.ResponseLog = cinfo.ResponseLog;
        }
        public EHotelCancelRequest HotelCancelRequest { get; set; }
        public BookingStatus CancellationStatus { get; set; }
        public string CancelReferenceNo { get; set; }
        public Dictionary<string, string> RequestLog { get; set; }
        public Dictionary<string, string> ResponseLog { get; set; }

    }
}
