using EtosApiServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Model
{
    public class EHotelDetailResponse
    {
        public EHotelDetailResponse()
        {

        }
        public EHotelDetailResponse(EHotelDetailResponse OldHotelDetailResponse, EHotelDetail newetosHotelDetail)
        {
            this.Request = OldHotelDetailResponse.Request;
            this.ResponseCode = OldHotelDetailResponse.ResponseCode;
            this.ResponseData = newetosHotelDetail;
            this.HRDID = OldHotelDetailResponse.HRDID;
        }

        public EHotelDetailResponse(EHotelDetailResponse OldHotelDetailResponse, int RoomIndex)
        {
            this.Request = OldHotelDetailResponse.Request;
            this.ResponseCode = OldHotelDetailResponse.ResponseCode;
            var EHotelDetailObj = new EHotelDetail();
            EHotelDetailObj = OldHotelDetailResponse.ResponseData;
            var EHotelRoomDetailLst = new List<EHotelRoomDetail>();
            EHotelRoomDetailLst = EHotelDetailObj.EHotelRoomDetailLst.AsEnumerable().Where(f => f.RoomIndex.Equals(RoomIndex)).ToList();
            EHotelDetailObj.EHotelRoomDetailLst = new List<EHotelRoomDetail>();
            EHotelDetailObj.EHotelRoomDetailLst = EHotelRoomDetailLst;
            this.ResponseData = EHotelDetailObj;
            this.HRDID = OldHotelDetailResponse.HRDID;
        }
        public string HRDID { get; set; }
        public EHotelDetailRequest Request { get; set; }
        public Status ResponseCode { get; set; }
        public string ResponseMeassage { get; set; }
        public EHotelDetail ResponseData { get; set; }
        public string SearchTraceKey { get; set; }
    }
}
