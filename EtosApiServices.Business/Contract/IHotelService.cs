using EtosApiServices.Hotel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Business.Contract
{
    public interface IHotelService
    {
        Task<EHotelSearchResponse> HotelSearch(EHotelSearchRequest ObjEHotelSearchRequest);

        Task<EHotelDetailResponse> HotelDetail(EHotelDetailRequest ObjEHotelDetailRequest);
        
        Task<EHotelPreBookingResponse> PreBookHotel(EHotelPreBookingRequest ObjEHotelPreBookingRequest);

        Task<EHotelBookingResponse> BookHotel(EHotelBookingRequest ObjEHotelBookingRequest);

        Task<EHotelCancelResponse> CancelHotel(EHotelCancelRequest ObjEHotelCancelRequest);

        Task<EHotelDetailResponse> StaticDetailHotel(EHotelDetailRequest ObjEHotelDetailRequest);
    }
}
