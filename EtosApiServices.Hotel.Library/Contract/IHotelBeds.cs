using EtosApiServices.Hotel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Library.Contract
{
    public interface IHotelBeds
    {
        Task<List<ETSearchHotel>> GetHotelBedsApiResponseAsync(EHotelSearchRequest SearchReq, Dictionary<string, string> ApiCredentials, string FolderName, MarkupClass MarkupClassObj);

        Task<EHotelDetail> GetHotelBedsApiDetailResponseAsync(EHotelDetailRequest DetailReq, Dictionary<string, string> ApiCredentials, string FolderName, string BHRDID, MarkupClass MarkupClassObj);

        Task<APIPreBookingInfo> GetHotelBedsApiPreBookingResponseAsync(EHotelPreBookingRequest ObjEHotelPreBookingRequest, List<string> HotelExtraInfo, Dictionary<string, string> ApiCredentials, MarkupClass MarkupClassObj, string Folderpath, int BookingMasterID, EHotelRoomDetail EHotelRoomDetailObj, EHotelDetailRequest Sreq);

        Task<APIBookingInfo> GetHotelBedsApiBookingResponseAsync(Dictionary<string, string> ApiCredentials, EHotelBookingRequest ObjEHotelBookingRequest, EHotelDetailRequest EHotelDetailRequest_, EHotelRoomDetail ObjEHotelRoomDetail, string FolderPath);

        Task<APICancelationInfo> GetHotelBedsApiCancelResponseAsync(Dictionary<string, string> ApiCredentials, string bookingreference, string FolderPath, int BookingID);
    }
}
