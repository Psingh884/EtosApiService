using EtosApiServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Model
{
    public class APICancelationInfo
    {
        public APICancelationInfo()
        {
            this.CancellationStatus = BookingStatus.ErrorInCancellation;
        }
        public string CancellationNo { get; set; }
        public BookingStatus CancellationStatus { get; set; }
        public decimal CancellationCharges { get; set; }
        public Dictionary<string, string> RequestLog { get; set; }
        public Dictionary<string, string> ResponseLog { get; set; }
    }
}
