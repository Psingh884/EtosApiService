using EtosApiServices.Data.Library.Common;
using EtosApiServices.Hotel.Library.Contract;
using EtosApiServices.Hotel.Model;
using EtosApiServices.Model.Setting;
using EtosApiServices.MongoDb.Library;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static EtosApiServices.Hotel.Model.HotelBedsAvailabilityResponse;
using static EtosApiServices.Hotel.Model.HotelBedsFacility;
using static EtosApiServices.Hotel.Model.TBOModelClass.Tbov7HotelDetailsResponseInfo;

namespace EtosApiServices.Hotel.Library.Services
{
    public class TBOAPI : ITBOAPI
    {
        private readonly TboApiRequest _tboApiRequest;
        private readonly IAPICredentials _iapiCredentials;
        readonly HotelApiSetting _hotelApiSetting;
        public readonly string APIName = AppConfig.GetConfigValue("Hotel:TBO:APIName");
        public readonly int ApiSupplierId = int.Parse(AppConfig.GetConfigValue("Hotel:TBO:SupplierId"));
        public readonly string APISupplierName = AppConfig.GetConfigValue("Hotel:TBO:SupplierName");
        private static readonly TBOModelClass.Tbov7HotelGiataHotelCodeList.Hotel s_emptyHotel = new TBOModelClass.Tbov7HotelGiataHotelCodeList.Hotel();

        public TBOAPI(TboApiRequest tboApiRequest, IAPICredentials aPICredentials, IOptions<HotelApiSetting> hotelApiSetting)
        {
            _tboApiRequest = tboApiRequest ?? throw new ArgumentException(nameof(tboApiRequest));
            _iapiCredentials = aPICredentials ?? throw new ArgumentException(nameof(aPICredentials));
            _hotelApiSetting = hotelApiSetting?.Value ?? throw new ArgumentException(nameof(hotelApiSetting));
        }

        public async Task<List<ETSearchHotel>> GetTboApiHotelResponseAsync(EHotelSearchRequest SearchReq, Dictionary<string, string> ApiCredentials, string FolderName, MarkupClass MarkupClassObj)
        {
            var resultList = new List<ETSearchHotel>();

            if (SearchReq?.DestinationInfo == null)
                return resultList;

            // Get list of hotels from MongoDB
            var client = new MongoDbCRUD(APIKeySettings.TBO_Database);
            var db = await client.GetTBOHotelInfoByLatLngV1(APIKeySettings.TBO_Table, SearchReq.DestinationInfo.lat, SearchReq.DestinationInfo.lng, SearchReq.DestinationInfo.RadiusinMeter).ConfigureAwait(false);
            var TBOHotelList = db ?? new List<TBOModelClass.Tbov7HotelGiataHotelCodeList.Hotel>();
            var staticData = TBOHotelList?.GroupBy(d => d.HotelCode).ToDictionary(g => g.Key, g => g.First()) ?? new Dictionary<string?, TBOModelClass.Tbov7HotelGiataHotelCodeList.Hotel>();

            // Create chunks of hotel ids (as the existing helper does) and run requests with bounded concurrency
            var hotelChunks = AppConfig.GetSetOfHotelIds<TBOModelClass.Tbov7HotelGiataHotelCodeList.Hotel>(TBOHotelList ?? new List<TBOModelClass.Tbov7HotelGiataHotelCodeList.Hotel>(), 200)?.ToList() ?? new List<List<TBOModelClass.Tbov7HotelGiataHotelCodeList.Hotel>>();
            var responses = new ConcurrentBag<TBOHotelSearchResponse.SearchResponse>();
            const int maxDegreeOfParallelism = 8;

            // Run API calls in parallel but bounded to avoid overwhelming the downstream API
            await Parallel.ForEachAsync(hotelChunks, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, async (chunk, ct) =>
            {
                if (chunk == null || chunk.Count == 0)
                    return;

                // Do not mutate the incoming SearchReq. Pass hotel ids as HotelID parameter instead.
                var hotelIdsCsv = string.Join(",", chunk.Select(d => d.HotelCode ?? string.Empty));
                try
                {
                    var resp = await _tboApiRequest.GetTboApiHotelResponseAsync(SearchReq, ApiCredentials, hotelIdsCsv, FolderName).ConfigureAwait(false);
                    if (resp != null)
                    {
                        responses.Add(resp);
                    }
                }
                catch
                {
                    // Swallow individual errors to allow other chunks to proceed. Consider logging if available.
                }
            }).ConfigureAwait(false);


            if (responses.Count > 0)
            {
                // Flatten results once
                var merged = responses.SelectMany((TBOHotelSearchResponse.SearchResponse resp) => resp.HotelResult ?? Enumerable.Empty<TBOHotelSearchResponse.HotelResult>()).ToList();

                if (merged.Count == 0)
                    return resultList;


                var apiCurrency = merged[0]?.Currency ?? string.Empty;

                // Fetch currency rates in parallel
                var taskApiRoe = _iapiCredentials.GetCurrancyRate(apiCurrency, SearchReq.ClientID, SearchReq.DBConnectioString ?? string.Empty);
                var taskUserRoe = _iapiCredentials.GetCurrancyRate(SearchReq.CurrencyCode ?? string.Empty, SearchReq.ClientID, SearchReq.DBConnectioString ?? string.Empty);
                await Task.WhenAll(taskApiRoe, taskUserRoe).ConfigureAwait(false);
                var apiRoe = taskApiRoe.Result;
                var userRoe = taskUserRoe.Result;

                foreach (var hitem in merged)
                {
                    staticData.TryGetValue(hitem.HotelCode, out var hbHotelDetail);
                    var etHotel = new ETHotelSearchList((int)HotelApiSupplier.TBAPIID, APIName ?? string.Empty, "", hitem, hbHotelDetail ?? s_emptyHotel, SearchReq, MarkupClassObj, apiRoe, userRoe, 0);
                    resultList.Add(etHotel);
                }
            }
            return resultList;
        }

