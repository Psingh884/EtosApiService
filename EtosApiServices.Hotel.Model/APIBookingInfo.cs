using EtosApiServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Model
{
    public class APIBookingInfo
    {
        public APIBookingInfo()
        {
            this.BookingStatus = BookingStatus.None;
        }
        public BookingStatus BookingStatus { get; set; }
        public string ConfirmationNo { get; set; }
        public string SubConfirmationNo { get; set; }
        public string ClientRefernceNo { get; set; }
        public decimal BookingCost { get; set; }
        public string SupplierInfo { get; set; }
        public Dictionary<string, string> RequestLog { get; set; }
        public Dictionary<string, string> ResponseLog { get; set; }
    }
}
