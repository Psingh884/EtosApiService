using EtosApiService.Filter;
using EtosApiServices.Business.Contract;
using EtosApiServices.Business.Services;
using EtosApiServices.Flight.Model;
using EtosApiServices.Hotel.Model;
using Microsoft.AspNetCore.Mvc;

namespace EtosApiService.Controllers
{
    [ApiController]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class HotelServiceController : Controller
    {
        private readonly IHotelService _hotelservice;

        public HotelServiceController(IHotelService hotelService)
        {
            _hotelservice = hotelService ?? throw new ArgumentNullException(nameof(hotelService));
        }

        [HttpPost]
        [Route("api/V1.0/HotelSearch")]
        public async Task<IActionResult> HotelSearch(EHotelSearchRequest ObjEHotelSearchRequest)
        {
            try
            {
                ObjEHotelSearchRequest.PlayerId = FirmInfo.PlayerId(Request);
                ObjEHotelSearchRequest.DBConnectioString = FirmInfo.DBConnectionString(Request);
                var result = await _hotelservice.HotelSearch(ObjEHotelSearchRequest);

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/V1.0/HotelDetail")]
        public async Task<IActionResult> HotelDetail(EHotelDetailRequest ObjEHotelDetailRequest)
        {
            try
            {               
                ObjEHotelDetailRequest.PlayerId = FirmInfo.PlayerId(Request);
                ObjEHotelDetailRequest.DBConnectioString = FirmInfo.DBConnectionString(Request);
                var result = await _hotelservice.HotelDetail(ObjEHotelDetailRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/V1.0/PreBookHotel")]
        public async Task<IActionResult> PreBookHotel(EHotelPreBookingRequest ObjEHotelPreBookingRequest)
        {
            try
            {                
                ObjEHotelPreBookingRequest.PlayerId = FirmInfo.PlayerId(Request);
                ObjEHotelPreBookingRequest.DBConnectioString = FirmInfo.DBConnectionString(Request);
                var result = await _hotelservice.PreBookHotel(ObjEHotelPreBookingRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/V1.0/BookHotel")]
        public async Task<IActionResult> BookHotel(EHotelBookingRequest ObjEHotelBookingRequest)
        {
            try
            {
                ObjEHotelBookingRequest.PlayerId = FirmInfo.PlayerId(Request);
                ObjEHotelBookingRequest.DBConnectioString = FirmInfo.DBConnectionString(Request);
                var result = await _hotelservice.BookHotel(ObjEHotelBookingRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/V1.0/CancelHotel")]
        public async Task<IActionResult> CancelHotel(EHotelCancelRequest ObjEHotelCancelRequest)
        {
            try
            {
                ObjEHotelCancelRequest.DBConnectioString = FirmInfo.DBConnectionString(Request);
                var result = await _hotelservice.CancelHotel(ObjEHotelCancelRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/V1.0/StaticDetailHotel")]
        public async Task<IActionResult> StaticDetailHotel(EHotelDetailRequest ObjEHotelDetailRequest)
        {
            try
            {
                ObjEHotelDetailRequest.DBConnectioString = FirmInfo.DBConnectionString(Request);
                var result = await _hotelservice.StaticDetailHotel(ObjEHotelDetailRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