        public async Task<EHotelDetail> GetTboApiDetailResponseAsync(EHotelDetailRequest DetailReq, Dictionary<string, string> ApiCredentials, string FolderName, string BHRDID, MarkupClass MarkupClassObj)
        {
            var HotelObj = new EHotelDetail();
            if (DetailReq == null) { return HotelObj; }

            var Tboresp = await _tboApiRequest.GetTboApiHotelDetailsResponseAsync(DetailReq, ApiCredentials, FolderName).ConfigureAwait(false);
            if (Tboresp != null && Tboresp.HotelResult != null && Tboresp.HotelResult.Count() > 0)
            {
                var client = new MongoDbCRUD(APIKeySettings.TBO_Database);
                var db = await client.LoadHotelData<TBOModelClass.Tbov7HotelGiataHotelCodeList.Hotel>(APIKeySettings.TBO_Table, "HotelCode", DetailReq.HotelId ?? string.Empty).ConfigureAwait(false);
                var TBOHotelList = db ?? new List<TBOModelClass.Tbov7HotelGiataHotelCodeList.Hotel>();
                var staticData = TBOHotelList?.GroupBy(d => d.HotelCode).ToDictionary(g => g.Key, g => g.First()) ?? new Dictionary<string?, TBOModelClass.Tbov7HotelGiataHotelCodeList.Hotel>();

                var apiCurrency = Tboresp.HotelResult[0]?.Currency ?? string.Empty;

                // Fetch currency rates in parallel
                var taskApiRoe = _iapiCredentials.GetCurrancyRate(apiCurrency, DetailReq.ClientID, DetailReq.DBConnectioString ?? string.Empty);
                var taskUserRoe = _iapiCredentials.GetCurrancyRate(DetailReq.CurrencyCode ?? string.Empty, DetailReq.ClientID, DetailReq.DBConnectioString ?? string.Empty);
                await Task.WhenAll(taskApiRoe, taskUserRoe).ConfigureAwait(false);
                var apiRoe = taskApiRoe.Result;
                var userRoe = taskUserRoe.Result;

                foreach (var hotel in Tboresp.HotelResult)
                {
                    var hotelDetail = staticData.Values.FirstOrDefault();
                    HotelObj.Name = hotelDetail?.HotelName ?? string.Empty;
                    HotelObj.Address = hotelDetail?.HotelAddress ?? string.Empty;
                    HotelObj.location = DetailReq?.DestinationInfo?.CityShortName ?? string.Empty;
                    int Rating = 0;
                    if (!string.IsNullOrEmpty(hotelDetail?.StarRating))
                    {
                        Rating = (int)Enum.Parse(typeof(TboApiv7StarRating), hotelDetail.StarRating);
                    }
                    HotelObj.StarRating = Convert.ToString(Rating);
                    HotelObj.HotelCode = hotelDetail?.HotelCode ?? string.Empty;
                    HotelObj.latitude = hotelDetail?.Latitude ?? string.Empty;
                    HotelObj.longitude = hotelDetail?.Longitude ?? string.Empty;
                    HotelObj.CityName = DetailReq?.DestinationInfo?.CityShortName ?? string.Empty;

                    HotelObj.Supplier = APIName;
                    HotelObj.HotelString = APIName + "~" + hotelDetail?.HotelCode ?? string.Empty;
                    HotelObj.Is_Offer = "0";
                    HotelObj.Property_Type = "Hotel";
                    HotelObj.Description = hotelDetail?.HotelDetails?.Description ?? string.Empty;

                    var EAminitiesLst = new List<EAminities>();
                    var EHotelImageGallaryLst = new List<EHotelImageGallary>();
                    if (hotelDetail != null && hotelDetail.HotelDetails != null)
                    {
                        if (hotelDetail.HotelDetails.ImageUrls != null && hotelDetail.HotelDetails.ImageUrls.ImageUrl != null && hotelDetail.HotelDetails.ImageUrls.ImageUrl.Count != 0)
                        {
                            foreach (var img in hotelDetail.HotelDetails.ImageUrls.ImageUrl)
                            {
                                var EHotelImageGallaryObj = new EHotelImageGallary()
                                {
                                    HotelId = Convert.ToString(hotelDetail.HotelCode),
                                    HotelName = Convert.ToString(hotelDetail.HotelName),
                                    ImageActualPath = Convert.ToString(img),
                                    ImageName = Convert.ToString(hotelDetail.HotelName),
                                    ImageThumPath = Convert.ToString(img)
                                };
                                EHotelImageGallaryLst.Add(EHotelImageGallaryObj);
                            }
                        }
                        else
                        {
                            var EHotelImageGallaryObj = new EHotelImageGallary()
                            {
                                HotelId = Convert.ToString(hotelDetail.HotelCode),
                                HotelName = Convert.ToString(hotelDetail.HotelName),
                                //ImageActualPath = Convert.ToString(objHotelInfo.HotelPicture),
                                ImageName = Convert.ToString(hotelDetail.HotelName),
                                //ImageThumPath = Convert.ToString(objHotelInfo.HotelPicture)
                            };
                            EHotelImageGallaryLst.Add(EHotelImageGallaryObj);
                        }
                        if (hotelDetail.HotelDetails.HotelFacilities != null && hotelDetail.HotelDetails.HotelFacilities.HotelFacility != null && hotelDetail.HotelDetails.HotelFacilities.HotelFacility.Count != 0)
                        {
                            foreach (var fac in hotelDetail.HotelDetails.HotelFacilities.HotelFacility)
                            {
                                var EAminitiesobj = new EAminities()
                                {
                                    AminitieName = fac,
                                    AminitieCategory = fac,
                                    topAminities = 0,
                                    fcid = 0,
                                    img = string.Empty
                                };
                                EAminitiesLst.Add(EAminitiesobj);
                            }
                        }

                    }
                    HotelObj.EHotelImageGallaryLst = EHotelImageGallaryLst;
                    HotelObj.EAminitiesLst = EAminitiesLst;

                    HotelObj.HotelURL = "";
                    HotelObj.HotelDetail = "";
                    HotelObj.HDType = HDType.Normal;
                    HotelObj.GiataID = "0";
                    HotelObj.GiataCityID = "0";
                    HotelObj.CheckInTime = "12:00";
                    HotelObj.CheckOutTime = "12:00";
                    HotelObj.CurrentDeals = string.Empty;
                    HotelObj.TotalTax = 0;

                    string APICurrencyName = hotel.Currency ?? string.Empty;
                    HotelObj.Currency = APICurrencyName;

                    var EHotelRoomDetailLst = new List<EHotelRoomDetail>();
                    foreach (var hotelroom in hotel.Rooms ?? new List<TBOHotelSearchResponse.Room>())
                    {
                        var HotelRoomType_ = hotelroom?.Name?.FirstOrDefault();
                        var RateCalculation = new Dictionary<string, object>();
                        var EHotelRoomDetailobj = new EHotelRoomDetail();
                        EHotelRoomDetailobj.BHRDID = BHRDID;
                        EHotelRoomDetailobj.ServiceAPIID = (int)HotelApiSupplier.TBAPIID;
                        EHotelRoomDetailobj.RoomBasicClassType = HotelRoomType_ ?? string.Empty;
                        EHotelRoomDetailobj.RoomIndex = 0;
                        EHotelRoomDetailobj.RoomNo = DetailReq?.TotalNoRoom ?? 1;
                        EHotelRoomDetailobj.HotelID = hotelDetail?.HotelCode ?? string.Empty;
                        EHotelRoomDetailobj.RoomType = HotelRoomType_ ?? string.Empty;
                        EHotelRoomDetailobj.RoomTypeId = HotelRoomType_ ?? string.Empty;
                        EHotelRoomDetailobj.RoomMeals = hotelroom?.MealType ?? string.Empty;
                        EHotelRoomDetailobj.RoomMealsID = "0";
                        EHotelRoomDetailobj.RoomOccupancyDetails = DetailReq?.RoomOccupancyDetails ?? new List<ERoomOccupancyInfo>();
                        if (hotelroom?.RoomPromotion != null && hotelroom.RoomPromotion != null)
                        {
                            EHotelRoomDetailobj.Errata = string.Join("<br>", hotelroom.RoomPromotion);
                        }
                        decimal DefaultAllNightRate = 0, AllNightTotalRate = 0;
                        decimal RoomRatePer = (Convert.ToDecimal(hotelroom?.TotalFare) / DetailReq.Nights) / DetailReq.TotalNoRoom;
                        RoomRatePer = (RoomRatePer / apiRoe) * userRoe;
                        decimal AllNightOfferTotalRate = Convert.ToDecimal(hotelroom?.TotalFare); //* NoOfRooms;
                        AllNightOfferTotalRate = (AllNightOfferTotalRate / apiRoe) * userRoe;
                        decimal Discount = 0;
                        Discount = (Discount / apiRoe) * userRoe;
                        EHotelRoomDetailobj.IsOffer = (Discount > 0 ? true : false);

                        string SuuplementStr = string.Empty;
                        if (hotelroom?.Supplements != null)
                        {
                            SuuplementStr += await GetSupplimentDetail_JSON(hotelroom.Supplements, DetailReq.CurrencyCode, apiRoe, userRoe).ConfigureAwait(false);
                        }

                        AllNightTotalRate = AllNightOfferTotalRate + Discount;
                        var DayWiseRatesLst = new List<EDateWiseRoomRate>();
                        for (int k = 0; k < DetailReq.Nights; k++)
                        {
                            var OneDayRate = RoomRatePer;
                            var DicRates1sd = APIMarkupSection.AddMarkupByRule(ApiSupplierId, Convert.ToDecimal(OneDayRate), MarkupClassObj, APICurrencyName, 1, DetailReq.TotalNoRoom);
                            DayWiseRatesLst.Add(new EDateWiseRoomRate() { Date = DetailReq.CheckInDate_.AddDays(k).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), Rate = Convert.ToDecimal((Convert.ToDouble(DicRates1sd[APIMarkupSection.AR]))) });
                        }

                        var DicRates1 = APIMarkupSection.AddMarkupByRule(ApiSupplierId, Convert.ToDecimal(AllNightOfferTotalRate), MarkupClassObj, APICurrencyName, DetailReq.Nights, DetailReq.TotalNoRoom);
                        AllNightOfferTotalRate = (Convert.ToDecimal(DicRates1[APIMarkupSection.AR]));
                        DefaultAllNightRate = AllNightOfferTotalRate;

                        var DicAllNightTotalRate = APIMarkupSection.AddMarkupByRule(ApiSupplierId, Convert.ToDecimal(AllNightTotalRate), MarkupClassObj, APICurrencyName, DetailReq.Nights, DetailReq.TotalNoRoom);
                        AllNightTotalRate = (Convert.ToDecimal(DicAllNightTotalRate[APIMarkupSection.AR]));
                        EHotelRoomDetailobj.TotalTax = ((Convert.ToDecimal(DicAllNightTotalRate[APIMarkupSection.TXV])));
                        EHotelRoomDetailobj.IsTaxType = Convert.ToBoolean(DicAllNightTotalRate[APIMarkupSection.TXO]); Discount = Convert.ToDecimal(AllNightTotalRate) - Convert.ToDecimal(DefaultAllNightRate);
                        RateCalculation.Add(ContractedHotelConstant.Base_Room_Rate, Convert.ToDecimal(DicAllNightTotalRate[APIMarkupSection.AR]) - EHotelRoomDetailobj.TotalTax);//AP Cost
                        decimal DefaultTotalRate = Convert.ToDecimal(hotelroom.TotalFare);

                        int SupplierMasterID = ApiSupplierId;
                        ESupplierRoomDefaultInfo SupplierRoomDefaultInfoObj = new ESupplierRoomDefaultInfo();
                        SupplierRoomDefaultInfoObj.Supplier_AllNightOfferRate = Convert.ToDecimal(hotelroom.TotalFare);
                        SupplierRoomDefaultInfoObj.Supplier_AllNightRate = Convert.ToDecimal(hotelroom.TotalFare) + Discount;
                        SupplierRoomDefaultInfoObj.Supplier_Currency = APICurrencyName;
                        SupplierRoomDefaultInfoObj.Supplier_ID = SupplierMasterID;
                        SupplierRoomDefaultInfoObj.Supplier_MinNightRate = Convert.ToDecimal(hotelroom.TotalFare);
                        SupplierRoomDefaultInfoObj.Supplier_Name = APISupplierName;
                        SupplierRoomDefaultInfoObj.SupplierCode = APIName;

                        EHotelRoomDetailobj.Currency = DetailReq.CurrencyCode ?? string.Empty;
                        EHotelRoomDetailobj.Supplier = APISupplierName;
                        EHotelRoomDetailobj.IsSupplier = true;
                        EHotelRoomDetailobj.SupplierName = APISupplierName;

                        string LancellationPolicyStr = string.Empty;
                        int ii = 0;
                        if (hotelroom.CancelPolicies != null)
                        {
                            foreach (var policy in hotelroom.CancelPolicies ?? new List<TBOHotelSearchResponse.CancelPolicy>())
                            {
                                string fromDateStr = policy.FromDate ?? string.Empty;
                                DateTime fromDate = DateTime.ParseExact(fromDateStr, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                if (DetailReq.TotalNoRoom > 1) { LancellationPolicyStr += "<b>Room " + (ii + 1) + "</b>:</br>"; }
                                double CancellationCharge = policy.CancellationCharge;


                                if (policy.ChargeType.Equals(Convert.ToString(TboApiv7CancellationChargeTypeForHotel.Fixed)))
                                {
                                    decimal _CancellationCharge = (Convert.ToDecimal(CancellationCharge) / apiRoe) * userRoe;
                                    LancellationPolicyStr += "From <b>" + fromDate.AddDays(_hotelApiSetting.FreeCancellationBufferPeriod).ToString("dd/MM/yyyy") + " </b>  <b>" + string.Empty + "</b> Cancellation Charge Will Be <b>" + DetailReq.CurrencyCode + " " + _CancellationCharge.ToString("0.00") + "<br/>";
                                }
                                if (policy.ChargeType.Equals(Convert.ToString(TboApiv7CancellationChargeTypeForHotel.Percentage)))
                                {
                                    LancellationPolicyStr += "<br/>From <b>" + fromDate.AddDays(_hotelApiSetting.FreeCancellationBufferPeriod).ToString("dd/MM/yyyy") + " </b>   <b>" + string.Empty + "</b> Cancellation Charge Will Be <b>" + CancellationCharge + "% Rate <br/>";
                                }

                                if (policy.ChargeType.Equals(Convert.ToString(TboApiv7CancellationChargeTypeForHotel.Night)))
                                {
                                    decimal _CancellationCharge = (Convert.ToDecimal(CancellationCharge) / apiRoe) * userRoe;
                                    LancellationPolicyStr += "<br />From <b>" + fromDate.AddDays(_hotelApiSetting.FreeCancellationBufferPeriod).ToString("dd/MM/yyyy") + " </b>   <b>" + string.Empty + "</b> Cancellation Charge Will Be <b>" + _CancellationCharge.ToString("0.00") + " nights <br/>";
                                }
                            }
                        }

                        EHotelRoomDetailobj.IsCanclePolicy = hotelroom?.CancelPolicies?.Count != 0;
                        EHotelRoomDetailobj.CanclePolicy = LancellationPolicyStr;
                        string fromDateStr_lst = hotelroom?.CancelPolicies?.Min(h => h.FromDate) ?? string.Empty;
                        var isFree = hotelroom?.CancelPolicies?.Where(x => Convert.ToDecimal(x.CancellationCharge).Equals(0)).Select(m => m).ToList();
                        EHotelRoomDetailobj.IsFreeCostCancellationDate = isFree?.Count != 0;

                        if (hotelroom?.CancelPolicies == null && hotelroom?.IsRefundable != null && hotelroom.IsRefundable)
                        {
                            fromDateStr_lst = DetailReq.CheckInDate_.ToString("dd-MM-yyyy HH:mm:ss");
                            EHotelRoomDetailobj.IsFreeCostCancellationDate = true;
                        }

                        DateTime lstCancel_fromDate = DateTime.ParseExact(fromDateStr_lst ?? string.Empty, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        DateTime LastCancellationDate = new DateTime();
                        LastCancellationDate = lstCancel_fromDate;

                        EHotelRoomDetailobj.FreeCostCancellationDate = LastCancellationDate.AddDays(_hotelApiSetting.FreeCancellationBufferPeriod).ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
                        EHotelRoomDetailobj.FreeCostCancellationDate = Convert.ToDateTime(EHotelRoomDetailobj.FreeCostCancellationDate).AddDays(-1).ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
                        if (DateTime.Now > Convert.ToDateTime(EHotelRoomDetailobj.FreeCostCancellationDate))
                        {
                            EHotelRoomDetailobj.IsFreeCostCancellationDate = false;
                        }
                        else
                        {
                            EHotelRoomDetailobj.IsFreeCostCancellationDate = true;
                        }

                        EHotelRoomDetailobj.Errata = SuuplementStr;
                        EHotelRoomDetailobj.IsErrata = SuuplementStr != string.Empty;

                        Dictionary<string, string> TBO_APIExtraInfo = new Dictionary<string, string>();
                        TBO_APIExtraInfo.Add("Inclusion", hotelroom?.Inclusion ?? string.Empty);
                        TBO_APIExtraInfo.Add("BookingCode", hotelroom?.BookingCode ?? string.Empty);
                        TBO_APIExtraInfo.Add("HotelCode", hotelDetail?.HotelCode ?? string.Empty);
                        TBO_APIExtraInfo.Add("HotelName", hotelDetail?.HotelName ?? string.Empty);
                        TBO_APIExtraInfo.Add("RoomIndex", "Roomindex_");
                        TBO_APIExtraInfo.Add("BHRDID", BHRDID);
                        EHotelRoomDetailobj.ExtraInfo = JsonConvert.SerializeObject(TBO_APIExtraInfo);

                        EHotelRoomDetailobj.AvilableRoom = DetailReq.TotalNoRoom;
                        EHotelRoomDetailobj.Issharedbedding = false;
                        EHotelRoomDetailobj.IsExtraBed = false;
                        EHotelRoomDetailobj.IsMealPlanUpgrad = false;
                        EViewCalculation ViewCalculation = new EViewCalculation()
                        {
                            Currency = DetailReq.CurrencyCode ?? string.Empty,
                            DiscountRate = (-Discount),
                            HotelName = Convert.ToString(string.Empty),
                            IsOffer = EHotelRoomDetailobj.IsOffer,
                            OfferStr = EHotelRoomDetailobj.OfferName,
                            OtherCharges = 0,
                            ROE = 1,
                            Rates = DayWiseRatesLst

                        };
                        EHotelRoomDetailobj.ViewCalculation = ViewCalculation;
                        EHotelRoomDetailobj.AllNightTotalRate = AllNightTotalRate;
                        EHotelRoomDetailobj.AllNigntOfferRate = AllNightOfferTotalRate;
                        EHotelRoomDetailobj.TotalRate = RoomRatePer;

                        List<EHotelRoomImages> HotelRoomImages = new List<EHotelRoomImages>();
                        EHotelRoomDetailobj.HotelRoomImages = HotelRoomImages;
                        EHotelRoomDetailobj.SupplierRoomDefaultInfo = SupplierRoomDefaultInfoObj;
                        EHotelRoomDetailobj.EMealPlanUpgradeInfoObj = new EMealPlanUpgradeInfo();
                        EHotelRoomDetailobj.GMXMealPlanUpgradeInfo = new GMXMealPlanUpgradeInfo();

                        ERoomDefaultInfo RoomDefaultInfoObj = new ERoomDefaultInfo();
                        RoomDefaultInfoObj.EHotelMarkupInfoLst = new List<EHotelMarkupInfo>();
                        RoomDefaultInfoObj.Purchase_CurrencyCode = hotel.Currency ?? string.Empty;
                        RoomDefaultInfoObj.Purchase_CurrencyID = 0;
                        RoomDefaultInfoObj.Purchase_CurrencyROE_DIV = Convert.ToDecimal("1");
                        RoomDefaultInfoObj.Purchase_CurrencyROE_MLP = Convert.ToDecimal("1");
                        RoomDefaultInfoObj.Purchase_FinalAllNightRate = Convert.ToDecimal(hotelroom?.TotalFare) + Discount;
                        RoomDefaultInfoObj.Supplier_AllNightOfferRate = Convert.ToDecimal(hotelroom?.TotalFare);
                        RoomDefaultInfoObj.Supplier_AllNightRate = Convert.ToDecimal(hotelroom?.TotalFare) + Discount;
                        RoomDefaultInfoObj.Supplier_Currency = hotel.Currency ?? string.Empty;
                        RoomDefaultInfoObj.System_AllNightOfferRate = Convert.ToDecimal(hotelroom?.TotalFare) + Discount;
                        RoomDefaultInfoObj.System_AllNightRate = Convert.ToDecimal(hotelroom?.TotalFare) + Discount;
                        RoomDefaultInfoObj.System_CurrencyCode = hotel.Currency ?? string.Empty;
                        RoomDefaultInfoObj.System_CurrencyID = 1;
                        RoomDefaultInfoObj.System_CurrencyROE_DIV = 1;
                        RoomDefaultInfoObj.System_CurrencyROE_MLP = 1;
                        RoomDefaultInfoObj.System_OneNightRate = RoomRatePer;
                        RoomDefaultInfoObj.System_TotalMarkup = 0;
                        RoomDefaultInfoObj.System_TotalTax = 0;
                        RoomDefaultInfoObj.DicAllNightRate = DicAllNightTotalRate;
                        RoomDefaultInfoObj.DicAllOfferTotalRate = DicRates1;
                        EHotelRoomDetailobj.RoomDefaultInfo = RoomDefaultInfoObj;
                        //RateCalculation
                        RateCalculation.Add(ContractedHotelConstant.Service_Charges, 0);
                        RateCalculation.Add(ContractedHotelConstant.Service_Charges_Value, "0");
                        RateCalculation.Add(ContractedHotelConstant.Service_Charges_CalType, "1");
                        RateCalculation.Add(ContractedHotelConstant.Extra_Fees, 0);
                        RateCalculation.Add(ContractedHotelConstant.Extra_Fees_Value, 0);
                        RateCalculation.Add(ContractedHotelConstant.Extra_Fees_CalType, "1");
                        RateCalculation.Add(ContractedHotelConstant.Extra_Fees_Desc, "Munipality Fees");
                        RateCalculation.Add(ContractedHotelConstant.Input_Tax, 0);
                        RateCalculation.Add(ContractedHotelConstant.Output_Tax, EHotelRoomDetailobj.TotalTax);
                        RateCalculation.Add(ContractedHotelConstant.Base_Room_Rate_OfferDiscount, Discount);
                        RateCalculation.Add(ContractedHotelConstant.Base_Room_Rate_OfferName, Convert.ToString(EHotelRoomDetailobj.OfferName));
                        EHotelRoomDetailobj.RateCalculation = RateCalculation;
                        //RateCalculation
                        EHotelRoomDetailobj.CheckInDate = DetailReq.CheckInDate_;
                        EHotelRoomDetailobj.CheckOutDate = DetailReq.CheckOutDate_;
                        EHotelRoomDetailLst.Add(EHotelRoomDetailobj);
                    }
                    HotelObj.MinOneRate = EHotelRoomDetailLst.Min(d => d.TotalRate).ToString();
                    HotelObj.AllNightRate = EHotelRoomDetailLst.Min(d => d.AllNigntOfferRate).ToString();
                    HotelObj.EHotelRoomDetailLst = EHotelRoomDetailLst;
                    HotelObj.OtherService = new System.Data.DataTable();
                }
            }

            return HotelObj;
        }

        public async Task<APIPreBookingInfo> GetTboApiPreBookingResponseAsync(EHotelPreBookingRequest ObjEHotelPreBookingRequest, List<string> HotelExtraInfo, Dictionary<string, string> ApiCredentials, MarkupClass MarkupClassObj, string FolderName, int BookingMasterID, EHotelRoomDetail EHotelRoomDetailObj, EHotelDetailRequest DetailReq)
        {
            var APIPreBookingInfoObj_ = new APIPreBookingInfo();

            var TBO_ExtraInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(HotelExtraInfo[0]);
            var PreBookingResponse = await _tboApiRequest.HotelCheckRate_Prebook(ApiCredentials, TBO_ExtraInfo ?? new Dictionary<string, string>(), FolderName);
            if (PreBookingResponse != null && PreBookingResponse.Status != null && PreBookingResponse.Status.Code.Equals(Convert.ToInt32(HttpStatusCode.OK)))
            {
                if (PreBookingResponse.HotelResult != null && PreBookingResponse.HotelResult.Count > 0)
                {
                    var RoomDe1 = await GetPreBookingHotelRoomDetail(EHotelRoomDetailObj, PreBookingResponse, DetailReq, DetailReq.HotelId ?? "", EHotelRoomDetailObj.BHRDID ?? "", MarkupClassObj, FolderName).ConfigureAwait(false);
                    if (RoomDe1 != null)
                    {
                        APIPreBookingInfoObj_.AvailableForBook = true;
                        if (RoomDe1.AllNigntOfferRate.ToString("0.00") != Convert.ToDecimal(RoomDe1.AllNightTotalRate).ToString("0.00"))
                        {
                            APIPreBookingInfoObj_.After_Rate = Convert.ToDecimal(RoomDe1.AllNightTotalRate);
                            APIPreBookingInfoObj_.Before_Rate = RoomDe1.AllNigntOfferRate;
                            APIPreBookingInfoObj_.IsPriceChange = true;
                            APIPreBookingInfoObj_.RawData = RoomDe1;
                            APIPreBookingInfoObj_.CancellationPolicy = RoomDe1.CanclePolicy;
                            APIPreBookingInfoObj_.CancellationDate = RoomDe1.FreeCostCancellationDate;
                            APIPreBookingInfoObj_.Norms = RoomDe1.Errata;
                        }
                        else
                        {
                            APIPreBookingInfoObj_.After_Rate = RoomDe1.AllNigntOfferRate;
                            APIPreBookingInfoObj_.Before_Rate = RoomDe1.AllNigntOfferRate;
                            APIPreBookingInfoObj_.IsPriceChange = false;
                            APIPreBookingInfoObj_.RawData = RoomDe1;
                            APIPreBookingInfoObj_.CancellationPolicy = RoomDe1.CanclePolicy;
                            APIPreBookingInfoObj_.CancellationDate = RoomDe1.FreeCostCancellationDate;
                            APIPreBookingInfoObj_.Norms = RoomDe1.Errata;
                            if (RoomDe1 != null && string.IsNullOrEmpty(RoomDe1.Errata))
                            {
                                APIPreBookingInfoObj_.Norms = RoomDe1.Errata;
                            }
                        }
                    }
                }
            }
            return APIPreBookingInfoObj_ ?? new APIPreBookingInfo();
        }

        public async Task<EHotelRoomDetail> GetPreBookingHotelRoomDetail(EHotelRoomDetail OldEHRDetails, TBOHotelPreBookResponse.PreBookResponse GetTBORoomDetails, EHotelDetailRequest DetailReq, string Hotelid, string BHRDID, MarkupClass MarkupClassObj, string FolderPath)
        {
            var EHotelRoomDetailobj = new EHotelRoomDetail();
            try
            {
                decimal APIROE = await _iapiCredentials.GetCurrancyRate(OldEHRDetails.Currency ?? "", DetailReq.ClientID, DetailReq.DBConnectioString ?? "");
                decimal UserROE = await _iapiCredentials.GetCurrancyRate(DetailReq.CurrencyCode ?? "", DetailReq.ClientID, DetailReq.DBConnectioString ?? "");
                string APICurrencyName = OldEHRDetails.Currency ?? "";
                foreach (var hotel in GetTBORoomDetails.HotelResult ?? new List<TBOHotelPreBookResponse.HotelResult>())
                {
                    foreach (var hotelroom in hotel.Rooms ?? new List<TBOHotelPreBookResponse.Room>())
                    {
                        var RateCalculation = new Dictionary<string, object>();
                        EHotelRoomDetailobj = new EHotelRoomDetail();
                        var HotelRoomType_ = hotelroom?.Name?.FirstOrDefault();
                        EHotelRoomDetailobj.BHRDID = BHRDID;
                        EHotelRoomDetailobj.ServiceAPIID = ApiSupplierId;
                        EHotelRoomDetailobj.RoomBasicClassType = HotelRoomType_ ?? string.Empty;
                        EHotelRoomDetailobj.RoomIndex = 1;
                        EHotelRoomDetailobj.RoomNo = DetailReq.TotalNoRoom;
                        EHotelRoomDetailobj.HotelID = hotel.HotelCode ?? string.Empty;
                        EHotelRoomDetailobj.RoomType = HotelRoomType_ ?? string.Empty;
                        EHotelRoomDetailobj.RoomTypeId = HotelRoomType_ ?? string.Empty;
                        EHotelRoomDetailobj.RoomMeals = hotelroom?.MealType ?? string.Empty;
                        EHotelRoomDetailobj.RoomMealsID = "0";
                        EHotelRoomDetailobj.RoomOccupancyDetails = DetailReq.RoomOccupancyDetails ?? new List<ERoomOccupancyInfo>();

                        if (hotel.RateConditions != null)
                        {
                            EHotelRoomDetailobj.ImportantNotes = hotel.RateConditions.Select(g => g).ToList();
                        }
                        if (hotelroom?.RoomPromotion != null && hotelroom.RoomPromotion != null)
                        {
                            EHotelRoomDetailobj.Errata = string.Join("<br>", hotelroom.RoomPromotion);
                        }
                        decimal DefaultAllNightRate = 0, AllNightTotalRate = 0;
                        decimal RoomRatePer = (Convert.ToDecimal(hotelroom?.TotalFare) / DetailReq.Nights) / DetailReq.TotalNoRoom;
                        RoomRatePer = (RoomRatePer / APIROE) * UserROE;
                        decimal AllNightOfferTotalRate = Convert.ToDecimal(hotelroom?.TotalFare); //* NoOfRooms;
                        AllNightOfferTotalRate = (AllNightOfferTotalRate / APIROE) * UserROE;
                        decimal Discount = 0;
                        Discount = (Discount / APIROE) * UserROE;
                        EHotelRoomDetailobj.IsOffer = (Discount > 0 ? true : false);

                        string SuuplementStr = string.Empty;
                        if (hotelroom?.Supplements != null)
                        {
                            //SuuplementStr += await GetSupplimentDetail_JSON(hotelroom.Supplements, DetailReq.CurrencyCode, APIROE, UserROE).ConfigureAwait(false);
                        }

                        AllNightTotalRate = AllNightOfferTotalRate + Discount;
                        var DayWiseRatesLst = new List<EDateWiseRoomRate>();
                        for (int k = 0; k < DetailReq.Nights; k++)
                        {
                            var OneDayRate = RoomRatePer;
                            var DicRates1sd = APIMarkupSection.AddMarkupByRule(ApiSupplierId, Convert.ToDecimal(OneDayRate), MarkupClassObj, APICurrencyName, 1, DetailReq.TotalNoRoom);
                            DayWiseRatesLst.Add(new EDateWiseRoomRate() { Date = DetailReq.CheckInDate_.AddDays(k).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), Rate = Convert.ToDecimal((Convert.ToDouble(DicRates1sd[APIMarkupSection.AR]))) });
                        }

                        var DicRates1 = APIMarkupSection.AddMarkupByRule(ApiSupplierId, Convert.ToDecimal(AllNightOfferTotalRate), MarkupClassObj, APICurrencyName, DetailReq.Nights, DetailReq.TotalNoRoom);
                        AllNightOfferTotalRate = (Convert.ToDecimal(DicRates1[APIMarkupSection.AR]));
                        DefaultAllNightRate = AllNightOfferTotalRate;

                        var DicAllNightTotalRate = APIMarkupSection.AddMarkupByRule(ApiSupplierId, Convert.ToDecimal(AllNightTotalRate), MarkupClassObj, APICurrencyName, DetailReq.Nights, DetailReq.TotalNoRoom);
                        AllNightTotalRate = (Convert.ToDecimal(DicAllNightTotalRate[APIMarkupSection.AR]));
                        EHotelRoomDetailobj.TotalTax = ((Convert.ToDecimal(DicAllNightTotalRate[APIMarkupSection.TXV])));
                        EHotelRoomDetailobj.IsTaxType = Convert.ToBoolean(DicAllNightTotalRate[APIMarkupSection.TXO]); Discount = Convert.ToDecimal(AllNightTotalRate) - Convert.ToDecimal(DefaultAllNightRate);
                        RateCalculation.Add(ContractedHotelConstant.Base_Room_Rate, Convert.ToDecimal(DicAllNightTotalRate[APIMarkupSection.AR]) - EHotelRoomDetailobj.TotalTax);//AP Cost
                        decimal DefaultTotalRate = Convert.ToDecimal(hotelroom.TotalFare);

                        int SupplierMasterID = ApiSupplierId;
                        ESupplierRoomDefaultInfo SupplierRoomDefaultInfoObj = new ESupplierRoomDefaultInfo();
                        SupplierRoomDefaultInfoObj.Supplier_AllNightOfferRate = Convert.ToDecimal(hotelroom.TotalFare);
                        SupplierRoomDefaultInfoObj.Supplier_AllNightRate = Convert.ToDecimal(hotelroom.TotalFare) + Discount;
                        SupplierRoomDefaultInfoObj.Supplier_Currency = APICurrencyName;
                        SupplierRoomDefaultInfoObj.Supplier_ID = SupplierMasterID;
                        SupplierRoomDefaultInfoObj.Supplier_MinNightRate = Convert.ToDecimal(hotelroom.TotalFare);
                        SupplierRoomDefaultInfoObj.Supplier_Name = APISupplierName;
                        SupplierRoomDefaultInfoObj.SupplierCode = APIName;

                        EHotelRoomDetailobj.Currency = DetailReq.CurrencyCode ?? string.Empty;
                        EHotelRoomDetailobj.Supplier = APISupplierName;
                        EHotelRoomDetailobj.IsSupplier = true;
                        EHotelRoomDetailobj.SupplierName = APISupplierName;

                        string LancellationPolicyStr = string.Empty;
                        int ii = 0;
                        if (hotelroom.CancelPolicies != null)
                        {
                            foreach (var policy in hotelroom.CancelPolicies ?? new List<TBOHotelPreBookResponse.CancelPolicy>())
                            {
                                string fromDateStr = policy.FromDate ?? string.Empty;
                                DateTime fromDate = DateTime.ParseExact(fromDateStr, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                if (DetailReq.TotalNoRoom > 1) { LancellationPolicyStr += "<b>Room " + (ii + 1) + "</b>:</br>"; }
                                double CancellationCharge = policy.CancellationCharge;


                                if (policy.ChargeType.Equals(Convert.ToString(TboApiv7CancellationChargeTypeForHotel.Fixed)))
                                {
                                    decimal _CancellationCharge = (Convert.ToDecimal(CancellationCharge) / APIROE) * UserROE;
                                    LancellationPolicyStr += "From <b>" + fromDate.AddDays(_hotelApiSetting.FreeCancellationBufferPeriod).ToString("dd/MM/yyyy") + " </b>  <b>" + string.Empty + "</b> Cancellation Charge Will Be <b>" + DetailReq.CurrencyCode + " " + _CancellationCharge.ToString("0.00") + "<br/>";
                                }
                                if (policy.ChargeType.Equals(Convert.ToString(TboApiv7CancellationChargeTypeForHotel.Percentage)))
                                {
                                    LancellationPolicyStr += "<br/>From <b>" + fromDate.AddDays(_hotelApiSetting.FreeCancellationBufferPeriod).ToString("dd/MM/yyyy") + " </b>   <b>" + string.Empty + "</b> Cancellation Charge Will Be <b>" + CancellationCharge + "% Rate <br/>";
                                }

                                if (policy.ChargeType.Equals(Convert.ToString(TboApiv7CancellationChargeTypeForHotel.Night)))
                                {
                                    decimal _CancellationCharge = (Convert.ToDecimal(CancellationCharge) / APIROE) * UserROE;
                                    LancellationPolicyStr += "<br />From <b>" + fromDate.AddDays(_hotelApiSetting.FreeCancellationBufferPeriod).ToString("dd/MM/yyyy") + " </b>   <b>" + string.Empty + "</b> Cancellation Charge Will Be <b>" + _CancellationCharge.ToString("0.00") + " nights <br/>";
                                }
                            }
                        }

                        EHotelRoomDetailobj.IsCanclePolicy = hotelroom?.CancelPolicies?.Count != 0;
                        EHotelRoomDetailobj.CanclePolicy = LancellationPolicyStr;
                        string fromDateStr_lst = hotelroom?.CancelPolicies?.Min(h => h.FromDate) ?? string.Empty;
                        var isFree = hotelroom?.CancelPolicies?.Where(x => Convert.ToDecimal(x.CancellationCharge).Equals(0)).Select(m => m).ToList();
                        EHotelRoomDetailobj.IsFreeCostCancellationDate = isFree?.Count != 0;

                        if (hotelroom?.CancelPolicies == null && hotelroom?.IsRefundable != null && hotelroom.IsRefundable)
                        {
                            fromDateStr_lst = DetailReq.CheckInDate_.ToString("dd-MM-yyyy HH:mm:ss");
                            EHotelRoomDetailobj.IsFreeCostCancellationDate = true;
                        }

                        DateTime lstCancel_fromDate = DateTime.ParseExact(fromDateStr_lst ?? string.Empty, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        DateTime LastCancellationDate = new DateTime();
                        LastCancellationDate = lstCancel_fromDate;

                        EHotelRoomDetailobj.FreeCostCancellationDate = LastCancellationDate.AddDays(_hotelApiSetting.FreeCancellationBufferPeriod).ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
                        EHotelRoomDetailobj.FreeCostCancellationDate = Convert.ToDateTime(EHotelRoomDetailobj.FreeCostCancellationDate).AddDays(-1).ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
                        if (DateTime.Now > Convert.ToDateTime(EHotelRoomDetailobj.FreeCostCancellationDate))
                        {
                            EHotelRoomDetailobj.IsFreeCostCancellationDate = false;
                        }
                        else
                        {
                            EHotelRoomDetailobj.IsFreeCostCancellationDate = true;
                        }

                        EHotelRoomDetailobj.Errata = SuuplementStr;
                        EHotelRoomDetailobj.IsErrata = SuuplementStr != string.Empty;

                        Dictionary<string, string> TBO_APIExtraInfo = new Dictionary<string, string>();
                        TBO_APIExtraInfo.Add("Inclusion", hotelroom?.Inclusion ?? string.Empty);
                        TBO_APIExtraInfo.Add("BookingCode", hotelroom?.BookingCode ?? string.Empty);
                        TBO_APIExtraInfo.Add("HotelCode", hotel?.HotelCode ?? string.Empty);
                        TBO_APIExtraInfo.Add("HotelName", hotel?.HotelCode ?? string.Empty);
                        TBO_APIExtraInfo.Add("RoomIndex", "Roomindex_1");
                        TBO_APIExtraInfo.Add("BHRDID", BHRDID);
                        TBO_APIExtraInfo.Add("TotalFare", Convert.ToString(hotelroom.TotalFare));
                        EHotelRoomDetailobj.ExtraInfo = JsonConvert.SerializeObject(TBO_APIExtraInfo);

                        EHotelRoomDetailobj.AvilableRoom = DetailReq.TotalNoRoom;
                        EHotelRoomDetailobj.Issharedbedding = false;
                        EHotelRoomDetailobj.IsExtraBed = false;
                        EHotelRoomDetailobj.IsMealPlanUpgrad = false;
                        EViewCalculation ViewCalculation = new EViewCalculation()
                        {
                            Currency = DetailReq.CurrencyCode ?? string.Empty,
                            DiscountRate = (-Discount),
                            HotelName = Convert.ToString(string.Empty),
                            IsOffer = EHotelRoomDetailobj.IsOffer,
                            OfferStr = EHotelRoomDetailobj.OfferName,
                            OtherCharges = 0,
                            ROE = 1,
                            Rates = DayWiseRatesLst

                        };
                        EHotelRoomDetailobj.ViewCalculation = ViewCalculation;
                        EHotelRoomDetailobj.AllNightTotalRate = AllNightTotalRate;
                        EHotelRoomDetailobj.AllNigntOfferRate = AllNightOfferTotalRate;
                        EHotelRoomDetailobj.TotalRate = RoomRatePer;

                        List<EHotelRoomImages> HotelRoomImages = new List<EHotelRoomImages>();
                        EHotelRoomDetailobj.HotelRoomImages = HotelRoomImages;
                        EHotelRoomDetailobj.SupplierRoomDefaultInfo = SupplierRoomDefaultInfoObj;
                        EHotelRoomDetailobj.EMealPlanUpgradeInfoObj = new EMealPlanUpgradeInfo();
                        EHotelRoomDetailobj.GMXMealPlanUpgradeInfo = new GMXMealPlanUpgradeInfo();

                        ERoomDefaultInfo RoomDefaultInfoObj = new ERoomDefaultInfo();
                        RoomDefaultInfoObj.EHotelMarkupInfoLst = new List<EHotelMarkupInfo>();
                        RoomDefaultInfoObj.Purchase_CurrencyCode = hotel.Currency ?? string.Empty;
                        RoomDefaultInfoObj.Purchase_CurrencyID = 0;
                        RoomDefaultInfoObj.Purchase_CurrencyROE_DIV = Convert.ToDecimal("1");
                        RoomDefaultInfoObj.Purchase_CurrencyROE_MLP = Convert.ToDecimal("1");
                        RoomDefaultInfoObj.Purchase_FinalAllNightRate = Convert.ToDecimal(hotelroom?.TotalFare) + Discount;
                        RoomDefaultInfoObj.Supplier_AllNightOfferRate = Convert.ToDecimal(hotelroom?.TotalFare);
                        RoomDefaultInfoObj.Supplier_AllNightRate = Convert.ToDecimal(hotelroom?.TotalFare) + Discount;
                        RoomDefaultInfoObj.Supplier_Currency = hotel.Currency ?? string.Empty;
                        RoomDefaultInfoObj.System_AllNightOfferRate = Convert.ToDecimal(hotelroom?.TotalFare) + Discount;
                        RoomDefaultInfoObj.System_AllNightRate = Convert.ToDecimal(hotelroom?.TotalFare) + Discount;
                        RoomDefaultInfoObj.System_CurrencyCode = hotel.Currency ?? string.Empty;
                        RoomDefaultInfoObj.System_CurrencyID = 1;
                        RoomDefaultInfoObj.System_CurrencyROE_DIV = 1;
                        RoomDefaultInfoObj.System_CurrencyROE_MLP = 1;
                        RoomDefaultInfoObj.System_OneNightRate = RoomRatePer;
                        RoomDefaultInfoObj.System_TotalMarkup = 0;
                        RoomDefaultInfoObj.System_TotalTax = 0;
                        RoomDefaultInfoObj.DicAllNightRate = DicAllNightTotalRate;
                        RoomDefaultInfoObj.DicAllOfferTotalRate = DicRates1;
                        EHotelRoomDetailobj.RoomDefaultInfo = RoomDefaultInfoObj;
                        //RateCalculation

                        RateCalculation.Add(ContractedHotelConstant.Service_Charges, 0);
                        RateCalculation.Add(ContractedHotelConstant.Service_Charges_Value, "0");
                        RateCalculation.Add(ContractedHotelConstant.Service_Charges_CalType, "1");
                        RateCalculation.Add(ContractedHotelConstant.Extra_Fees, 0);
                        RateCalculation.Add(ContractedHotelConstant.Extra_Fees_Value, 0);
                        RateCalculation.Add(ContractedHotelConstant.Extra_Fees_CalType, "1");
                        RateCalculation.Add(ContractedHotelConstant.Extra_Fees_Desc, "Munipality Fees");
                        RateCalculation.Add(ContractedHotelConstant.Input_Tax, 0);
                        RateCalculation.Add(ContractedHotelConstant.Output_Tax, EHotelRoomDetailobj.TotalTax);
                        RateCalculation.Add(ContractedHotelConstant.Base_Room_Rate_OfferDiscount, Discount);
                        RateCalculation.Add(ContractedHotelConstant.Base_Room_Rate_OfferName, Convert.ToString(EHotelRoomDetailobj.OfferName));
                        EHotelRoomDetailobj.RateCalculation = RateCalculation;
                        //RateCalculation
                        EHotelRoomDetailobj.CheckInDate = DetailReq.CheckInDate_;
                        EHotelRoomDetailobj.CheckOutDate = DetailReq.CheckOutDate_;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return EHotelRoomDetailobj;
        }

        public async Task<APIBookingInfo> GetTboApiBookingResponseAsync(Dictionary<string, string> ApiCredentials, EHotelBookingRequest ObjEHotelBookingRequest, EHotelDetailRequest ObjEHotelDetailRequest, EHotelRoomDetail ObjEHotelRoomDetail, string FolderPath)
        {
            var result = new APIBookingInfo();

            var TBO_ExtraInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(ObjEHotelRoomDetail.ExtraInfo) ?? new Dictionary<string, string>();
            result = await _tboApiRequest.HotelBooking(ApiCredentials, ObjEHotelBookingRequest.RoomWisePaxInfo, TBO_ExtraInfo, FolderPath, ObjEHotelBookingRequest.BookingID).ConfigureAwait(false);

            return result;
        }

        public async Task<APICancelationInfo> GetTboApiCancelResponseAsync(Dictionary<string, string> ApiCredentials, string bookingreference, string FolderPath, int BookingID)
        {
            var APICancelationInfoObj = new APICancelationInfo();
            APICancelationInfoObj = await _tboApiRequest.HotelCancel(ApiCredentials, bookingreference, FolderPath, BookingID).ConfigureAwait(false);
            return APICancelationInfoObj;
        }

        private async Task<string> GetSupplimentDetail_JSON(List<List<TBOHotelSearchResponse.Supplement>> supplements, string? currencyCode, decimal apiRoe, decimal userRoe)
        {
            string SupplimentStr = string.Empty;
            return string.Empty;
        }
    }
}
