using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Model
{
    public class HotelApiSetting
    {
        public int ServiceTypeID { get; set; }
        public int HotelMKServiceID { get; set; }
        public int IsRida { get; set; }
        public string? CurrencyApiKey { get; set; }
        public int IsContAllocation { get; set; }
        public int IsLive { get; set; }
        public int IsReverseMarkup { get; set; }
        public int IsMultiDMC { get; set; }
        public int IsMarkupkeywise { get; set; }
        public int IsBookingAllow { get; set; }
        public int FreeCancellationBufferPeriod { get; set; }
    }

    public class ApiFileServerPathPaths
    {
        public string? AdminPath { get; set; }
        public string? AttractionImagePath { get; set; }
        public string? FileServerPath { get; set; }
    }
}
