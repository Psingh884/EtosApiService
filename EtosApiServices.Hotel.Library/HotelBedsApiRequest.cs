using EtosApiServices.Hotel.Model;
using EtosApiServices.Hotel.Model.ApiSetting;
using EtosApiServices.MongoDb.Library;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Library
{
    public class HotelBedsApiRequest
    {
        readonly HotelBedsCredential _hotelBedsCredential;
        public HotelBedsApiRequest(IOptions<HotelBedsCredential> hotelBedsCredential)
        {
            _hotelBedsCredential = hotelBedsCredential?.Value ?? throw new ArgumentException(nameof(hotelBedsCredential));
        }

        public async Task<HotelBedsAvailabilityResponse.HotelSearchResponse> GetHotelBedsApiResponseAsync(EHotelSearchRequest SearchReq, Dictionary<string, string> ApiCredentials, string FolderName)
        {
            var Result = new HotelBedsAvailabilityResponse.HotelSearchResponse();
            try
            {
                var TotalRoomsPaxOccupancy = (SearchReq.RoomOccupancyDetails ?? Enumerable.Empty<ERoomOccupancyInfo>())
                    .GroupBy(x => new { x.AdultCount, x.ChildCount, ChildAges = (x.ChildAges.Count > 0 ? string.Join("-", x.ChildAges) : "0-0-0") })
                    .Select(g => new
                    {
                        TotalRoom = g.Count(),
                        TotalAdults = g.Max(x => x.AdultCount),
                        TotalChildren = g.Max(x => x.ChildCount),
                        ChildAges = g.SelectMany(x => x.ChildAges).Where(h => h > 0).Distinct().ToList()
                    }).OrderByDescending(x => x.TotalRoom)
                    .ToList();



                HotelBedsAvailabilityRequest.HotelSearchRequest objHotelSearchRequest = new HotelBedsAvailabilityRequest.HotelSearchRequest();
                objHotelSearchRequest.stay = new HotelBedsAvailabilityRequest.Stay();
                objHotelSearchRequest.stay.checkIn = SearchReq.CheckInDate_.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                objHotelSearchRequest.stay.checkOut = SearchReq.CheckOutDate_.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                objHotelSearchRequest.occupancies = new List<HotelBedsAvailabilityRequest.Occupancy>();
                if (TotalRoomsPaxOccupancy != null && TotalRoomsPaxOccupancy.Count() > 0)
                {
                    objHotelSearchRequest.occupancies.AddRange(TotalRoomsPaxOccupancy.Select(x => new HotelBedsAvailabilityRequest.Occupancy
                    {
                        rooms = x.TotalRoom,
                        adults = x.TotalAdults,
                        children = x.TotalChildren,
                        paxes = x.ChildAges.Select(age => new HotelBedsAvailabilityRequest.Pax
                        {
                            type = "CH", // Assuming all are children; adjust as needed
                            age = age
                        }).ToList()
                    }));
                }
                objHotelSearchRequest.hotels = new HotelBedsAvailabilityRequest.Hotels();
                //objHotelSearchRequest.hotels.hotel = new List<int> { 162651, 707854, 82130 };
                objHotelSearchRequest.geolocation = new HotelBedsAvailabilityRequest.Geolocation();
                objHotelSearchRequest.geolocation.latitude = SearchReq.DestinationInfo.lat;
                objHotelSearchRequest.geolocation.longitude = SearchReq.DestinationInfo.lng;
                objHotelSearchRequest.geolocation.radius = SearchReq.DestinationInfo.RadiusinMeter / 1000;
                objHotelSearchRequest.geolocation.unit = "km";

                string jsonReq = JsonConvert.SerializeObject(objHotelSearchRequest);
                var resultstr = await GetResponseRestClient(Method.Post, Convert.ToString(ApiCredentials["HotelBedsAPIURL"]) + "hotel-api/1.0/hotels", jsonReq, ApiCredentials);
                if (!string.IsNullOrEmpty(resultstr))
                {
                    Result = JsonConvert.DeserializeObject<HotelBedsAvailabilityResponse.HotelSearchResponse>(resultstr);
                    if (Result != null && Result.hotels != null && Result.hotels.hotels != null)
                    {
                        var finalcitywisehotellist = Result.hotels.hotels.AsEnumerable().Where(xt => !string.IsNullOrWhiteSpace(xt.destinationName) && xt.destinationName.Contains(SearchReq.DestinationInfo.CityShortName.Trim())).ToList();
                        Result.hotels.hotels = finalcitywisehotellist;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Result ?? new HotelBedsAvailabilityResponse.HotelSearchResponse();
        }

        public async Task<HotelBedsDetailsAvailabilityResponse.HotelDetailsResponse> AvailabilityByHodelCode(EHotelDetailRequest SearchReq, Dictionary<string, string> ApiCredentials, string FolderName)
        {
            var Result = new HotelBedsDetailsAvailabilityResponse.HotelDetailsResponse();
            try
            {
                var TotalRoomsPaxOccupancy = (SearchReq.RoomOccupancyDetails ?? Enumerable.Empty<ERoomOccupancyInfo>())
                   .GroupBy(x => new { x.AdultCount, x.ChildCount, ChildAges = (x.ChildAges.Count > 0 ? string.Join("-", x.ChildAges) : "0-0-0") })
                   .Select(g => new
                   {
                       TotalRoom = g.Count(),
                       TotalAdults = g.Max(x => x.AdultCount),
                       TotalChildren = g.Max(x => x.ChildCount),
                       ChildAges = g.SelectMany(x => x.ChildAges).Where(h => h > 0).Distinct().ToList()
                   }).OrderByDescending(x => x.TotalRoom)
                   .ToList();



                HotelBedsAvailabilityRequest.HotelSearchRequest objHotelSearchRequest = new HotelBedsAvailabilityRequest.HotelSearchRequest();
                objHotelSearchRequest.stay = new HotelBedsAvailabilityRequest.Stay();
                objHotelSearchRequest.stay.checkIn = SearchReq.CheckInDate_.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                objHotelSearchRequest.stay.checkOut = SearchReq.CheckOutDate_.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                objHotelSearchRequest.occupancies = new List<HotelBedsAvailabilityRequest.Occupancy>();
                if (TotalRoomsPaxOccupancy != null && TotalRoomsPaxOccupancy.Count() > 0)
                {
                    objHotelSearchRequest.occupancies.AddRange(TotalRoomsPaxOccupancy.Select(x => new HotelBedsAvailabilityRequest.Occupancy
                    {
                        rooms = x.TotalRoom,
                        adults = x.TotalAdults,
                        children = x.TotalChildren,
                        paxes = x.ChildAges.Select(age => new HotelBedsAvailabilityRequest.Pax
                        {
                            type = "CH", // Assuming all are children; adjust as needed
                            age = age
                        }).ToList()
                    }));
                }
                objHotelSearchRequest.hotels = new HotelBedsAvailabilityRequest.Hotels();
                objHotelSearchRequest.hotels.hotel = new List<int> { int.Parse(SearchReq.HotelId ?? "0") };
                string jsonReq = JsonConvert.SerializeObject(objHotelSearchRequest);
                var resultstr = await GetResponseRestClient(Method.Post, Convert.ToString(ApiCredentials["HotelBedsAPIURL"]) + "hotel-api/1.0/hotels", jsonReq, ApiCredentials);
                if (!string.IsNullOrEmpty(resultstr))
                {
                    Result = JsonConvert.DeserializeObject<HotelBedsDetailsAvailabilityResponse.HotelDetailsResponse>(resultstr);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Result ?? new HotelBedsDetailsAvailabilityResponse.HotelDetailsResponse();
        }

        public async Task<HotelBedsModelClass.HotelBedsHotelDetail.Hotel> GetHotelInfoByHotelCode(Dictionary<string, string> ApiCredentials, string HotelCode)
        {
            var Result = new HotelBedsModelClass.HotelBedsHotelDetail.Hotel();
            try
            {
                var resultstr = await GetResponseRestClient(Method.Get, Convert.ToString(ApiCredentials["HotelBedsAPIURL"]) + "hotel-content-api/1.0/hotels/" + HotelCode, string.Empty, ApiCredentials);
                if (!string.IsNullOrEmpty(resultstr))
                {
                    Result = JsonConvert.DeserializeObject<HotelBedsModelClass.HotelBedsHotelDetail.Hotel>(resultstr);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Result ?? new HotelBedsModelClass.HotelBedsHotelDetail.Hotel();
        }

        public async Task<CheckResponse.HotelBedscheckratesRes> HotelCheckRate_Prebook(Dictionary<string, string> ApiCredentials, List<string> rateKey, string FolderName)
        {
            var Result = new CheckResponse.HotelBedscheckratesRes();
            try
            {
                var RoomReqLst = new List<HotelBedscheckrates.RoomReq>();
                foreach (var item in rateKey)
                {
                    var RoomReqObj = new HotelBedscheckrates.RoomReq();
                    RoomReqObj.rateKey = item;
                    RoomReqLst.Add(RoomReqObj);
                }

                var req = new HotelBedscheckrates.HotelBedscheckratesReq()
                {
                    rooms = RoomReqLst
                };

                var resultstr = await GetResponseRestClient(Method.Post, Convert.ToString(ApiCredentials["HotelBedsAPIURL"]) + "hotel-api/1.0/checkrates", JsonConvert.SerializeObject(req), ApiCredentials);
                if (!string.IsNullOrEmpty(resultstr))
                {
                    Result = JsonConvert.DeserializeObject<CheckResponse.HotelBedscheckratesRes>(resultstr);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Result ?? new CheckResponse.HotelBedscheckratesRes();
        }

        public async Task<List<HBRateCheck>> HotelCheckRate(Dictionary<string, string> ApiCredentials, List<string> rateKey, string FolderName, int Bookingid)
        {
            var HBRateCheckLst = new List<HBRateCheck>();
            var Result = new CheckResponse.HotelBedscheckratesRes();
            try
            {
                var RoomReqLst = new List<HotelBedscheckrates.RoomReq>();
                foreach (var item in rateKey)
                {
                    var RoomReqObj = new HotelBedscheckrates.RoomReq();
                    RoomReqObj.rateKey = item;
                    RoomReqLst.Add(RoomReqObj);
                }

                var req = new HotelBedscheckrates.HotelBedscheckratesReq()
                {
                    rooms = RoomReqLst
                };

                var resultstr = await GetResponseRestClient(Method.Post, Convert.ToString(ApiCredentials["HotelBedsAPIURL"]) + "hotel-api/1.0/checkrates", JsonConvert.SerializeObject(req), ApiCredentials);
                if (!string.IsNullOrEmpty(resultstr))
                {
                    Result = JsonConvert.DeserializeObject<CheckResponse.HotelBedscheckratesRes>(resultstr);
                    if (Result != null && Result.hotel != null && Result.hotel.rooms != null && Result.hotel.rooms.Count != 0)
                    {
                        foreach (var item in Result.hotel.rooms)
                        {
                            foreach (var ritem in item.rates)
                            {
                                var HBRateCheck_ = new HBRateCheck();
                                HBRateCheck_.rateComments = ritem?.rateComments ?? string.Empty;
                                HBRateCheck_.rateKeys = ritem?.rateKey ?? string.Empty;
                                decimal net_ = 0;
                                decimal.TryParse(ritem?.net, out net_);
                                HBRateCheck_.RatePrice = net_;

                                foreach (var r in ritem?.cancellationPolicies ?? new List<CheckResponse.CancellationPolicy>())
                                {
                                    HBRateCheck_.CancellationAmount = r.amount;
                                    HBRateCheck_.CancellationDate = r.from;
                                }
                                HBRateCheckLst.Add(HBRateCheck_);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return HBRateCheckLst ?? new List<HBRateCheck>();
        }

        public async Task<APIBookingInfo> HotelBooking(Dictionary<string, string> ApiCredentials, List<RoomWisePaxDetails> lstRoomWisePaxDetails, int totalnoofroom, List<HBRateCheck> rateKeys, string SpecialReq, string FolderPath, int BookingID)
        {
            var result = new APIBookingInfo();
            var RequestLog = new Dictionary<string, string>();
            var ResponseLog = new Dictionary<string, string>();
            var HotelBedsBookingObj = new HotelBedsBookingResponse.HotelBedsResponse();
            try
            {
                var mainpax = lstRoomWisePaxDetails.Where(f => f.Priority).FirstOrDefault();
                var roominfoLst = new List<HotelBedsBookingRequest.Room>();               

                var TotalRoomsPaxOccupancy = (lstRoomWisePaxDetails ?? Enumerable.Empty<RoomWisePaxDetails>())
                  .GroupBy(x => new { x.RoomNo })                  
                  .ToList();


                var BookingReq = new HotelBedsBookingRequest.HotelBedsRequest()
                {
                    clientReference = DateTime.Now.ToString("ddMMyyHHmmssfff") + "#" + RandomString(4),
                    holder = new HotelBedsBookingRequest.Holder()
                    {
                        name = mainpax?.FirstName ?? string.Empty,
                        surname = mainpax?.LastName ?? string.Empty
                    },
                    remark = SpecialReq,
                    rooms = roominfoLst
                };

                RequestLog.Add("HotelBedsBookingRequest", JsonConvert.SerializeObject(BookingReq));
                var resultstr = await GetResponseRestClient(Method.Post, Convert.ToString(ApiCredentials["HotelBedsAPIURL"]) + "hotel-api/1.0/bookings", JsonConvert.SerializeObject(BookingReq), ApiCredentials);
                ResponseLog.Add("HotelBedsBookingResponse", resultstr);
                if (!string.IsNullOrEmpty(resultstr))
                {
                    HotelBedsBookingObj = JsonConvert.DeserializeObject<HotelBedsBookingResponse.HotelBedsResponse>(resultstr);
                    result.RequestLog = RequestLog;
                    result.ResponseLog = ResponseLog;
                    result.SupplierInfo = AppConfig.GetConfigValue("Hotel:HotelBed:SupplierName");
                    if (HotelBedsBookingObj != null && HotelBedsBookingObj.booking != null && !string.IsNullOrEmpty(HotelBedsBookingObj?.booking.status))
                    {
                        if (HotelBedsBookingObj.booking.status.Equals("CONFIRMED"))
                        {
                            await BookingDetail(ApiCredentials, HotelBedsBookingObj.booking.reference ?? string.Empty, FolderPath, BookingID);
                            result.BookingCost = HotelBedsBookingObj.booking.totalNet;
                            result.BookingStatus = EtosApiServices.Model.BookingStatus.BookingSuccessful;
                            result.ClientRefernceNo = HotelBedsBookingObj.booking.clientReference ?? string.Empty;
                            result.ConfirmationNo = HotelBedsBookingObj.booking.reference ?? string.Empty;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return result ?? new APIBookingInfo();
        }

        public async Task<HotelBedsBookingDetailsClass.HotelBedsBookingDetail> BookingDetail(Dictionary<string, string> ApiCredentials, string bookingreference, string FolderPath, int BookingID)
        {
            var result = new HotelBedsBookingDetailsClass.HotelBedsBookingDetail();
            try
            {
                var bookingDetailStr = await GetResponseRestClient(Method.Get, Convert.ToString(ApiCredentials["HotelBedsAPIURL"]) + "hotel-api/1.0/bookings/" + bookingreference, string.Empty, ApiCredentials);
                string ResFileName = FolderPath + "/HB_BookingDetailResponse" + Guid.NewGuid().ToString().Replace("-", "_") + ".txt";
                result = JsonConvert.DeserializeObject<HotelBedsBookingDetailsClass.HotelBedsBookingDetail>(bookingDetailStr);
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public async Task<string> GetResponseRestClient(Method Methodobj, string URL, string Poststr, Dictionary<string, string> ApiCred)
        {
            string Result = string.Empty;
            try
            {
                var client = new RestClient(URL);
                var request = new RestRequest();
                request.Method = Methodobj;
                request.AddHeader("Api-key", ApiCred["ApiKey"]);
                request.AddHeader("X-Signature", GetSignatureKey(ApiCred));
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Accept-Encoding", "gzip");
                if (Methodobj == Method.Post)
                {
                    request.AddParameter("application/json", Poststr, ParameterType.RequestBody);
                }
                else
                {
                    request.AlwaysMultipartFormData = true;
                }
                RestResponse response = await client.ExecuteAsync(request);
                if (response.Content != null && response.Content != "")
                {
                    Result = response.Content;
                }
            }
            catch (Exception ex)
            {

            }
            return Result;
        }

        public string GetSignatureKey(Dictionary<string, string> ApiCred)
        {
            string apiKey = Convert.ToString(ApiCred["ApiKey"]);// HotelBedsApiCredentials.apiKey;// "vxn9xx39d9e3msex2q7qn72y";
            string sharedSecret = Convert.ToString(ApiCred["SharedSecret"]);//HotelBedsApiCredentials.sharedSecret;//  "cQEqKjBEa9";
            string HotelBedsUtctime = "0";
            HotelBedsUtctime = Convert.ToString(ApiCred["HotelBedsUtctime"]);

            // Compute the signature to be used in the API call (combined key + secret + timestamp in seconds)
            string signature;
            using (var sha = SHA256.Create())
            {
                long ts = (long)(DateTime.UtcNow.AddMinutes(-Convert.ToInt32(HotelBedsUtctime == null || HotelBedsUtctime.Equals(string.Empty) ? "0" : HotelBedsUtctime)) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds / 1000;
                Console.WriteLine("Timestamp: " + ts);
                var computedHash = sha.ComputeHash(Encoding.UTF8.GetBytes(apiKey + sharedSecret + ts));
                signature = BitConverter.ToString(computedHash).Replace("-", "");
            }
            return signature;
        }

        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
