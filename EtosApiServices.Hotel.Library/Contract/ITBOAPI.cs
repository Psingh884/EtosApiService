using EtosApiServices.Hotel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Library.Contract
{
    public interface ITBOAPI
    {
        Task<List<ETSearchHotel>> GetTboApiHotelResponseAsync(EHotelSearchRequest SearchReq, Dictionary<string, string> ApiCredentials, string FolderName, MarkupClass MarkupClassObj);

        Task<EHotelDetail> GetTboApiDetailResponseAsync(EHotelDetailRequest DetailReq, Dictionary<string, string> ApiCredentials, string FolderName, string BHRDID, MarkupClass MarkupClassObj);

        Task<APIPreBookingInfo> GetTboApiPreBookingResponseAsync(EHotelPreBookingRequest ObjEHotelPreBookingRequest, List<string> HotelExtraInfo, Dictionary<string, string> ApiCredentials, MarkupClass MarkupClassObj, string FolderName, int BookingMasterID, EHotelRoomDetail EHotelRoomDetailObj, EHotelDetailRequest Sreq);

        Task<APIBookingInfo> GetTboApiBookingResponseAsync(Dictionary<string, string> ApiCredentials, EHotelBookingRequest ObjEHotelBookingRequest, EHotelDetailRequest ObjEHotelDetailRequest, EHotelRoomDetail ObjEHotelRoomDetail, string FolderPath);
 
         Task<APICancelationInfo> GetTboApiCancelResponseAsync(Dictionary<string, string> ApiCredentials, string bookingreference, string FolderPath, int BookingID);

    }
}
