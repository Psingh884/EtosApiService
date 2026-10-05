using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Model.ApiSetting
{
    public class HotelBedsCredential
    {
        public string? APIName { get; set; }
        public string? SupplierName { get; set; }
        public int SupplierId { get; set; }
        public string? ApiKey { get; set; }
        public string? SharedSecret { get; set; }
        public string? ImagePath { get; set; }
        public string? SearchUrl { get; set; }
        public string? HotelDetailUrl { get; set; }
        public string? ReCheckRateUrl { get; set; }
        public string? BookingUrl { get; set; }
        public string? CancellationReq { get; set; }
        public string? LocationUrl { get; set; }
        public string? HotelBedsratecommentdetailsUrl { get; set; }
        public string? FacilityUrl { get; set; }
        public string? HotelBedsUtctime { get; set; }
        public string? HotelStaticContentapi { get; set; }
        public string? HotelBedsAPIURL { get; set; }
    }
}
