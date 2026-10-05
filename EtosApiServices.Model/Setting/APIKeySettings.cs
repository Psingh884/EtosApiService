using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Model.Setting
{
    public class APIKeySettings
    {
        public const string HB_Database = "HotelBedsHotelData";
        public const string HB_Table = "HotelInfo";
        public const string TBO_Database = "TBOHotelDataV2";
        public const string TBO_Table = "HotelInfo";
    }

    public enum FlightApiSupplier
    {
        Babylon = 54
    }

    public enum HotelApiSupplier
    {
        LocalAPIID = 0,
        HBAPIID = 2,
        TBAPIID = 1
    }
}
