using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Model
{
    public class EHotelDetailRequest : EHotelSearchRequest
    {
        // Parameterless constructor added for JSON deserialization
        public EHotelDetailRequest()
        {
        }
        public EHotelDetailRequest(EHotelDetailRequest req_)
        {
            this.CheckInDate = req_.CheckInDate;
            this.CheckOutDate = req_.CheckOutDate;
            this.CurrencyCode = req_.CurrencyCode;
            this.DestinationInfo = req_.DestinationInfo;
            this.ExtraInfo = req_.ExtraInfo;
            this.HotelId = req_.HotelId;
            this.HotelstringIds = req_.HotelstringIds;
            this.LocalHotelIndex = req_.LocalHotelIndex;
            this.NationalityCode = req_.NationalityCode;
            this.Nights = req_.Nights;
            this.RequestedUserInfo = req_.RequestedUserInfo;
            this.RoomOccupancyDetails = req_.RoomOccupancyDetails;
            this.SDT = req_.SDT;
            this.SearchType = req_.SearchType;
            this.TotalNoRoom = req_.TotalNoRoom;
            this.ClientID = req_.ClientID;
            this.ApiKey = req_.ApiKey;
            this.LangId = req_.LangId;
            this.ismodifysearch = req_.ismodifysearch;
        }

        public string? HotelstringIds { get; set; }
        public string? ExtraInfo { get; set; }
        public string? LocalHotelIndex { get; set; }
        public string? SDT { get; set; }
        public string? HotelId { get; set; }
    }
}
