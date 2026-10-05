using EtosApiServices.Hotel.Model;
using EtosApiServices.Model;
using EtosApiServices.MongoDb.Library;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static EtosApiServices.Flight.Model.BabylonFlightCreateOrderResponse;

namespace EtosApiServices.Hotel.Library
{
    public class TboApiRequest
    {
        public TboApiRequest() { }


        public async Task<TBOHotelSearchResponse.SearchResponse> GetTboApiHotelResponseAsync(EHotelSearchRequest SearchReq, Dictionary<string, string> ApiCredentials, string HotelID, string FolderPath)
        {
            var Result = new TBOHotelSearchResponse.SearchResponse();

            try
            {
                TBOHotelSearchRequest ObjTBOHotelSearchRequest = new TBOHotelSearchRequest();
                ObjTBOHotelSearchRequest.CheckIn = SearchReq.CheckInDate_.ToString("yyyy-MM-dd");
                ObjTBOHotelSearchRequest.CheckOut = SearchReq.CheckOutDate_.ToString("yyyy-MM-dd");
                ObjTBOHotelSearchRequest.HotelCodes = HotelID;
                ObjTBOHotelSearchRequest.GuestNationality = SearchReq?.NationalityCode;
                ObjTBOHotelSearchRequest.PaxRooms = SearchReq?.RoomOccupancyDetails?.Select(rg => new TBOHotelSearchRequest.PaxRoom
                {
                    Adults = rg.AdultCount,
                    Children = rg.ChildCount,
                    ChildrenAges = rg.ChildAges ?? new List<int>()
                }).ToList() ?? new List<TBOHotelSearchRequest.PaxRoom>();
                ObjTBOHotelSearchRequest.ResponseTime = ApiCredentials.ContainsKey("ResponseTime") ? Convert.ToDouble(ApiCredentials["ResponseTime"]) : 23;
                ObjTBOHotelSearchRequest.IsDetailedResponse = SearchReq?.DestinationInfo?.HotelIds != null && SearchReq.DestinationInfo.HotelIds.Count != 0;
                ObjTBOHotelSearchRequest.Filters = new TBOHotelSearchRequest.TBOHotelSearchFilters();
                ObjTBOHotelSearchRequest.Filters.Refundable = false;
                ObjTBOHotelSearchRequest.Filters.NoOfRooms = 0;
                ObjTBOHotelSearchRequest.Filters.MealType = "All";

                string jsonReq = JsonConvert.SerializeObject(ObjTBOHotelSearchRequest);
                var resultstr = await GetTboApiResponseAsync(ApiCredentials["TBOApiEndpointUrl"] + "Search", jsonReq, ApiCredentials).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(resultstr))
                {
                    Result = JsonConvert.DeserializeObject<TBOHotelSearchResponse.SearchResponse>(resultstr);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Result ?? new TBOHotelSearchResponse.SearchResponse();
        }

        public async Task<TBOHotelSearchResponse.SearchResponse> GetTboApiHotelDetailsResponseAsync(EHotelDetailRequest DetailReq, Dictionary<string, string> ApiCredentials, string FolderPath)
        {
            var Result = new TBOHotelSearchResponse.SearchResponse();

            try
            {
                TBOHotelSearchRequest ObjTBOHotelSearchRequest = new TBOHotelSearchRequest();
                ObjTBOHotelSearchRequest.CheckIn = DetailReq.CheckInDate_.ToString("yyyy-MM-dd");
                ObjTBOHotelSearchRequest.CheckOut = DetailReq.CheckOutDate_.ToString("yyyy-MM-dd");
                ObjTBOHotelSearchRequest.HotelCodes = DetailReq.HotelId;
                ObjTBOHotelSearchRequest.GuestNationality = DetailReq?.NationalityCode;
                ObjTBOHotelSearchRequest.PaxRooms = DetailReq?.RoomOccupancyDetails?.Select(rg => new TBOHotelSearchRequest.PaxRoom
                {
                    Adults = rg.AdultCount,
                    Children = rg.ChildCount,
                    ChildrenAges = rg.ChildAges ?? new List<int>()
                }).ToList() ?? new List<TBOHotelSearchRequest.PaxRoom>();
                ObjTBOHotelSearchRequest.ResponseTime = ApiCredentials.ContainsKey("ResponseTime") ? Convert.ToDouble(ApiCredentials["ResponseTime"]) : 23;
                ObjTBOHotelSearchRequest.IsDetailedResponse = DetailReq?.DestinationInfo?.HotelIds != null && DetailReq.DestinationInfo.HotelIds.Count != 0;
                ObjTBOHotelSearchRequest.Filters = new TBOHotelSearchRequest.TBOHotelSearchFilters();
                ObjTBOHotelSearchRequest.Filters.Refundable = false;
                ObjTBOHotelSearchRequest.Filters.NoOfRooms = 0;
                ObjTBOHotelSearchRequest.Filters.MealType = "All";

                string jsonReq = JsonConvert.SerializeObject(ObjTBOHotelSearchRequest);
                var resultstr = await GetTboApiResponseAsync(ApiCredentials["TBOApiEndpointUrl"] + "Search", jsonReq, ApiCredentials).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(resultstr))
                {
                    Result = JsonConvert.DeserializeObject<TBOHotelSearchResponse.SearchResponse>(resultstr);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Result ?? new TBOHotelSearchResponse.SearchResponse();
        }

        public async Task<TBOHotelPreBookResponse.PreBookResponse> HotelCheckRate_Prebook(Dictionary<string, string> ApiCredentials, Dictionary<string, string> TBO_ExtraInfo, string FolderName)
        {
            var Result = new TBOHotelPreBookResponse.PreBookResponse();
            try
            {
                TBOHotelPreBookRequest.PreBookRequest ObjPreBookRequest = new TBOHotelPreBookRequest.PreBookRequest();
                ObjPreBookRequest.BookingCode = TBO_ExtraInfo["BookingCode"];
                ObjPreBookRequest.PaymentMode = "Limit";
                string jsonReq = JsonConvert.SerializeObject(ObjPreBookRequest);
                var resultstr = await GetTboApiResponseAsync(ApiCredentials["TBOApiEndpointUrl"] + "PreBook", jsonReq, ApiCredentials).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(resultstr))
                {
                    Result = JsonConvert.DeserializeObject<TBOHotelPreBookResponse.PreBookResponse>(resultstr);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Result ?? new TBOHotelPreBookResponse.PreBookResponse();
        }

        public async Task<APIBookingInfo> HotelBooking(Dictionary<string, string> ApiCredentials, List<RoomWisePaxDetails> lstRoomWisePaxDetails, Dictionary<string, string> TBO_ExtraInfo, string FolderName, int BookingID)
        {
            var result = new APIBookingInfo();
            var RequestLog = new Dictionary<string, string>();
            var ResponseLog = new Dictionary<string, string>();
            var TboBookingObj = new TBOHotelBookResponse.BookingResponse();
            try
            {
                string LeadGuest_EmailId = lstRoomWisePaxDetails.Where(p => p.Priority).Select(g => g.Email).FirstOrDefault() ?? string.Empty;
                string LeadGuest_PhoneNumber = lstRoomWisePaxDetails.Where(p => p.Priority).Select(g => g.MobileNo).FirstOrDefault() ?? string.Empty;

                TBOHotelBookRequest.BookingRequest ObjBookingRequest = new TBOHotelBookRequest.BookingRequest();
                ObjBookingRequest.BookingCode = TBO_ExtraInfo["BookingCode"];
                ObjBookingRequest.ClientReferenceId = "ETOS_" + BookingID;
                ObjBookingRequest.BookingReferenceId = "ETOS_" + BookingID;
                ObjBookingRequest.TotalFare = Convert.ToInt32(TBO_ExtraInfo["TotalFare"]);
                ObjBookingRequest.EmailId = LeadGuest_EmailId;
                ObjBookingRequest.PhoneNumber = LeadGuest_PhoneNumber;
                ObjBookingRequest.BookingType = "Voucher";



                string jsonReq = JsonConvert.SerializeObject(ObjBookingRequest);
                RequestLog.Add("TboApiBookingRequest", JsonConvert.SerializeObject(jsonReq));
                var resultstr = await GetTboApiResponseAsync(ApiCredentials["TBOApiEndpointUrl"] + "Book", jsonReq, ApiCredentials).ConfigureAwait(false);
                ResponseLog.Add("TboApiBookingResponse", resultstr);
                if (!string.IsNullOrEmpty(resultstr))
                {
                    result.RequestLog = RequestLog;
                    result.ResponseLog = ResponseLog;
                    result.SupplierInfo = AppConfig.GetConfigValue("Hotel:TBO:SupplierName");

                    TboBookingObj = JsonConvert.DeserializeObject<TBOHotelBookResponse.BookingResponse>(resultstr);
                    if (TboBookingObj != null && TboBookingObj.Status != null && TboBookingObj.Status.Code.Equals(Convert.ToInt32(HttpStatusCode.OK)))
                    {
                        result.ConfirmationNo = TboBookingObj?.ConfirmationNumber ?? string.Empty;
                        result.ClientRefernceNo = TboBookingObj?.ClientReferenceId ?? string.Empty;
                        result.BookingCost = 0;
                        result.BookingStatus = BookingStatus.BookingSuccessful;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result ?? new APIBookingInfo();
        }

        public async Task<APICancelationInfo> HotelCancel(Dictionary<string, string> ApiCredentials, string ConfirmationNumber, string FolderName, int Bookingid)
        {
            var APICancelationInfoObj = new APICancelationInfo();
            var RequestLog = new Dictionary<string, string>();
            var ResponseLog = new Dictionary<string, string>();
            var Result = new TBOHotelCancelResponse.CancelResponse();
            try
            {
                TBOHotelCancelRequest.CancelRequest ObjCancelRequest = new TBOHotelCancelRequest.CancelRequest();
                ObjCancelRequest.ConfirmationNumber = ConfirmationNumber;

                string jsonReq = JsonConvert.SerializeObject(ObjCancelRequest);
                RequestLog.Add("TBO_HotelCancelRequest", JsonConvert.SerializeObject(jsonReq));
                var resultstr = await GetTboApiResponseAsync(ApiCredentials["TBOApiEndpointUrl"] + "Cancel", jsonReq, ApiCredentials).ConfigureAwait(false);
                ResponseLog.Add("TBO_HotelCancelResponse", resultstr);

                APICancelationInfoObj.RequestLog = RequestLog;
                APICancelationInfoObj.ResponseLog = ResponseLog;

                if (!string.IsNullOrEmpty(resultstr))
                {
                    Result = JsonConvert.DeserializeObject<TBOHotelCancelResponse.CancelResponse>(resultstr);
                    if (Result != null && Result.Status != null && Result.Status.Code.Equals(Convert.ToInt32(HttpStatusCode.OK)) && Result.Status.Description == "Cancelled")
                    {
                        APICancelationInfoObj.CancellationCharges = 0;
                        APICancelationInfoObj.CancellationNo = Result.ConfirmationNumber + "_Cancel";
                        APICancelationInfoObj.CancellationStatus = BookingStatus.BookingCancel;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return APICancelationInfoObj ?? new APICancelationInfo();
        }

        public async Task<string> GetTboApiResponseAsync(string url, string requestBody, Dictionary<string, string> ApiCredentials)
        {
            using (var httpClient = new HttpClient())
            {
                string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(ApiCredentials["UserName"] + ":" + ApiCredentials["Password"]));
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("Authorization", string.Format("Basic {0}", credentials));
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

    }
}
