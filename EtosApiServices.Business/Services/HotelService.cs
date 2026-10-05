using EtosApiServices.Business.Contract;
using EtosApiServices.Data.Library.Common;
using EtosApiServices.Data.Library.Repository;
using EtosApiServices.Flight.Library;
using EtosApiServices.Flight.Model;
using EtosApiServices.Flight.Model.ApiSetting;
using EtosApiServices.Hotel.Library;
using EtosApiServices.Hotel.Library.Contract;
using EtosApiServices.Hotel.Library.Services;
using EtosApiServices.Hotel.Model;
using EtosApiServices.Hotel.Model.ApiSetting;
using EtosApiServices.Model;
using EtosApiServices.Model.Setting;
using EtosApiServices.MongoDb.Library;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EtosApiServices.Business.Services
{
    public class HotelService : IHotelService
    {
        private readonly IAPICredentials _iapiCredentials;
        private readonly HotelApiSetting _hotelApiSetting;
        private readonly IHotelApiCommonFunction _ihotelApiCommonFunction;
        private readonly IContractedHotel _iContractedHotel;
        private readonly IHotelBeds _ihotelBeds;
        private readonly ITBOAPI _iTBOAPI;
        public readonly string Base_FolderURL = AppConfig.ServerMapPath("~/APILogs/");
        public static bool IsLive = string.IsNullOrEmpty(AppConfig.GetConfigValue("AppSettings:IsLive")) ? false : Convert.ToBoolean(int.Parse(AppConfig.GetConfigValue("AppSettings:IsLive")));
        private const int DefaultMaxConcurrency = 8;

        public HotelService(IAPICredentials aPICredentials, IOptions<HotelApiSetting> hotelApiSetting, IContractedHotel iContractedHotel, IHotelBeds iHotelBeds, ITBOAPI itBOAPI, IHotelApiCommonFunction hotelApiCommonFunction)
        {
            _iapiCredentials = aPICredentials ?? throw new ArgumentException(nameof(aPICredentials));
            _hotelApiSetting = hotelApiSetting?.Value ?? throw new ArgumentException(nameof(hotelApiSetting));
            _iContractedHotel = iContractedHotel ?? throw new ArgumentException(nameof(iContractedHotel));
            _ihotelBeds = iHotelBeds ?? throw new ArgumentException(nameof(iHotelBeds));
            _iTBOAPI = itBOAPI ?? throw new ArgumentException(nameof(itBOAPI));
            _ihotelApiCommonFunction = hotelApiCommonFunction ?? throw new ArgumentException(nameof(hotelApiCommonFunction));
        }

        // Helper to run tasks with bounded concurrency using Parallel.ForEachAsync
        private static Task ForEachAsync<T>(IEnumerable<T> source, int maxDegreeOfParallelism, Func<T, Task> body)
        {
            if (source == null) return Task.CompletedTask;
            if (maxDegreeOfParallelism <= 0) maxDegreeOfParallelism = DefaultMaxConcurrency;
            return Parallel.ForEachAsync(source, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, async (item, ct) =>
            {
                await body(item).ConfigureAwait(false);
            });
        }

        public async Task<EHotelSearchResponse> HotelSearch(EHotelSearchRequest ObjEHotelSearchRequest)
        {
            if (ObjEHotelSearchRequest == null) throw new ArgumentNullException(nameof(ObjEHotelSearchRequest));

            var results = new ConcurrentBag<List<ETSearchHotel>>();

            // Only perform radius search processing; otherwise return empty response wrapper.
            if (ObjEHotelSearchRequest.DestinationInfo == null || !ObjEHotelSearchRequest.DestinationInfo.IsRadiusSearch)
            {
                return new EHotelSearchResponse(ObjEHotelSearchRequest);
            }

            string SearchTraceKey = Guid.NewGuid().ToString().Replace('-', '_');

            var APICredentialsObj = await _iapiCredentials.GetAPICredentialsByApiKey(ObjEHotelSearchRequest.ClientID, ObjEHotelSearchRequest.ApiKey ?? string.Empty, _hotelApiSetting.ServiceTypeID, IsLive, ObjEHotelSearchRequest.DBConnectioString ?? string.Empty).ConfigureAwait(false);
            if (APICredentialsObj == null || APICredentialsObj.Count == 0)
            {
                return new EHotelSearchResponse(ObjEHotelSearchRequest);
            }

            APICredentialsObj = await _iapiCredentials.SortAgentWiseAvaibaleAPIBy(APICredentialsObj, ObjEHotelSearchRequest.Agent_ContactedID, ObjEHotelSearchRequest.DBConnectioString ?? string.Empty).ConfigureAwait(false);

            if (APICredentialsObj.Count > 0 && ObjEHotelSearchRequest.SupplierID > 0)
            {
                var sortAPI = APICredentialsObj.Where(b => b.Key.Equals(ObjEHotelSearchRequest.SupplierID)).ToList();
                if (sortAPI.Count > 0)
                {
                    APICredentialsObj = sortAPI.ToDictionary(d => d.Key, b => b.Value);
                }
            }

            var APIMarkupClass = await _iapiCredentials.GetAPIMarkupsByAPIKey(
                ObjEHotelSearchRequest.ClientID,
                (_hotelApiSetting.IsMarkupkeywise == 1 ? ObjEHotelSearchRequest.ApiKey ?? string.Empty : string.Empty),
                _hotelApiSetting.HotelMKServiceID,
                (ObjEHotelSearchRequest.Agent_ContactedID > 0 ? 0 : 1),
                0,
                ObjEHotelSearchRequest.DestinationInfo?.CityId ?? 0,
                ObjEHotelSearchRequest.Agent_ContactedID,
                ObjEHotelSearchRequest.DBConnectioString ?? string.Empty).ConfigureAwait(false) ?? new Dictionary<int, MarkupClass>();

            // Run API calls with bounded concurrency
            await ForEachAsync(APICredentialsObj, DefaultMaxConcurrency, async apicKVp =>
            {
                var apic = apicKVp; // KeyValuePair<int, T>

                if (apic.Key.Equals((int)HotelApiSupplier.LocalAPIID))
                {
                    var markup = APIMarkupClass.ContainsKey((int)HotelApiSupplier.LocalAPIID) ? APIMarkupClass[(int)HotelApiSupplier.LocalAPIID] : null;
                    var res = await _iContractedHotel.GetContractedHotelApiResponseAsync(ObjEHotelSearchRequest, apic.Value, "FolderName", markup ?? new MarkupClass()).ConfigureAwait(false);
                    if (res != null)
                        results.Add(res);
                }
                else if (apic.Key.Equals((int)HotelApiSupplier.HBAPIID))
                {
                    var markup = APIMarkupClass.ContainsKey((int)HotelApiSupplier.HBAPIID) ? APIMarkupClass[(int)HotelApiSupplier.HBAPIID] : null;
                    var res = await _ihotelBeds.GetHotelBedsApiResponseAsync(ObjEHotelSearchRequest, apic.Value, "FolderName", markup ?? new MarkupClass()).ConfigureAwait(false);
                    if (res != null)
                        results.Add(res);
                }
                else if (apic.Key.Equals((int)HotelApiSupplier.TBAPIID))
                {
                    var markup = APIMarkupClass.ContainsKey((int)HotelApiSupplier.TBAPIID) ? APIMarkupClass[(int)HotelApiSupplier.TBAPIID] : null;
                    var res = await _iTBOAPI.GetTboApiHotelResponseAsync(ObjEHotelSearchRequest, apic.Value, "FolderName", markup ?? new MarkupClass()).ConfigureAwait(false);
                    if (res != null)
                        results.Add(res);
                }

            }).ConfigureAwait(false);

            var merged = results.SelectMany(list => list).ToList();
            var response = new EHotelSearchResponse(ObjEHotelSearchRequest, merged, "SearchTraceFolder")
            {
                SearchTraceKey = SearchTraceKey
            };

            return response;
        }

        public async Task<EHotelDetailResponse> HotelDetail(EHotelDetailRequest ObjEHotelDetailRequest)
        {
            if (ObjEHotelDetailRequest == null) throw new ArgumentNullException(nameof(ObjEHotelDetailRequest));

            var EHotelDetailResponseObj = new EHotelDetailResponse();
            var results = new ConcurrentBag<EHotelDetail>();

            string clientFolder = Path.Combine(Base_FolderURL ?? string.Empty, ObjEHotelDetailRequest.ClientID.ToString(), "DetailHotel");
            string detailsTraceId = Guid.NewGuid().ToString().Replace('-', '_');
            string searchTraceId = string.Concat(detailsTraceId, "_", "HotelDetail");
            string searchTraceFolder = Path.Combine(clientFolder, searchTraceId);

            if (!Directory.Exists(Base_FolderURL))
            {
                Directory.CreateDirectory(Base_FolderURL ?? "");
            }
            if (!Directory.Exists(searchTraceFolder))
            {
                Directory.CreateDirectory(searchTraceFolder);
            }

            var APICredentialsObj = await _iapiCredentials.GetAPICredentialsByApiKey(ObjEHotelDetailRequest.ClientID, ObjEHotelDetailRequest.ApiKey ?? string.Empty, _hotelApiSetting.ServiceTypeID, IsLive, ObjEHotelDetailRequest.DBConnectioString ?? string.Empty).ConfigureAwait(false);
            if (APICredentialsObj == null || APICredentialsObj.Count == 0)
            {
                return EHotelDetailResponseObj ?? new EHotelDetailResponse();
            }

            var APIMarkupClass = await _iapiCredentials.GetAPIMarkupsByAPIKey(
                ObjEHotelDetailRequest.ClientID,
                (_hotelApiSetting.IsMarkupkeywise == 1 ? ObjEHotelDetailRequest.ApiKey ?? string.Empty : string.Empty),
                _hotelApiSetting.HotelMKServiceID,
                (ObjEHotelDetailRequest.Agent_ContactedID > 0 ? 0 : 1),
                0,
                ObjEHotelDetailRequest.DestinationInfo?.CityId ?? 0,
                ObjEHotelDetailRequest.Agent_ContactedID,
                ObjEHotelDetailRequest.DBConnectioString ?? string.Empty).ConfigureAwait(false) ?? new Dictionary<int, MarkupClass>();

            var detailRequests = new List<EHotelDetailRequest>();

            var apiList = (ObjEHotelDetailRequest.HotelstringIds ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var extraInfoLst = (ObjEHotelDetailRequest.ExtraInfo ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.None);

            // Build detail requests with correct indexing first
            for (int i = 0; i < apiList.Length; i++)
            {
                var apic = apiList[i];
                var apiInfo = apic.Split('~');
                var apiValue = apiInfo.LastOrDefault();
                var extraInfoValue = extraInfoLst.Length > i ? extraInfoLst[i] : ObjEHotelDetailRequest.ExtraInfo;

                var sreq = new EHotelDetailRequest(ObjEHotelDetailRequest)
                {
                    HotelstringIds = apic,
                    HotelId = apiValue,
                    ExtraInfo = extraInfoValue
                };

                detailRequests.Add(sreq);
            }

            // Run supplier-specific detail calls concurrently with bounded parallelism
            await ForEachAsync(detailRequests, DefaultMaxConcurrency, async sreq =>
            {
                if (string.IsNullOrEmpty(sreq.HotelstringIds))
                    return;

                var apiInfo = sreq.HotelstringIds.Split('~');
                var apiKey = apiInfo.FirstOrDefault();

                if (apiKey != null && apiKey.Equals(AppConfig.GetConfigValue("Hotel:Contracted:APIName"), StringComparison.OrdinalIgnoreCase))
                {
                    if (APICredentialsObj.ContainsKey((int)HotelApiSupplier.LocalAPIID))
                    {
                        var markup = APIMarkupClass.ContainsKey((int)HotelApiSupplier.LocalAPIID) ? APIMarkupClass[(int)HotelApiSupplier.LocalAPIID] : null;
                        var creds = APICredentialsObj[(int)HotelApiSupplier.LocalAPIID];
                        var res = await _iContractedHotel.GetContractedHotelApiDetailResponseAsync(sreq, creds, searchTraceFolder, detailsTraceId, markup ?? new MarkupClass()).ConfigureAwait(false);
                        if (res != null)
                            results.Add(res);
                    }
                }
                else if (apiKey != null && apiKey.Equals(AppConfig.GetConfigValue("Hotel:HotelBed:APIName"), StringComparison.OrdinalIgnoreCase))
                {
                    if (APICredentialsObj.ContainsKey((int)HotelApiSupplier.HBAPIID))
                    {
                        var markup = APIMarkupClass.ContainsKey((int)HotelApiSupplier.HBAPIID) ? APIMarkupClass[(int)HotelApiSupplier.HBAPIID] : null;
                        var creds = APICredentialsObj[(int)HotelApiSupplier.HBAPIID];
                        var res = await _ihotelBeds.GetHotelBedsApiDetailResponseAsync(sreq, creds, searchTraceFolder, detailsTraceId, markup ?? new MarkupClass()).ConfigureAwait(false);
                        if (res != null)
                            results.Add(res);
                    }
                }

                else if (apiKey != null && apiKey.Equals(AppConfig.GetConfigValue("Hotel:TBO:APIName"), StringComparison.OrdinalIgnoreCase))
                {
                    if (APICredentialsObj.ContainsKey((int)HotelApiSupplier.TBAPIID))
                    {
                        var markup = APIMarkupClass.ContainsKey((int)HotelApiSupplier.TBAPIID) ? APIMarkupClass[(int)HotelApiSupplier.TBAPIID] : null;
                        var creds = APICredentialsObj[(int)HotelApiSupplier.TBAPIID];
                        var res = await _iTBOAPI.GetTboApiDetailResponseAsync(sreq, creds, searchTraceFolder, detailsTraceId, markup ?? new MarkupClass()).ConfigureAwait(false);
                        if (res != null)
                            results.Add(res);
                    }
                }
            }).ConfigureAwait(false);

            // Preserve the prepared requests in the response in case callers rely on it
            EHotelDetailResponseObj.SearchTraceKey = ObjEHotelDetailRequest.SearchTraceKey ?? "";
            EHotelDetailResponseObj.HRDID = detailsTraceId;
            EHotelDetailResponseObj.Request = ObjEHotelDetailRequest;
            EHotelDetailResponseObj.ResponseCode = Status.successful;
            EHotelDetailResponseObj.ResponseData = new EHotelDetail(results.ToList(), ObjEHotelDetailRequest);
            EHotelDetailResponseObj.ResponseMeassage = Status.successful.ToString();

            // Write trace file asynchronously
            var detailFilePath = Path.Combine(clientFolder, detailsTraceId + ".txt");
            await File.WriteAllTextAsync(detailFilePath, JsonConvert.SerializeObject(EHotelDetailResponseObj)).ConfigureAwait(false);
            return EHotelDetailResponseObj ?? new EHotelDetailResponse();
        }

        public async Task<EHotelPreBookingResponse> PreBookHotel(EHotelPreBookingRequest ObjEHotelPreBookingRequest)
        {
            if (ObjEHotelPreBookingRequest == null) throw new ArgumentNullException(nameof(ObjEHotelPreBookingRequest));
            var EHotelPreBookingResponseObj = new EHotelPreBookingResponse(ObjEHotelPreBookingRequest);
            var APICredentialsObj = await _iapiCredentials.GetAPICredentialsByApiKey(ObjEHotelPreBookingRequest.ClientID, ObjEHotelPreBookingRequest.ApiKey ?? string.Empty, _hotelApiSetting.ServiceTypeID, IsLive, ObjEHotelPreBookingRequest.DBConnectioString ?? string.Empty).ConfigureAwait(false);
            if (APICredentialsObj == null || APICredentialsObj.Count == 0)
            {
                return EHotelPreBookingResponseObj;
            }

            string EBtoken = Guid.NewGuid().ToString();
            string ClientWiseBookingFolder = Path.Combine(Base_FolderURL ?? string.Empty, ObjEHotelPreBookingRequest.ClientID.ToString(), "PreBook", EBtoken) + Path.DirectorySeparatorChar;
            string PreBook_FolderURL = Path.Combine(Base_FolderURL ?? string.Empty, ObjEHotelPreBookingRequest.ClientID.ToString(), "PreBook") + Path.DirectorySeparatorChar;
            if (!Directory.Exists(ClientWiseBookingFolder)) { Directory.CreateDirectory(ClientWiseBookingFolder); }
            if (!Directory.Exists(PreBook_FolderURL)) { Directory.CreateDirectory(PreBook_FolderURL); }

            int BookingMasterID = 0;
            var APIPreBookingInfo_ = new ConcurrentBag<APIPreBookingInfo>();
            EHotelDetailResponse EHotelDetailResponseObj = new EHotelDetailResponse();
            var prebookEHotelRoomDetail = new EHotelRoomDetail();
            int AgentContracedID = 0;
            var HoteDetais = await _ihotelApiCommonFunction.GetPreHotelDetailsAsync(ObjEHotelPreBookingRequest.HRDID, ObjEHotelPreBookingRequest.SID, ObjEHotelPreBookingRequest.DBConnectioString ?? string.Empty).ConfigureAwait(false);
            if (HoteDetais.Result1 != null && HoteDetais.Result2 != null)
            {
                var preHotelDetail = HoteDetais.Result1.FirstOrDefault();
                var preHotelServiceCart = HoteDetais.Result2.FirstOrDefault();
                if (preHotelDetail != null && !string.IsNullOrEmpty(preHotelDetail.HotelDetailResponse))
                {
                    var EHotelDetailResponseObjOld = JsonConvert.DeserializeObject<EHotelDetailResponse>(preHotelDetail.HotelDetailResponse ?? string.Empty, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    string[] fname = EHotelDetailResponseObjOld?.ResponseData?.EHotelRoomDetailLst?.FirstOrDefault()?.BHRDID?.Split('!');
                    var detailFilePath = Path.Combine(Base_FolderURL ?? string.Empty, ObjEHotelPreBookingRequest.ClientID.ToString(), "DetailHotel", (fname != null && fname.Length > 0 ? fname[0] : string.Empty) + ".txt");
                    if (File.Exists(detailFilePath))
                    {
                        string _HotelDetailFilePath_Content = await File.ReadAllTextAsync(detailFilePath).ConfigureAwait(false);
                        EHotelDetailResponseObj = JsonConvert.DeserializeObject<EHotelDetailResponse>(_HotelDetailFilePath_Content) ?? new EHotelDetailResponse();
                    }
                }
                if (preHotelServiceCart != null)
                {
                    int.TryParse(preHotelServiceCart.AgentContracedID.ToString(), out AgentContracedID);
                }
            }

            if (EHotelDetailResponseObj != null && EHotelDetailResponseObj.ResponseCode == Status.successful)
            {

                Dictionary<int, MarkupClass> APIMarkupClass = await _iapiCredentials.GetAPIMarkupsByAPIKey(ObjEHotelPreBookingRequest.ClientID, ObjEHotelPreBookingRequest.ApiKey ?? string.Empty, _hotelApiSetting.HotelMKServiceID, (AgentContracedID > 0 ? 0 : 1), 0, EHotelDetailResponseObj.Request.DestinationInfo.CityId, AgentContracedID, ObjEHotelPreBookingRequest.DBConnectioString ?? string.Empty).ConfigureAwait(false);
                EHotelDetail EHotelDetailObj = EHotelDetailResponseObj?.ResponseData ?? new EHotelDetail();
                EHotelRoomDetail EHotelRoomDetailObj = EHotelDetailObj?.EHotelRoomDetailLst?.Where(d => d.RoomIndex.Equals(ObjEHotelPreBookingRequest.RoomIndex)).FirstOrDefault() ?? new EHotelRoomDetail();

                if (EHotelRoomDetailObj != null && EHotelRoomDetailObj.RoomIndex > -1)
                {
                    var HotelExtraInfo = (EHotelRoomDetailObj.ExtraInfo ?? string.Empty).Split('$').ToList();

                    IEnumerable<string> suppliers = Enumerable.Empty<string>();
                    if (!string.IsNullOrWhiteSpace(EHotelRoomDetailObj.Supplier))
                    {
                        suppliers = (EHotelRoomDetailObj.Supplier ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
                    }

                    await ForEachAsync(suppliers, DefaultMaxConcurrency, async sreq =>
                    {
                        if (sreq.Equals(AppConfig.GetConfigValue("Hotel:Contracted:SupplierName")))
                        {
                            // Local API handling placeholder - kept empty as original
                            return;
                        }
                        else if (sreq.Equals(AppConfig.GetConfigValue("Hotel:HotelBed:SupplierName")))
                        {
                            if (APICredentialsObj.ContainsKey((int)HotelApiSupplier.HBAPIID))
                            {
                                var markup = APIMarkupClass.ContainsKey((int)HotelApiSupplier.HBAPIID) ? APIMarkupClass[(int)HotelApiSupplier.HBAPIID] : new MarkupClass();
                                var creds = APICredentialsObj[(int)HotelApiSupplier.HBAPIID];
                                var res = await _ihotelBeds.GetHotelBedsApiPreBookingResponseAsync(ObjEHotelPreBookingRequest, HotelExtraInfo, creds, markup, ClientWiseBookingFolder, BookingMasterID, EHotelRoomDetailObj, EHotelDetailResponseObj?.Request ?? new EHotelDetailRequest()).ConfigureAwait(false);
                                if (res != null)
                                    APIPreBookingInfo_.Add(res);
                            }
                        }
                        else if (sreq.Equals(AppConfig.GetConfigValue("Hotel:TBO:SupplierName")))
                        {
                            if (APICredentialsObj.ContainsKey((int)HotelApiSupplier.TBAPIID))
                            {
                                var markup = APIMarkupClass.ContainsKey((int)HotelApiSupplier.TBAPIID) ? APIMarkupClass[(int)HotelApiSupplier.TBAPIID] : new MarkupClass();
                                var creds = APICredentialsObj[(int)HotelApiSupplier.TBAPIID];
                                var res = await _iTBOAPI.GetTboApiPreBookingResponseAsync(ObjEHotelPreBookingRequest, HotelExtraInfo, creds, markup, ClientWiseBookingFolder, BookingMasterID, EHotelRoomDetailObj, EHotelDetailResponseObj?.Request ?? new EHotelDetailRequest()).ConfigureAwait(false);
                                if (res != null)
                                    APIPreBookingInfo_.Add(res);
                            }
                        }
                    }).ConfigureAwait(false);

                    if (APIPreBookingInfo_ != null && APIPreBookingInfo_.Count > 0)
                    {
                        var APIPreBookingInfo = APIPreBookingInfo_.FirstOrDefault();
                        if (APIPreBookingInfo != null)
                        {
                            EHotelPreBookingResponseObj.AvailableForBook = APIPreBookingInfo.AvailableForBook;
                            EHotelPreBookingResponseObj.After_Rate = APIPreBookingInfo.After_Rate;
                            EHotelPreBookingResponseObj.Before_Rate = APIPreBookingInfo.Before_Rate;
                            EHotelPreBookingResponseObj.IsPriceChange = APIPreBookingInfo.IsPriceChange;
                            EHotelPreBookingResponseObj.Norms = APIPreBookingInfo.Norms;
                            EHotelPreBookingResponseObj.RequestLog = APIPreBookingInfo.RequestLog;
                            EHotelPreBookingResponseObj.ResponseLog = APIPreBookingInfo.ResponseLog;
                            EHotelPreBookingResponseObj.CancellationPolicy = APIPreBookingInfo.CancellationPolicy;
                            EHotelPreBookingResponseObj.CancellationDate = APIPreBookingInfo.CancellationDate;
                        }
                    }
                }
            }
            // Future supplier implementations can be added here
            return EHotelPreBookingResponseObj;
        }

        public async Task<EHotelBookingResponse> BookHotel(EHotelBookingRequest ObjEHotelBookingRequest)
        {
            var Result = new EHotelBookingResponse(ObjEHotelBookingRequest);
            string EBtoken = Guid.NewGuid().ToString();
            string ClientWiseBookingFolder = Path.Combine(Base_FolderURL + ObjEHotelBookingRequest.ClientID + "\\BookHotel\\" + EBtoken + "\\") + Path.DirectorySeparatorChar;
            if (!Directory.Exists(ClientWiseBookingFolder)) { Directory.CreateDirectory(ClientWiseBookingFolder); }
            string Detail_FolderURL = Path.Combine(Base_FolderURL + ObjEHotelBookingRequest.ClientID + "\\DetailHotel\\") + Path.DirectorySeparatorChar;
            string _HotelDetailFilePath = Path.Combine(Detail_FolderURL + ObjEHotelBookingRequest.HRDID + ".txt") + Path.DirectorySeparatorChar; ;
            string _HotelDetailFilePath_Content = File.ReadAllText(_HotelDetailFilePath);
            EHotelDetailResponse EHotelDetailResponseObj = JsonConvert.DeserializeObject<EHotelDetailResponse>(_HotelDetailFilePath_Content ?? "") ?? new EHotelDetailResponse();
            if (EHotelDetailResponseObj != null && !string.IsNullOrEmpty(EHotelDetailResponseObj.HRDID))
            {
                EHotelDetail EHotelDetailObj = EHotelDetailResponseObj.ResponseData;
                EHotelRoomDetail EHotelRoomDetailObj = EHotelDetailObj?.EHotelRoomDetailLst?.Where(d => d.RoomIndex.Equals(ObjEHotelBookingRequest.RoomIndex)).FirstOrDefault() ?? new EHotelRoomDetail();
                if (EHotelRoomDetailObj != null && EHotelRoomDetailObj.RoomIndex > -1)
                {
                    var APICredentialsObj = await _iapiCredentials.GetAPICredentialsByApiKey(ObjEHotelBookingRequest.ClientID, ObjEHotelBookingRequest.ApiKey ?? string.Empty, _hotelApiSetting.ServiceTypeID, IsLive, ObjEHotelBookingRequest.DBConnectioString ?? string.Empty).ConfigureAwait(false);
                    var MainPaxInfo = ObjEHotelBookingRequest.RoomWisePaxInfo.Where(c => c.Priority).FirstOrDefault();
                    var MainPax = ObjEHotelBookingRequest.RoomWisePaxInfo.Where(c => c.Priority).Select(d => string.Format("{0} {1}", d.FirstName, d.LastName)).FirstOrDefault();
                    var BookingEmailID = (MainPaxInfo != null && !string.IsNullOrEmpty(MainPaxInfo.Email)) ? MainPaxInfo.Email : string.Empty;
                    var BookingMobileCode = (MainPaxInfo != null && !string.IsNullOrEmpty(MainPaxInfo.MobileCode)) ? MainPaxInfo.MobileCode : string.Empty;
                    var BookingMobileNo = (MainPaxInfo != null && !string.IsNullOrEmpty(MainPaxInfo.MobileNo)) ? MainPaxInfo.MobileNo : string.Empty;
                    var roomwiseAges = new Dictionary<int, List<int>>();
                    var BookingUniqID = DateTime.Now.ToString("yyyyMMddhhmmssffff", System.Globalization.CultureInfo.InvariantCulture);
                    int BookingMasterID = ObjEHotelBookingRequest.BookingID;

                    foreach (var item in EHotelDetailResponseObj.Request.RoomOccupancyDetails ?? new List<ERoomOccupancyInfo>())
                    {
                        if (!roomwiseAges.ContainsKey(item.RoomNo))
                        {
                            roomwiseAges.Add(item.RoomNo, item.ChildAges);
                        }
                    }

                    var RoomWisePaxDetailsLst = new List<RoomWisePaxDetails>();

                    var roomgp = from item in ObjEHotelBookingRequest.RoomWisePaxInfo
                                 group item by
                                 new
                                 {
                                     RoomNo = item.RoomNo
                                 } into rgroup
                                 select rgroup;
                    foreach (var im in roomgp)
                    {
                        var Ages = roomwiseAges.ContainsKey(im.Key.RoomNo) ? roomwiseAges[im.Key.RoomNo] : null;

                        int ageindex = 0;
                        foreach (var ig in im)
                        {
                            var nig = ig;
                            if (nig.PaxType != BookingPaxType.Adult)
                            {
                                if (Ages != null)
                                {
                                    if (Ages.Count > ageindex)
                                    {
                                        nig.Age = Ages[ageindex];
                                    }
                                }
                                ageindex++;
                            }
                            RoomWisePaxDetailsLst.Add(nig);
                        }
                    }
                    ObjEHotelBookingRequest.RoomWisePaxInfo = RoomWisePaxDetailsLst;

                    var APIBookingInfo_ = new ConcurrentBag<APIBookingInfo>();
                    IEnumerable<string> suppliers = Enumerable.Empty<string>();
                    if (!string.IsNullOrWhiteSpace(EHotelRoomDetailObj.Supplier))
                    {
                        suppliers = (EHotelRoomDetailObj.Supplier ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
                    }

                    await ForEachAsync(suppliers, DefaultMaxConcurrency, async sreq =>
                    {
                        if (sreq.Equals(AppConfig.GetConfigValue("Hotel:Contracted:SupplierName")))
                        {
                            // Local API handling placeholder - kept empty as original
                            return;
                        }
                        else if (sreq.Equals(AppConfig.GetConfigValue("Hotel:HotelBed:SupplierName")))
                        {
                            if (APICredentialsObj.ContainsKey((int)HotelApiSupplier.HBAPIID))
                            {
                                var creds = APICredentialsObj[(int)HotelApiSupplier.HBAPIID];
                                var res = await _ihotelBeds.GetHotelBedsApiBookingResponseAsync(creds, ObjEHotelBookingRequest, EHotelDetailResponseObj.Request, EHotelRoomDetailObj, ClientWiseBookingFolder).ConfigureAwait(false);
                                if (res != null)
                                    APIBookingInfo_.Add(res);
                            }
                        }
                        else if (sreq.Equals(AppConfig.GetConfigValue("Hotel:TBO:SupplierName")))
                        {
                            if (APICredentialsObj.ContainsKey((int)HotelApiSupplier.TBAPIID))
                            {
                                var creds = APICredentialsObj[(int)HotelApiSupplier.TBAPIID];
                                var res = await _iTBOAPI.GetTboApiBookingResponseAsync(creds, ObjEHotelBookingRequest, EHotelDetailResponseObj.Request, EHotelRoomDetailObj, ClientWiseBookingFolder).ConfigureAwait(false);
                                if (res != null)
                                    APIBookingInfo_.Add(res);
                            }
                        }
                    }).ConfigureAwait(false);

                    if (APIBookingInfo_ != null && APIBookingInfo_.Count() > 0)
                    {
                        var APIBookingInfo = APIBookingInfo_.FirstOrDefault();
                        if (APIBookingInfo != null)
                        {
                            Result.BookingCost = APIBookingInfo.BookingCost;
                            Result.BookingStatus = (B2BEnum.BookingStatus)(int)APIBookingInfo.BookingStatus;
                            Result.ConfirmationNo = APIBookingInfo.ConfirmationNo;
                            Result.RequestLog = APIBookingInfo.RequestLog;
                            Result.ResponseLog = APIBookingInfo.ResponseLog;
                            Result.EBtoken = EBtoken;
                            Result.SupplierInfo = APIBookingInfo.SupplierInfo;
                            Result.SubConfirmationNo = APIBookingInfo.SubConfirmationNo;
                        }
                    }
                }
            }

            var bookingLogPath = Path.Combine(ClientWiseBookingFolder, "Bookinglog_" + Result.EBtoken.Replace('-', '_') + ".txt");
            await File.WriteAllTextAsync(bookingLogPath, JsonConvert.SerializeObject(Result)).ConfigureAwait(false);
            return Result;
        }

        public async Task<EHotelCancelResponse> CancelHotel(EHotelCancelRequest ObjEHotelCancelRequest)
        {
            var Result = new EHotelCancelResponse(ObjEHotelCancelRequest);
            string ClientWiseBookingFolder = Path.Combine(Base_FolderURL ?? string.Empty, ObjEHotelCancelRequest.ClientID.ToString(), "CancelHotel", ObjEHotelCancelRequest.BToken ?? string.Empty);
            if (!string.IsNullOrEmpty(ObjEHotelCancelRequest.BToken))
            {
                var APICredentialsObj = await _iapiCredentials.GetAPICredentialsByApiKey(ObjEHotelCancelRequest.ClientID, ObjEHotelCancelRequest.ApiKey ?? string.Empty, _hotelApiSetting.ServiceTypeID, IsLive, ObjEHotelCancelRequest.DBConnectioString ?? string.Empty).ConfigureAwait(false);
                var bookingLogPath = Path.Combine(ClientWiseBookingFolder, "Bookinglog_" + ObjEHotelCancelRequest.BToken.Replace('-', '_') + ".txt");
                if (!File.Exists(bookingLogPath)) return Result;

                string BookingInfo_Str = await File.ReadAllTextAsync(bookingLogPath).ConfigureAwait(false);
                var EHotelBookingResponseObj = JsonConvert.DeserializeObject<EHotelBookingResponse>(BookingInfo_Str);
                if (EHotelBookingResponseObj != null && EHotelBookingResponseObj.BookingStatus.Equals(BookingStatus.BookingSuccessful))
                {
                    var APICancelationInfo_ = new ConcurrentBag<APICancelationInfo>();
                    string SupplierKey = EHotelBookingResponseObj.SupplierInfo;
                    string bookingReferenceinfo = EHotelBookingResponseObj.ConfirmationNo;
                    await ForEachAsync(SupplierKey, DefaultMaxConcurrency, async sreq =>
                    {
                        if (sreq.Equals(AppConfig.GetConfigValue("Hotel:Contracted:SupplierName")))
                        {
                            // Local API handling placeholder - kept empty as original
                            return;
                        }
                        else if (sreq.Equals(AppConfig.GetConfigValue("Hotel:HotelBed:SupplierName")))
                        {
                            if (APICredentialsObj.ContainsKey((int)HotelApiSupplier.HBAPIID))
                            {
                                var creds = APICredentialsObj[(int)HotelApiSupplier.HBAPIID];
                                var res = await _ihotelBeds.GetHotelBedsApiCancelResponseAsync(creds, bookingReferenceinfo, ClientWiseBookingFolder, 0).ConfigureAwait(false);
                                if (res != null)
                                    APICancelationInfo_.Add(res);
                            }
                        }
                        else if (sreq.Equals(AppConfig.GetConfigValue("Hotel:TBO:SupplierName")))
                        {
                            if (APICredentialsObj.ContainsKey((int)HotelApiSupplier.TBAPIID))
                            {
                                var creds = APICredentialsObj[(int)HotelApiSupplier.TBAPIID];
                                var res = await _iTBOAPI.GetTboApiCancelResponseAsync(creds, bookingReferenceinfo, ClientWiseBookingFolder, 0).ConfigureAwait(false);
                                if (res != null)
                                    APICancelationInfo_.Add(res);
                            }
                        }
                    }).ConfigureAwait(false);

                    if (APICancelationInfo_ != null && APICancelationInfo_.Count() > 0)
                    {
                        var APICancelationInfo = APICancelationInfo_.FirstOrDefault();
                        if (APICancelationInfo != null)
                        {
                            Result = new EHotelCancelResponse(ObjEHotelCancelRequest, APICancelationInfo);
                        }
                    }
                }
            }
            return Result;
        }

        public async Task<EHotelDetailResponse> StaticDetailHotel(EHotelDetailRequest ObjEHotelDetailRequest)
        {
            if (ObjEHotelDetailRequest == null) throw new ArgumentNullException(nameof(ObjEHotelDetailRequest));

            var EHotelDetailResponseObj = new EHotelDetailResponse();
            var results = new ConcurrentBag<EHotelDetail>();

            string clientFolder = Path.Combine(Base_FolderURL ?? string.Empty, ObjEHotelDetailRequest.ClientID.ToString(), "DetailHotel");
            string detailsTraceId = Guid.NewGuid().ToString().Replace('-', '_');
            string searchTraceId = string.Concat(detailsTraceId, "_", "HotelDetail");
            string searchTraceFolder = Path.Combine(clientFolder, searchTraceId);

            var detailRequests = new List<EHotelDetailRequest>();
            var apiList = (ObjEHotelDetailRequest.HotelstringIds ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var extraInfoLst = (ObjEHotelDetailRequest.ExtraInfo ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.None);
            for (int i = 0; i < apiList.Length; i++)
            {
                var apic = apiList[i];
                var apiInfo = apic.Split('~');
                var apiValue = apiInfo.LastOrDefault();
                var extraInfoValue = extraInfoLst.Length > i ? extraInfoLst[i] : ObjEHotelDetailRequest.ExtraInfo;

                var sreq = new EHotelDetailRequest(ObjEHotelDetailRequest)
                {
                    HotelstringIds = apic,
                    HotelId = apiValue,
                    ExtraInfo = extraInfoValue
                };

                detailRequests.Add(sreq);
            }

            await ForEachAsync(detailRequests, DefaultMaxConcurrency, async sreq =>
            {
                if (string.IsNullOrEmpty(sreq.HotelstringIds))
                    return;

                var apiInfo = sreq.HotelstringIds.Split('~');
                var apiKey = apiInfo.FirstOrDefault();

                if (apiKey != null && apiKey.Equals(AppConfig.GetConfigValue("Hotel:Contracted:APIName"), StringComparison.OrdinalIgnoreCase))
                {
                    // Local API handling placeholder - kept empty as original
                    return;
                }
                else if (apiKey != null && apiKey.Equals(AppConfig.GetConfigValue("Hotel:HotelBed:APIName"), StringComparison.OrdinalIgnoreCase))
                {
                    var client = new MongoDbCRUD(APIKeySettings.HB_Database);
                    var db = await client.LoadHotelData<HotelBedsModelClass.HotelBedsHotelDetail.Hotel>(APIKeySettings.HB_Table, "code", sreq.HotelId ?? string.Empty).ConfigureAwait(false);
                    results.Add(new EHotelDetail(db.FirstOrDefault() ?? new HotelBedsModelClass.HotelBedsHotelDetail.Hotel(), new List<EHotelRoomDetail>(), string.Empty, string.Empty));
                }
                else if (apiKey != null && apiKey.Equals(AppConfig.GetConfigValue("Hotel:TBO:APIName"), StringComparison.OrdinalIgnoreCase))
                {
                    var client = new MongoDbCRUD(APIKeySettings.TBO_Database);
                    var db = await client.LoadHotelData<TBOModelClass.Tbov7HotelGiataHotelCodeList.Hotel>(APIKeySettings.TBO_Table, "code", sreq.HotelId ?? string.Empty).ConfigureAwait(false);
                    results.Add(new EHotelDetail(db.FirstOrDefault() ?? new TBOModelClass.Tbov7HotelGiataHotelCodeList.Hotel(), new List<EHotelRoomDetail>(), string.Empty, string.Empty));
                }

            }).ConfigureAwait(false);

            EHotelDetailResponseObj.SearchTraceKey = ObjEHotelDetailRequest.SearchTraceKey ?? string.Empty;
            EHotelDetailResponseObj.HRDID = detailsTraceId;
            EHotelDetailResponseObj.Request = ObjEHotelDetailRequest;
            EHotelDetailResponseObj.ResponseCode = Status.successful;
            EHotelDetailResponseObj.ResponseData = new EHotelDetail(results.ToList(), ObjEHotelDetailRequest);
            EHotelDetailResponseObj.ResponseMeassage = Status.successful.ToString();

            return EHotelDetailResponseObj;
        }
    }
}
