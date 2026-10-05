using EtosApiServices.Data.Library.Common;
using EtosApiServices.Hotel.Library.Contract;
using EtosApiServices.Hotel.Model;
using EtosApiServices.Hotel.Model.ApiSetting;
using EtosApiServices.Model.Setting;
using EtosApiServices.MongoDb.Library;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EtosApiServices.Hotel.Model.HotelBedsAvailabilityResponse;

namespace EtosApiServices.Hotel.Library.Services
{
    public class HotelBeds : IHotelBeds
    {
        private readonly HotelBedsApiRequest _hotelBedsApiRequest;
        private readonly IAPICredentials _iapiCredentials;
        readonly HotelApiSetting _hotelApiSetting;
        public readonly string APIName = AppConfig.GetConfigValue("Hotel:HotelBed:APIName");
        public readonly int ApiSupplierId = int.Parse(AppConfig.GetConfigValue("Hotel:HotelBed:SupplierId"));
        public readonly string APISupplierName = AppConfig.GetConfigValue("Hotel:HotelBed:SupplierName");//"HB";
        private static readonly HotelBedsModelClass.HotelBedsHotelDetail.Hotel s_emptyHotel = new HotelBedsModelClass.HotelBedsHotelDetail.Hotel();

        public HotelBeds(HotelBedsApiRequest hotelBedsApiRequest, IAPICredentials aPICredentials, IOptions<HotelApiSetting> hotelApiSetting)
        {
            _hotelBedsApiRequest = hotelBedsApiRequest ?? throw new ArgumentException(nameof(hotelBedsApiRequest));
            _iapiCredentials = aPICredentials ?? throw new ArgumentException(nameof(aPICredentials));
            _hotelApiSetting = hotelApiSetting?.Value ?? throw new ArgumentException(nameof(hotelApiSetting));
        }

        public async Task<List<ETSearchHotel>> GetHotelBedsApiResponseAsync(EHotelSearchRequest SearchReq, Dictionary<string, string> ApiCredentials, string FolderName, MarkupClass MarkupClassObj)
        {
            var resultList = new List<ETSearchHotel>();

            var hotelBedRes = await _hotelBedsApiRequest.GetHotelBedsApiResponseAsync(SearchReq, ApiCredentials, FolderName).ConfigureAwait(false);
            var hotels = hotelBedRes?.hotels?.hotels;
            if (hotels == null || hotels.Count == 0)
            {
                return resultList;
            }

            // ensure folder path is set
            HotelBedsModelClass.HotelBedsFolderPath = AppConfig.ServerMapPath("~/APILogs/HotelBeds/");

            // Prepare hotel codes (distinct) and load static data from MongoDB
            var hotelCodes = hotels.Select(h => h.code).Where(c => c != default).Distinct().ToList();
            var client = new MongoDbCRUD(APIKeySettings.HB_Database);
            var db = await client.LoadHotelDataWithInHotelBeds<HotelBedsModelClass.HotelBedsHotelDetail.Hotel>(APIKeySettings.HB_Table, "code", hotelCodes).ConfigureAwait(false);

            // Build a lookup dictionary from DB results (keep first for duplicate keys)
            var staticData = (db ?? Enumerable.Empty<HotelBedsModelClass.HotelBedsHotelDetail.Hotel>())
                .GroupBy(d => d.code)
                .ToDictionary(g => g.Key, g => g.First());

            // Fetch currency rates in parallel
            var apiCurrency = hotels[0].currency ?? string.Empty;
            var taskApiRoe = _iapiCredentials.GetCurrancyRate(apiCurrency, SearchReq.ClientID, SearchReq.DBConnectioString ?? string.Empty);
            var taskUserRoe = _iapiCredentials.GetCurrancyRate(SearchReq.CurrencyCode ?? string.Empty, SearchReq.ClientID, SearchReq.DBConnectioString ?? string.Empty);
            await Task.WhenAll(taskApiRoe, taskUserRoe).ConfigureAwait(false);
            var apiRoe = taskApiRoe.Result;
            var userRoe = taskUserRoe.Result;

            // Pre-size result list
            resultList = new List<ETSearchHotel>(hotels.Count);

            foreach (var hitem in hotels)
            {
                staticData.TryGetValue(hitem.code, out var hbHotelDetail);
                var etHotel = new ETHotelSearchList((int)HotelApiSupplier.HBAPIID, APIName ?? string.Empty, hitem, hbHotelDetail ?? s_emptyHotel, SearchReq, MarkupClassObj, apiRoe, userRoe);
                resultList.Add(etHotel);
            }

            return resultList;
        }

        public async Task<EHotelDetail> GetHotelBedsApiDetailResponseAsync(EHotelDetailRequest DetailReq, Dictionary<string, string> ApiCredentials, string FolderName, string BHRDID, MarkupClass MarkupClassObj)
        {
            var HotelObj = new EHotelDetail();

            var TotalRoomsPaxOccupancy = (DetailReq.RoomOccupancyDetails ?? Enumerable.Empty<ERoomOccupancyInfo>())
                  .GroupBy(x => new { x.AdultCount, x.ChildCount, ChildAges = ((x.ChildAges != null && x.ChildAges.Count > 0) ? string.Join("-", x.ChildAges) : "0-0-0") })
                  .Select(g => new
                  {
                      TotalRoom = g.Count(),
                      TotalAdults = g.Max(x => x.AdultCount),
                      TotalChildren = g.Max(x => x.ChildCount),
                      ChildAges = g.SelectMany(x => x.ChildAges ?? new List<int>()).Where(h => h > 0).Distinct().ToList()
                  }).OrderByDescending(x => x.TotalRoom)
                  .ToList();

            var HotelBedRes = await _hotelBedsApiRequest.AvailabilityByHodelCode(DetailReq, ApiCredentials, FolderName);
            var hotels = HotelBedRes?.hotels?.hotels;
            if (hotels == null || hotels.Count == 0)
            {
                return HotelObj;
            }

            var hotelCodes = hotels.Select(h => h.code).Where(c => c != default).Distinct().ToList();
            var client = new MongoDbCRUD(APIKeySettings.HB_Database);
            var db = await client.LoadHotelDataWithInHotelBeds<HotelBedsModelClass.HotelBedsHotelDetail.Hotel>(APIKeySettings.HB_Table, "code", hotelCodes).ConfigureAwait(false);

            // Build a lookup dictionary from DB results (keep first for duplicate keys)
            var staticData = (db ?? Enumerable.Empty<HotelBedsModelClass.HotelBedsHotelDetail.Hotel>())
                .GroupBy(d => d.code)
                .ToDictionary(g => g.Key, g => g.First());

            // If staticData is empty, attempt to fetch and add
            if (staticData.Count == 0)
            {
                var data = await _hotelBedsApiRequest.GetHotelInfoByHotelCode(ApiCredentials, DetailReq.HotelId ?? "");
                if (data != null)
                {
                    staticData = new Dictionary<int, HotelBedsModelClass.HotelBedsHotelDetail.Hotel>(staticData)
                    {
                        [data.code] = data
                    };
                }
            }

            var apiCurrency = hotels[0].currency ?? string.Empty;
            var taskApiRoe = _iapiCredentials.GetCurrancyRate(apiCurrency, DetailReq.ClientID, DetailReq.DBConnectioString ?? string.Empty);
            var taskUserRoe = _iapiCredentials.GetCurrancyRate(DetailReq.CurrencyCode ?? string.Empty, DetailReq.ClientID, DetailReq.DBConnectioString ?? string.Empty);
            await Task.WhenAll(taskApiRoe, taskUserRoe).ConfigureAwait(false);
            decimal apiRoe = taskApiRoe.Result;
            decimal userRoe = taskUserRoe.Result;

            foreach (var hitem in hotels)
            {
                staticData.TryGetValue(hitem.code, out var Staticinfo);
                HotelObj = new EHotelDetail();
                HotelObj.Name = hitem.name;
                HotelObj.Address = Staticinfo?.address?.content;
                HotelObj.location = Staticinfo?.address?.street;
                int _star = 0;
                var star_ = hitem?.categoryName?.Split(' ')[0];
                if (int.TryParse(star_, out _star))
                {
                    HotelObj.StarRating = Convert.ToString(_star);
                }
                else
                {
                    HotelObj.StarRating = Convert.ToString(hitem?.categoryName);
                }

                HotelObj.HotelCode = Convert.ToString(hitem?.code);
                HotelObj.latitude = hitem?.latitude;
                HotelObj.longitude = hitem?.longitude;
                HotelObj.CityName = Staticinfo?.city?.content;
                if (Staticinfo?.images != null && Staticinfo.images.Count != 0)
                {
                    HotelObj.MainImage = Staticinfo?.images?.OrderBy(f => f.order).FirstOrDefault().path;
                }

                HotelObj.Supplier = APIName;
                HotelObj.HotelString = APIName + "~" + Convert.ToString(hitem?.code);
                HotelObj.Is_Offer = "0";
                HotelObj.Property_Type = Staticinfo?.accommodationTypeCode;
                HotelObj.Description = Staticinfo?.description?.content;
                var EAminitiesLst = new List<EAminities>();
                var EHotelImageGallaryLst = new List<EHotelImageGallary>();
                if (Staticinfo?.images != null && Staticinfo.images.Count != 0)
                {
                    foreach (var img in Staticinfo.images)
                    {
                        var EHotelImageGallaryObj = new EHotelImageGallary()
                        {
                            HotelId = Convert.ToString(HotelObj.HotelCode),
                            HotelName = Convert.ToString(HotelObj.Name),
                            ImageActualPath = Convert.ToString(img.path),
                            ImageName = Convert.ToString(img.imageTypeCodeDes),
                            ImageThumPath = Convert.ToString(img.path)
                        };
                        EHotelImageGallaryLst.Add(EHotelImageGallaryObj);
                    }

                    if (Staticinfo?.facilities?.Count != 0)
                    {
                        var Amenities = Staticinfo?.facilities?.Where(f => f.facilityGroupCode.Equals(70)).ToList();
                        foreach (var fac in Amenities ?? new List<HotelBedsModelClass.HotelBedsHotelDetail.Facility>())
                        {
                            var EAminitiesobj = new EAminities()
                            {
                                AminitieName = fac.facilityCodeDesc ?? "",
                                AminitieCategory = fac.facilityGroupCodeDesc ?? "",
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
                HotelObj.HotelContactDetails = string.Join(",", Staticinfo?.phones?.Select(f => f.phoneNumber ?? "") ?? Enumerable.Empty<string>());
                HotelObj.HotelEmail = Staticinfo?.email ?? string.Empty;
                HotelObj.HotelURL = Staticinfo.web ?? "";
                HotelObj.HotelDetail = "";
                HotelObj.HDType = HDType.Normal;
                HotelObj.GiataID = "0";
                HotelObj.GiataCityID = "0";
                HotelObj.CheckInTime = HotelBedRes?.hotels?.checkIn;
                HotelObj.CheckOutTime = HotelBedRes?.hotels?.checkOut;
                HotelObj.CurrentDeals = string.Empty;
                HotelObj.TotalTax = 0;
                string APICurrencyName = hitem?.currency ?? "";
                HotelObj.Currency = APICurrencyName;

                var total = (hitem?.minRate / apiRoe) * userRoe;
                HotelObj.MinOneRate = Convert.ToString(total / DetailReq.TotalNoRoom);
                HotelObj.AllNightRate = Convert.ToString(total);
                var EHotelRoomDetailLst = new List<EHotelRoomDetail>();
                foreach (var Rooms in hitem?.rooms ?? Enumerable.Empty<HotelBedsDetailsAvailabilityResponse.Room>())
                {
                    var LstRoom = from rates_Item in Rooms?.rates ?? Enumerable.Empty<HotelBedsDetailsAvailabilityResponse.Rate>()
                                  group rates_Item by new
                                  {
                                      rateKey = rates_Item.rateKey,
                                      rooms = rates_Item.rooms,
                                      adults = rates_Item.adults,
                                      children = rates_Item.children,
                                      childrenAges = rates_Item.children > 0 ? rates_Item.childrenAges : "0",
                                      boardName = rates_Item.boardName,
                                  } into GRate
                                  select GRate;

                    var LstRoomList = LstRoom?.ToList();

                    if (LstRoomList != null && LstRoomList.Count > 0)
                    {
                        var LstRoomBoardName = (from Boardrates_Item in LstRoomList
                                                group Boardrates_Item by new
                                                {
                                                    boardName = Boardrates_Item.Key.GetType().GetProperty("boardName").GetValue(Boardrates_Item.Key, null)
                                                } into BRate
                                                select BRate).ToList();

                        if (LstRoomBoardName.Count > 0)
                        {
                            foreach (var RoomBoardName in LstRoomBoardName)
                            {
                                var LstRoomRates = new List<HotelBedsDetailsAvailabilityResponse.Rate>();

                                foreach (var HbRoomsPaxOccupancy in TotalRoomsPaxOccupancy)
                                {
                                    var ObjRoomRates = LstRoomList.Where(f =>
                                    {
                                        var key = f.Key;
                                        try
                                        {
                                            var rateKey = (string)key.GetType().GetProperty("rateKey").GetValue(key, null);
                                            var roomsVal = (int)key.GetType().GetProperty("rooms").GetValue(key, null);
                                            var adultsVal = (int)key.GetType().GetProperty("adults").GetValue(key, null);
                                            var childrenVal = (int)key.GetType().GetProperty("children").GetValue(key, null);
                                            var childrenAgesVal = (string)key.GetType().GetProperty("childrenAges").GetValue(key, null);
                                            var boardNameVal = (string)key.GetType().GetProperty("boardName").GetValue(key, null);

                                            return boardNameVal == RoomBoardName.Key.GetType().GetProperty("boardName").GetValue(RoomBoardName.Key, null)?.ToString()
                                                   && adultsVal.ToString() == HbRoomsPaxOccupancy.TotalAdults.ToString()
                                                   && childrenVal.ToString() == HbRoomsPaxOccupancy.TotalChildren.ToString()
                                                   && childrenAgesVal == (HbRoomsPaxOccupancy.TotalChildren > 0 ? string.Join(",", HbRoomsPaxOccupancy.ChildAges.Where(l => l.ToString() != "0").Select(h => h.ToString())) : "0");
                                        }
                                        catch
                                        {
                                            return false;
                                        }
                                    }).ToList();

                                    if (ObjRoomRates != null && ObjRoomRates.Count > 0)
                                    {
                                        var keyObj = ObjRoomRates[0].Key;
                                        var query = Rooms?.rates?.FirstOrDefault(f => f.rateKey == keyObj.GetType().GetProperty("rateKey").GetValue(keyObj, null) as string
                                                                                 && f.rooms == (int)keyObj.GetType().GetProperty("rooms").GetValue(keyObj, null)
                                                                                 && f.adults == (int)keyObj.GetType().GetProperty("adults").GetValue(keyObj, null)
                                                                                 && f.children == (int)keyObj.GetType().GetProperty("children").GetValue(keyObj, null)
                                                                                 && ((f.childrenAges != null ? f.childrenAges : "0") == (keyObj.GetType().GetProperty("childrenAges").GetValue(keyObj, null) as string))
                                                                                 && f.boardName == (keyObj.GetType().GetProperty("boardName").GetValue(keyObj, null) as string));

                                        if (query != null && !LstRoomRates.Contains(query))
                                        {
                                            LstRoomRates.Add(query);
                                        }
                                    }
                                }

                                if (LstRoomRates.Any() && DetailReq.TotalNoRoom == LstRoomRates.Sum(f => f.rooms))
                                {
                                    var RateCalculation = new Dictionary<string, object>();
                                    var EHotelRoomDetailobj = new EHotelRoomDetail();
                                    EHotelRoomDetailobj.BHRDID = BHRDID + "!" + Rooms?.code;
                                    EHotelRoomDetailobj.ServiceAPIID = ApiSupplierId;
                                    EHotelRoomDetailobj.RoomBasicClassType = Rooms?.name ?? "";
                                    EHotelRoomDetailobj.RoomIndex = 0;
                                    EHotelRoomDetailobj.RoomNo = DetailReq.TotalNoRoom;
                                    EHotelRoomDetailobj.HotelID = HotelObj?.HotelCode ?? "";
                                    EHotelRoomDetailobj.RoomType = Rooms?.name ?? "";
                                    EHotelRoomDetailobj.RoomTypeId = Rooms?.code ?? "";
                                    EHotelRoomDetailobj.RoomMeals = LstRoomRates[0].boardName ?? "";
                                    EHotelRoomDetailobj.RoomMealsName = LstRoomRates[0].boardName ?? "";
                                    EHotelRoomDetailobj.RoomMealsID = LstRoomRates[0].boardCode ?? "";
                                    EHotelRoomDetailobj.RoomOccupancyDetails = DetailReq.RoomOccupancyDetails ?? new List<ERoomOccupancyInfo>();

                                    decimal DefaultAllNightRate = 0, AllNightTotalRate = 0;
                                    decimal net;
                                    if (LstRoomRates.Any(q => q.hotelMandatory.Equals(true)))
                                    {
                                        net = LstRoomRates.Select(q => q.sellingRate).Sum();
                                    }
                                    else
                                    {
                                        net = LstRoomRates.Select(q => q.net).Sum();
                                    }
                                    net = (net / apiRoe) * userRoe;
                                    decimal RoomRatePer = (net / DetailReq.Nights) / DetailReq.TotalNoRoom;
                                    decimal AllNightOfferTotalRate = net; //* NoOfRooms;

                                    var Discount = (LstRoomRates.Select(p => p.discount).Sum() / apiRoe) * userRoe;
                                    AllNightTotalRate = AllNightOfferTotalRate + Discount;
                                    var DayWiseRatesLst = new List<EDateWiseRoomRate>();

                                    if (LstRoomRates[0].dailyRate != null)
                                    {
                                        for (int k = 0; k < LstRoomRates[0]?.dailyRate?.Count; k++)
                                        {
                                            var OneDayRate = LstRoomRates.Select(r => r.dailyRate[k].dailySellingRate).Sum();
                                            OneDayRate = (OneDayRate / apiRoe) * userRoe;
                                            var DicRates1sd = APIMarkupSection.AddMarkupByRule(ApiSupplierId, Convert.ToDecimal(OneDayRate), MarkupClassObj, APICurrencyName, 1, DetailReq.TotalNoRoom);
                                            DayWiseRatesLst.Add(new EDateWiseRoomRate() { Date = DetailReq.CheckInDate_.AddDays(k).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), Rate = Convert.ToDecimal((Convert.ToDouble(DicRates1sd[APIMarkupSection.AR]))) });
                                        }
                                    }
                                    else
                                    {
                                        for (int k = 0; k < DetailReq.Nights; k++)
                                        {
                                            var DicRates1sd = APIMarkupSection.AddMarkupByRule(ApiSupplierId, Convert.ToDecimal(RoomRatePer), MarkupClassObj, APICurrencyName, 1, DetailReq.TotalNoRoom);
                                            DayWiseRatesLst.Add(new EDateWiseRoomRate() { Date = DetailReq.CheckInDate_.AddDays(k).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), Rate = Convert.ToDecimal((Convert.ToDouble(DicRates1sd[APIMarkupSection.AR]))) });
                                        }
                                    }
                                    var DicRates1 = APIMarkupSection.AddMarkupByRule(ApiSupplierId, Convert.ToDecimal(AllNightOfferTotalRate), MarkupClassObj, APICurrencyName, DetailReq.Nights, DetailReq.TotalNoRoom);
                                    AllNightOfferTotalRate = (Convert.ToDecimal(DicRates1[APIMarkupSection.AR]));
                                    DefaultAllNightRate = AllNightOfferTotalRate;

                                    var DicAllNightTotalRate = APIMarkupSection.AddMarkupByRule(ApiSupplierId, Convert.ToDecimal(AllNightTotalRate), MarkupClassObj, APICurrencyName, DetailReq.Nights, DetailReq.TotalNoRoom);
                                    AllNightTotalRate = (Convert.ToDecimal(DicAllNightTotalRate[APIMarkupSection.AR]));
                                    EHotelRoomDetailobj.TotalTax = ((Convert.ToDecimal(DicAllNightTotalRate[APIMarkupSection.TXV])));
                                    EHotelRoomDetailobj.IsTaxType = Convert.ToBoolean(DicAllNightTotalRate[APIMarkupSection.TXO]);
                                    RateCalculation.Add(ContractedHotelConstant.Base_Room_Rate, Convert.ToDecimal(DicAllNightTotalRate[APIMarkupSection.AR]) - EHotelRoomDetailobj.TotalTax);//AP Cost
                                    Discount = Convert.ToDecimal(AllNightTotalRate) - Convert.ToDecimal(DefaultAllNightRate);
                                    decimal DefaultTotalRate = Convert.ToDecimal(LstRoomRates[0].hotelSellingRate);

                                    string SupplierName = APISupplierName;
                                    int SupplierMasterID = ApiSupplierId;

                                    ESupplierRoomDefaultInfo SupplierRoomDefaultInfoObj = new ESupplierRoomDefaultInfo();
                                    SupplierRoomDefaultInfoObj.Supplier_AllNightOfferRate = Convert.ToDecimal(LstRoomRates.Select(q => q.net).Sum());
                                    SupplierRoomDefaultInfoObj.Supplier_AllNightRate = Convert.ToDecimal(LstRoomRates.Select(q => q.net).Sum()) + Discount;
                                    SupplierRoomDefaultInfoObj.Supplier_Currency = APICurrencyName;
                                    SupplierRoomDefaultInfoObj.Supplier_ID = SupplierMasterID;
                                    SupplierRoomDefaultInfoObj.Supplier_MinNightRate = Convert.ToDecimal(RoomRatePer);
                                    SupplierRoomDefaultInfoObj.Supplier_Name = SupplierName;
                                    SupplierRoomDefaultInfoObj.SupplierCode = APIName;

                                    #region cancellation Policy
                                    EHotelRoomDetailobj.Currency = hitem?.currency ?? "";
                                    EHotelRoomDetailobj.Supplier = SupplierName;
                                    EHotelRoomDetailobj.IsSupplier = true;
                                    EHotelRoomDetailobj.SupplierName = SupplierName;
                                    EHotelRoomDetailobj.IsFreeCostCancellationDate = false;
                                    DateTime LastCancellationDate = DetailReq.CheckInDate_;
                                    string LancellationPolicyStr = string.Empty;

                                    if (LstRoomRates[0].cancellationPolicies != null)
                                    {
                                        var Allcancellation = LstRoomRates.SelectMany(n => n.cancellationPolicies ?? new List<HotelBedsDetailsAvailabilityResponse.CancellationPolicy>()).ToList();
                                        var pl = Allcancellation.Select(xx => xx.from).Distinct().ToList();
                                        foreach (var datess in pl)
                                        {
                                            var amt = Allcancellation.Where(xm => xm.from.ToString() == datess.ToString().Trim()).Sum(xxx => Convert.ToDecimal(xxx.amount));

                                            DateTime CancellationDaidLineDate = new DateTime();
                                            amt = (amt / apiRoe) * userRoe;
                                            amt = Convert.ToDecimal(APIMarkupSection.AddPolicyMarkupByRule(ApiSupplierId, amt, MarkupClassObj, APICurrencyName, 1, 1));
                                            amt = Math.Ceiling(amt);
                                            CancellationDaidLineDate = datess;
                                            LancellationPolicyStr += "From  <b>" + CancellationDaidLineDate.AddDays(_hotelApiSetting.FreeCancellationBufferPeriod).ToString("dd MMM yyyy", CultureInfo.InvariantCulture) + "</b>  Cancellation Penalty will be <b>" + DetailReq.CurrencyCode + " " + amt.ToString() + "</b>  <br/>";
                                        }

                                        DateTime freedate = Convert.ToDateTime(Allcancellation.Select(xt => xt.from).Min().ToString());
                                        DateTime todaysdate = DateTime.Now;
                                        EHotelRoomDetailobj.FreeCostCancellationDate = freedate.AddDays(_hotelApiSetting.FreeCancellationBufferPeriod).ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
                                        EHotelRoomDetailobj.FreeCostCancellationDate = Convert.ToDateTime(EHotelRoomDetailobj.FreeCostCancellationDate).AddDays(-1).ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
                                        if (Convert.ToDateTime(EHotelRoomDetailobj.FreeCostCancellationDate) >= todaysdate)
                                        {
                                            EHotelRoomDetailobj.IsFreeCostCancellationDate = true;
                                        }
                                        EHotelRoomDetailobj.IsCanclePolicy = LstRoomRates[0]?.cancellationPolicies?.Count != 0;
                                        LastCancellationDate = freedate;
                                    }
                                    else
                                    {
                                        EHotelRoomDetailobj.IsFreeCostCancellationDate = false;
                                        EHotelRoomDetailobj.IsCanclePolicy = false;
                                        EHotelRoomDetailobj.FreeCostCancellationDate = DetailReq.CheckInDate_.AddDays(_hotelApiSetting.FreeCancellationBufferPeriod).ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
                                    }
                                    EHotelRoomDetailobj.CanclePolicy = LancellationPolicyStr;

                                    #endregion

                                    string SuuplementStr = string.Empty;
                                    if (LstRoomRates[0].promotions != null)
                                    {
                                        SuuplementStr = LstRoomRates[0]?.promotions?.FirstOrDefault()?.name ?? "";
                                    }
                                    EHotelRoomDetailobj.Errata = SuuplementStr;
                                    EHotelRoomDetailobj.IsErrata = SuuplementStr != string.Empty;

                                    // Build ExtraInfo as per-rate entries: fields separated by '!' and rates separated by '$'
                                    var perRateExtraInfo = new List<string>();
                                    foreach (var ExtInfo in LstRoomRates ?? Enumerable.Empty<HotelBedsDetailsAvailabilityResponse.Rate>())
                                    {
                                        var fields = new List<string>
                                        {
                                            ExtInfo?.rateKey ?? string.Empty,       //0
                                            ExtInfo?.rateCommentsId ?? string.Empty, //1
                                            ExtInfo?.boardCode ?? string.Empty,      //2
                                            ExtInfo?.rateType ?? string.Empty,       //3
                                            BHRDID ?? string.Empty                   //4
                                        };
                                        perRateExtraInfo.Add(string.Join("!", fields));
                                    }

                                    EHotelRoomDetailobj.ExtraInfo = string.Join("$", perRateExtraInfo);
                                    EHotelRoomDetailobj.AvilableRoom = DetailReq.TotalNoRoom;
                                    EHotelRoomDetailobj.Issharedbedding = false;
                                    EHotelRoomDetailobj.IsExtraBed = false;
                                    EHotelRoomDetailobj.IsMealPlanUpgrad = false;

                                    EViewCalculation ViewCalculation = new EViewCalculation()
                                    {
                                        Currency = DetailReq.CurrencyCode ?? "",
                                        DiscountRate = (-Discount),
                                        HotelName = string.Empty,
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
                                    EHotelRoomDetailobj.otherserviceoffercost = new Dictionary<string, OtherServiceOffer>();

                                    if (EAminitiesLst != null && EAminitiesLst.Count != 0)
                                    {
                                        EHotelRoomDetailobj.HotelRoomAminities = EAminitiesLst.Select(g => new EHotelRoomAminities() { AminitieName = g.AminitieName }).ToList();
                                    }
                                    else
                                    {
                                        EHotelRoomDetailobj.HotelRoomAminities = HotelObj.EAminitiesLst.Select(g => new EHotelRoomAminities() { AminitieName = g.AminitieName }).ToList();
                                    }

                                    List<EHotelRoomImages> HotelRoomImages = new List<EHotelRoomImages>();

                                    if (Staticinfo?.images != null && Staticinfo.images.Count != 0)
                                    {
                                        var sort = Staticinfo.images.Where(f => f.roomCode != null && Rooms.code.Contains(f.roomCode)).ToList();
                                        if (sort != null && sort.Count != 0)
                                        {
                                            foreach (var t1 in sort)
                                            {
                                                var EHotelRoomImages_ = new EHotelRoomImages();
                                                EHotelRoomImages_.ImageActualPath = t1.path;
                                                EHotelRoomImages_.ImageThumPath = t1.path;
                                                EHotelRoomImages_.ImageName = EHotelRoomDetailobj.RoomType;
                                                EHotelRoomImages_.RoomTypeID = EHotelRoomDetailobj.RoomTypeId;
                                                EHotelRoomImages_.RoomTypeName = EHotelRoomDetailobj.RoomType;
                                                HotelRoomImages.Add(EHotelRoomImages_);
                                            }
                                        }
                                        else
                                        {
                                            foreach (var rimg in HotelObj?.EHotelImageGallaryLst ?? Enumerable.Empty<EHotelImageGallary>())
                                            {
                                                var EHotelRoomImages_ = new EHotelRoomImages(rimg);
                                                EHotelRoomImages_.RoomTypeID = EHotelRoomDetailobj.RoomTypeId;
                                                EHotelRoomImages_.RoomTypeName = EHotelRoomDetailobj.RoomType;
                                                HotelRoomImages.Add(EHotelRoomImages_);
                                            }
                                        }
                                    }

                                    EHotelRoomDetailobj.HotelRoomImages = HotelRoomImages;
                                    EHotelRoomDetailobj.SupplierRoomDefaultInfo = SupplierRoomDefaultInfoObj;
                                    EHotelRoomDetailobj.EMealPlanUpgradeInfoObj = new EMealPlanUpgradeInfo();
                                    EHotelRoomDetailobj.GMXMealPlanUpgradeInfo = new GMXMealPlanUpgradeInfo();

                                    ERoomDefaultInfo RoomDefaultInfoObj = new ERoomDefaultInfo();
                                    RoomDefaultInfoObj.EHotelMarkupInfoLst = new List<EHotelMarkupInfo>();
                                    RoomDefaultInfoObj.Purchase_CurrencyCode = hitem?.currency ?? "";
                                    RoomDefaultInfoObj.Purchase_CurrencyID = 0;
                                    RoomDefaultInfoObj.Purchase_CurrencyROE_DIV = Convert.ToDecimal("1");
                                    RoomDefaultInfoObj.Purchase_CurrencyROE_MLP = Convert.ToDecimal("1");

                                    RoomDefaultInfoObj.Purchase_FinalAllNightRate = Convert.ToDecimal(LstRoomRates.Select(q => q.net).Sum()) + Discount;
                                    RoomDefaultInfoObj.Supplier_AllNightOfferRate = Convert.ToDecimal(LstRoomRates.Select(q => q.net).Sum());
                                    RoomDefaultInfoObj.Supplier_AllNightRate = Convert.ToDecimal(LstRoomRates.Select(q => q.net).Sum()) + Discount;
                                    RoomDefaultInfoObj.Supplier_Currency = hitem?.currency ?? "";

                                    RoomDefaultInfoObj.System_AllNightOfferRate = Convert.ToDecimal(LstRoomRates.Select(q => q.net).Sum()) + Discount;
                                    RoomDefaultInfoObj.System_AllNightRate = Convert.ToDecimal(LstRoomRates.Select(q => q.net).Sum()) + Discount;

                                    RoomDefaultInfoObj.System_CurrencyCode = hitem?.currency ?? "";
                                    RoomDefaultInfoObj.System_CurrencyID = 1;
                                    RoomDefaultInfoObj.System_CurrencyROE_DIV = 1;
                                    RoomDefaultInfoObj.System_CurrencyROE_MLP = 1;
                                    RoomDefaultInfoObj.System_OneNightRate = RoomRatePer;
                                    RoomDefaultInfoObj.System_TotalMarkup = 0;
                                    RoomDefaultInfoObj.System_TotalTax = 0;
                                    RoomDefaultInfoObj.DicAllNightRate = DicAllNightTotalRate;
                                    RoomDefaultInfoObj.DicAllOfferTotalRate = DicRates1;
                                    EHotelRoomDetailobj.RoomDefaultInfo = RoomDefaultInfoObj;

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
                                    EHotelRoomDetailobj.CheckInDate = DetailReq.CheckInDate_;
                                    EHotelRoomDetailobj.CheckOutDate = DetailReq.CheckOutDate_;
                                    //RateCalculation
                                    EHotelRoomDetailLst.Add(EHotelRoomDetailobj);
                                }
                            }
                        }
                    }
                }
                HotelObj.MinOneRate = EHotelRoomDetailLst.Min(d => d.TotalRate).ToString();
                HotelObj.AllNightRate = EHotelRoomDetailLst.Min(d => d.AllNigntOfferRate).ToString();
                HotelObj.EHotelRoomDetailLst = EHotelRoomDetailLst;
                HotelObj.OtherService = new System.Data.DataTable();
            }
            return HotelObj ?? new EHotelDetail();
        }

        public async Task<APIPreBookingInfo> GetHotelBedsApiPreBookingResponseAsync(EHotelPreBookingRequest ObjEHotelPreBookingRequest, List<string> HotelExtraInfo, Dictionary<string, string> ApiCredentials, MarkupClass MarkupClassObj, string FolderName, int BookingMasterID, EHotelRoomDetail EHotelRoomDetailObj, EHotelDetailRequest Sreq)
        {
            var APIPreBookingInfoObj_ = new APIPreBookingInfo();
            var HBRateCheckLst = new List<HBRateCheck>();
            var prebookEHotelRoomDetail = new EHotelRoomDetail();

            var rateKey = new List<string>();
            var HBExtraInfo = HotelExtraInfo[0].Split('!');
            var rateType = Convert.ToString(HBExtraInfo[3]).Trim().ToUpper();

            if (rateType == "RECHECK")
            {
                foreach (var item in HotelExtraInfo)
                {
                    var HBExtraInfo1 = item.Split('!');
                    rateKey.Add(Convert.ToString(HBExtraInfo1[0]).Trim());
                }
                var PreBookingResponse = await _hotelBedsApiRequest.HotelCheckRate_Prebook(ApiCredentials, rateKey ?? new List<string>(), FolderName);
                if (PreBookingResponse != null && PreBookingResponse.hotel != null && PreBookingResponse.hotel.rooms != null && PreBookingResponse.hotel.rooms.Count != 0)
                {
                    foreach (var item in PreBookingResponse.hotel.rooms)
                    {
                        foreach (var ritem in item.rates)
                        {
                            var HBRateCheck_ = new HBRateCheck();
                            HBRateCheck_.rateComments = ritem.rateComments ?? "";
                            HBRateCheck_.rateKeys = ritem.rateKey ?? "";
                            decimal net_ = 0;
                            decimal.TryParse(ritem.net, out net_);
                            HBRateCheck_.RatePrice = net_;
                            var abc = ritem.cancellationPolicies;
                            var LancellationPolicyStr = "";
                            foreach (var r in ritem.cancellationPolicies ?? new List<CheckResponse.CancellationPolicy>())
                            {
                                var amt = r.amount;
                                DateTime CancellationDaidLineDate = new DateTime();
                                CancellationDaidLineDate = r.from;
                                LancellationPolicyStr += "From  <b>" + CancellationDaidLineDate.AddDays(_hotelApiSetting.FreeCancellationBufferPeriod).ToString("dd MMM yyyy", CultureInfo.InvariantCulture) + "</b>  Cancellation Penalty will be <b>" + Sreq.CurrencyCode + " " + amt.ToString() + "</b>  <br/>";
                                HBRateCheck_.CancellationAmount = r.amount ?? "";
                                HBRateCheck_.CancellationDate = r.from;
                            }
                            HBRateCheck_.CancellationStr = LancellationPolicyStr;
                            HBRateCheck_.boardName = ritem.boardName ?? "";
                            HBRateCheck_.boardCode = ritem.boardCode ?? "";
                            HBRateCheck_.rateCommentsId = ritem.rateCommentsId ?? "";
                            HBRateCheck_.rateType = ritem.rateType ?? "";
                            HBRateCheckLst.Add(HBRateCheck_);
                        }
                    }
                }

                if (HBRateCheckLst != null && HBRateCheckLst.Count() > 0)
                {
                    var newRate = HBRateCheckLst.Select(g => g.RatePrice).Sum();
                    var RoomDe1 = await GetPreBookingHotelRoomDetail(EHotelRoomDetailObj ?? new EHotelRoomDetail(), HBRateCheckLst, Sreq, Sreq.HotelId ?? "", EHotelRoomDetailObj?.BHRDID ?? "", MarkupClassObj, FolderName);
                    var oldRoom = EHotelRoomDetailObj;
                    if (RoomDe1 != null)
                    {
                        APIPreBookingInfoObj_.AvailableForBook = true;
                        if (RoomDe1 != null)
                        {
                            if (oldRoom.AllNigntOfferRate.ToString("0.00") != RoomDe1.AllNightTotalRate.ToString("0.00"))
                            {
                                APIPreBookingInfoObj_.After_Rate = Convert.ToDecimal(RoomDe1.AllNightTotalRate);
                                APIPreBookingInfoObj_.Before_Rate = oldRoom.AllNigntOfferRate;
                                APIPreBookingInfoObj_.IsPriceChange = true;
                                APIPreBookingInfoObj_.RawData = RoomDe1;
                                APIPreBookingInfoObj_.CancellationPolicy = RoomDe1.CanclePolicy;
                                APIPreBookingInfoObj_.IsFreeCostCancellationDate = RoomDe1.IsFreeCostCancellationDate;
                                APIPreBookingInfoObj_.CancellationDate = RoomDe1.FreeCostCancellationDate;
                                //result.ResponseLog = RoomDe1;
                            }
                            else
                            {
                                APIPreBookingInfoObj_.After_Rate = oldRoom.AllNigntOfferRate;
                                APIPreBookingInfoObj_.Before_Rate = oldRoom.AllNigntOfferRate;
                                APIPreBookingInfoObj_.IsPriceChange = false;
                                APIPreBookingInfoObj_.RawData = RoomDe1;
                                APIPreBookingInfoObj_.CancellationPolicy = RoomDe1.CanclePolicy;
                                APIPreBookingInfoObj_.IsFreeCostCancellationDate = RoomDe1.IsFreeCostCancellationDate;
                                APIPreBookingInfoObj_.CancellationDate = RoomDe1.FreeCostCancellationDate;
                            }
                        }
                        else
                        {
                            APIPreBookingInfoObj_.AvailableForBook = true;
                            APIPreBookingInfoObj_.After_Rate = oldRoom.AllNigntOfferRate;
                            APIPreBookingInfoObj_.Before_Rate = oldRoom.AllNigntOfferRate;
                            APIPreBookingInfoObj_.IsPriceChange = false;
                            APIPreBookingInfoObj_.RawData = oldRoom;
                            APIPreBookingInfoObj_.CancellationPolicy = oldRoom.CanclePolicy;
                            APIPreBookingInfoObj_.IsFreeCostCancellationDate = oldRoom.IsFreeCostCancellationDate;
                            APIPreBookingInfoObj_.CancellationDate = oldRoom.FreeCostCancellationDate;
                        }
                    }
                }
            }
            else
            {

                APIPreBookingInfoObj_.CancellationPolicy = EHotelRoomDetailObj?.CanclePolicy ?? "";
                APIPreBookingInfoObj_.After_Rate = EHotelRoomDetailObj.AllNigntOfferRate;
                APIPreBookingInfoObj_.Before_Rate = EHotelRoomDetailObj.AllNigntOfferRate;
                APIPreBookingInfoObj_.IsPriceChange = false;
                APIPreBookingInfoObj_.RawData = EHotelRoomDetailObj;
                APIPreBookingInfoObj_.IsFreeCostCancellationDate = EHotelRoomDetailObj.IsFreeCostCancellationDate;
                APIPreBookingInfoObj_.CancellationDate = Convert.ToDateTime(EHotelRoomDetailObj.FreeCostCancellationDate).AddHours((-1) * 1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                APIPreBookingInfoObj_.AvailableForBook = true;
            }

            prebookEHotelRoomDetail = (EHotelRoomDetail)APIPreBookingInfoObj_.RawData;
            return APIPreBookingInfoObj_;
        }

        public async Task<APIBookingInfo> GetHotelBedsApiBookingResponseAsync(Dictionary<string, string> ApiCredentials, EHotelBookingRequest ObjEHotelBookingRequest, EHotelDetailRequest ObjEHotelDetailRequest, EHotelRoomDetail ObjEHotelRoomDetail, string FolderPath)
        {
            var result = new APIBookingInfo();

            var rateKey = new List<string>();
            var HotelExtraInfo = (ObjEHotelRoomDetail.ExtraInfo ?? string.Empty).Split('$').ToList();
            var HBExtraInfo = HotelExtraInfo[0].Split('!');
            var rateType = Convert.ToString(HBExtraInfo[3]).Trim().ToUpper();
            var recheLst = new List<HBRateCheck>();
            bool IsProceedtobook = false;
            if (rateType == "RECHECK")
            {
                foreach (var item in HotelExtraInfo)
                {
                    var HBExtraInfo1 = item.Split('!');
                    rateKey.Add(Convert.ToString(HBExtraInfo1[0]).Trim());
                }

                var UserRoomPrice = ObjEHotelRoomDetail.SupplierRoomDefaultInfo.Supplier_AllNightOfferRate;
                recheLst = await _hotelBedsApiRequest.HotelCheckRate(ApiCredentials, rateKey, FolderPath, ObjEHotelBookingRequest.BookingID);
                if (recheLst != null && recheLst.Count > 0)
                {
                    var newRate = recheLst.Select(g => g.RatePrice).Sum();
                    if (newRate.Equals(UserRoomPrice))
                    {
                        IsProceedtobook = true;
                    }
                }
            }
            else
            {
                recheLst = new List<HBRateCheck>();
                foreach (var item in HotelExtraInfo)
                {
                    var HBExtraInfo1 = item.Split('!');
                    recheLst.Add(new HBRateCheck() { rateKeys = Convert.ToString(HBExtraInfo1[0]).Trim() });
                }
                IsProceedtobook = true;
            }
            if (IsProceedtobook)
            {
                result = await _hotelBedsApiRequest.HotelBooking(ApiCredentials, ObjEHotelBookingRequest.RoomWisePaxInfo, ObjEHotelDetailRequest.TotalNoRoom, recheLst ?? new List<HBRateCheck>(), string.Empty, FolderPath, ObjEHotelBookingRequest.BookingID).ConfigureAwait(false);
            }

            return result;
        }

        public async Task<APICancelationInfo> GetHotelBedsApiCancelResponseAsync(Dictionary<string, string> ApiCredentials, string bookingreference, string FolderPath, int BookingID)
        {
            var APICancelationInfoObj = new APICancelationInfo();
            var RequestLog = new Dictionary<string, string>();
            var ResponseLog = new Dictionary<string, string>();

            RequestLog.Add("HB_HotelCancelRequest", Convert.ToString(ApiCredentials["HotelBedsAPIURL"]) + "hotel-api/1.0/bookings/" + bookingreference + "?cancellationFlag=CANCELLATION");
            var bookingDetailStr = await _hotelBedsApiRequest.GetResponseRestClient(Method.Delete, Convert.ToString(ApiCredentials["HotelBedsAPIURL"]) + "hotel-api/1.0/bookings/" + bookingreference + "?cancellationFlag=CANCELLATION", string.Empty, ApiCredentials);
            ResponseLog.Add("HB_HotelCancelResponse",  JsonConvert.SerializeObject(bookingDetailStr));
            APICancelationInfoObj.RequestLog = RequestLog;
            APICancelationInfoObj.ResponseLog = ResponseLog;

            var result = JsonConvert.DeserializeObject<HotelBedsBookingDetailsClass.HotelBedsBookingDetail>(bookingDetailStr);
            if (result != null && result.booking != null && result.booking.hotel != null && !string.IsNullOrEmpty(result.booking.status))
            {
                if (result.booking.status.Equals("CANCELLED"))
                {
                    APICancelationInfoObj.CancellationCharges = result.booking.hotel.cancellationAmount;
                    APICancelationInfoObj.CancellationNo = result.booking.reference ?? string.Empty;
                    APICancelationInfoObj.CancellationStatus = EtosApiServices.Model.BookingStatus.BookingCancel;                  
                }
            }
            return APICancelationInfoObj;
        }

        public async Task<EHotelRoomDetail> GetPreBookingHotelRoomDetail(EHotelRoomDetail OldEHRDetails, List<HBRateCheck> roomDet, EHotelDetailRequest Sreq, string Hotelid, string BHRDID, MarkupClass MarkupClassObj, string FolderPath)
        {
            var EHotelRoomDetailobj = new EHotelRoomDetail();
            string Result = string.Empty;

            try
            {
                decimal APIROE = await _iapiCredentials.GetCurrancyRate(OldEHRDetails.Currency ?? "", Sreq.ClientID, Sreq.DBConnectioString ?? "");
                decimal UserROE = await _iapiCredentials.GetCurrancyRate(Sreq.CurrencyCode ?? "", Sreq.ClientID, Sreq.DBConnectioString ?? "");
                string APICurrencyName = OldEHRDetails.Currency ?? "";

                if (roomDet != null && roomDet.Count > 0)
                {
                    int RoomIndex_ = 1;
                    EHotelRoomDetailobj.BHRDID = BHRDID;
                    EHotelRoomDetailobj.ServiceAPIID = ApiSupplierId;
                    EHotelRoomDetailobj.RoomBasicClassType = string.Empty;
                    EHotelRoomDetailobj.RoomIndex = RoomIndex_;
                    EHotelRoomDetailobj.RoomNo = Sreq.TotalNoRoom;
                    EHotelRoomDetailobj.HotelID = Sreq.HotelId ?? "";
                    EHotelRoomDetailobj.RoomType = string.Empty;
                    EHotelRoomDetailobj.RoomTypeId = string.Empty;
                    EHotelRoomDetailobj.RoomMeals = string.Empty;
                    EHotelRoomDetailobj.RoomMealsID = "0";
                    EHotelRoomDetailobj.RoomOccupancyDetails = Sreq.RoomOccupancyDetails ?? new List<ERoomOccupancyInfo>();
                    EHotelRoomDetailobj.IsOffer = false;
                    EHotelRoomDetailobj.OfferName = string.Empty;
                    EHotelRoomDetailobj.OfferCount = 0;
                    EHotelRoomDetailobj.CheckOutDate = Sreq.CheckOutDate_;
                    EHotelRoomDetailobj.CheckInDate = Sreq.CheckInDate_;

                    string APIResponseFilePath = string.Empty;
                    Dictionary<string, string> SMYRoomsExtraInfo = new Dictionary<string, string>();
                    SMYRoomsExtraInfo.Add("RawFile", APIResponseFilePath);
                    SMYRoomsExtraInfo.Add("RIndex", String.Empty);
                    SMYRoomsExtraInfo.Add("BHRDID", BHRDID);
                    SMYRoomsExtraInfo.Add("OfferCode", string.Empty);

                    decimal DefaultAllNightRate = 0, AllNightTotalRate = 0;
                    decimal AllNightOfferTotalRate = roomDet.Sum(f => f.RatePrice);
                    decimal RoomRatePer = (AllNightOfferTotalRate / Sreq.Nights) / Sreq.TotalNoRoom;
                    RoomRatePer = (RoomRatePer / APIROE) * UserROE;
                    AllNightOfferTotalRate = (AllNightOfferTotalRate / APIROE) * UserROE;
                    decimal Discount = 0;
                    Discount = (Discount / APIROE) * UserROE;
                    if (Discount.Equals(0) && false)
                    {
                        EHotelRoomDetailobj.IsOffer = true;
                    }
                    AllNightTotalRate = AllNightOfferTotalRate + Discount;

                    var perRateExtraInfo = new List<string>();
                    foreach (var ExtInfo in roomDet ?? Enumerable.Empty<HBRateCheck>())
                    {
                        var fields = new List<string>
                                        {
                                            ExtInfo?.rateKeys ?? string.Empty,       //0
                                            ExtInfo?.rateCommentsId ?? string.Empty, //1
                                            ExtInfo?.boardCode ?? string.Empty,      //2
                                            ExtInfo?.rateType ?? string.Empty,       //3
                                            BHRDID ?? string.Empty                   //4
                                        };
                        perRateExtraInfo.Add(string.Join("!", fields));
                    }
                    EHotelRoomDetailobj.ExtraInfo = string.Join("$", perRateExtraInfo);

                    var DayWiseRatesLst = new List<EDateWiseRoomRate>();
                    for (int k = 0; k < Sreq.Nights; k++)
                    {
                        var OneDayRate = RoomRatePer;
                        var DicRates1sd = APIMarkupSection.AddMarkupByRule(ApiSupplierId, Convert.ToDecimal(OneDayRate), MarkupClassObj, APICurrencyName, 1, Sreq.TotalNoRoom);
                        DayWiseRatesLst.Add(new EDateWiseRoomRate() { Date = Sreq.CheckInDate_.AddDays(k).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), Rate = Convert.ToDecimal((Convert.ToDouble(DicRates1sd[APIMarkupSection.AR]))) });
                    }

                    var DicRates1 = APIMarkupSection.AddMarkupByRule(ApiSupplierId, Convert.ToDecimal(AllNightOfferTotalRate), MarkupClassObj, APICurrencyName, Sreq.Nights, Sreq.TotalNoRoom);
                    AllNightOfferTotalRate = (Convert.ToDecimal(DicRates1[APIMarkupSection.AR]));
                    DefaultAllNightRate = AllNightOfferTotalRate;

                    var DicAllNightTotalRate = APIMarkupSection.AddMarkupByRule(ApiSupplierId, Convert.ToDecimal(AllNightTotalRate), MarkupClassObj, APICurrencyName, Sreq.Nights, Sreq.TotalNoRoom);
                    AllNightTotalRate = (Convert.ToDecimal(DicAllNightTotalRate[APIMarkupSection.AR]));
                    EHotelRoomDetailobj.TotalTax = ((Convert.ToDecimal(DicAllNightTotalRate[APIMarkupSection.TXV])));
                    EHotelRoomDetailobj.IsTaxType = Convert.ToBoolean(DicAllNightTotalRate[APIMarkupSection.TXO]); Discount = Convert.ToDecimal(AllNightTotalRate) - Convert.ToDecimal(DefaultAllNightRate);
                    decimal DefaultTotalRate = roomDet.Sum(f => f.RatePrice);

                    string SupplierName = APISupplierName;
                    int SupplierMasterID = ApiSupplierId;

                    ESupplierRoomDefaultInfo SupplierRoomDefaultInfoObj = new ESupplierRoomDefaultInfo();
                    SupplierRoomDefaultInfoObj.Supplier_AllNightOfferRate = DefaultTotalRate;
                    SupplierRoomDefaultInfoObj.Supplier_AllNightRate = DefaultTotalRate;
                    SupplierRoomDefaultInfoObj.Supplier_Currency = APICurrencyName;
                    SupplierRoomDefaultInfoObj.Supplier_ID = SupplierMasterID;
                    SupplierRoomDefaultInfoObj.Supplier_MinNightRate = DefaultTotalRate / Sreq.Nights;
                    SupplierRoomDefaultInfoObj.Supplier_Name = SupplierName;
                    SupplierRoomDefaultInfoObj.SupplierCode = APIName;

                    EHotelRoomDetailobj.Currency = Sreq.CurrencyCode ?? string.Empty;
                    EHotelRoomDetailobj.Supplier = SupplierName;
                    EHotelRoomDetailobj.IsSupplier = true;
                    EHotelRoomDetailobj.SupplierName = SupplierName;

                    EHotelRoomDetailobj.IsFreeCostCancellationDate = false;
                    DateTime LastCancellationDate = new DateTime();
                    LastCancellationDate = DateTime.Now;
                    string LancellationPolicyStr = string.Empty;

                    var freedate = roomDet[0].CancellationDate.AddDays(_hotelApiSetting.FreeCancellationBufferPeriod).ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
                    EHotelRoomDetailobj.FreeCostCancellationDate = Convert.ToDateTime(freedate).AddHours((-1) * 1).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                    DateTime todaysdate = DateTime.Now;
                    string dateString = EHotelRoomDetailobj.FreeCostCancellationDate;
                    string format = "dd/MM/yyyy"; // Change this format to match your date string format

                    if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                    {
                        if (parsedDate >= todaysdate)
                        {
                            EHotelRoomDetailobj.IsFreeCostCancellationDate = true;
                        }
                    }

                    EHotelRoomDetailobj.IsCanclePolicy = true;
                    EHotelRoomDetailobj.CanclePolicy = string.Join("<br>", roomDet.Select(f => f.CancellationStr));
                    EHotelRoomDetailobj.Errata = string.Empty;
                    EHotelRoomDetailobj.IsErrata = false;
                    EHotelRoomDetailobj.AvilableRoom = Sreq.TotalNoRoom;
                    EHotelRoomDetailobj.Issharedbedding = false;
                    EHotelRoomDetailobj.IsExtraBed = false;
                    EHotelRoomDetailobj.IsMealPlanUpgrad = false;

                    EViewCalculation ViewCalculation = new EViewCalculation()
                    {
                        Currency = Sreq.CurrencyCode ?? string.Empty,
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
                    RoomDefaultInfoObj.Purchase_CurrencyCode = APICurrencyName;
                    RoomDefaultInfoObj.Purchase_CurrencyID = 0;
                    RoomDefaultInfoObj.Purchase_CurrencyROE_DIV = Convert.ToDecimal("1");
                    RoomDefaultInfoObj.Purchase_CurrencyROE_MLP = Convert.ToDecimal("1");
                    RoomDefaultInfoObj.Purchase_FinalAllNightRate = DefaultTotalRate;
                    RoomDefaultInfoObj.Supplier_AllNightOfferRate = DefaultTotalRate;
                    RoomDefaultInfoObj.Supplier_AllNightRate = DefaultTotalRate;
                    RoomDefaultInfoObj.Supplier_Currency = APICurrencyName;
                    RoomDefaultInfoObj.System_AllNightOfferRate = DefaultTotalRate;
                    RoomDefaultInfoObj.System_AllNightRate = DefaultTotalRate;
                    RoomDefaultInfoObj.System_CurrencyCode = APICurrencyName;
                    RoomDefaultInfoObj.System_CurrencyID = 1;
                    RoomDefaultInfoObj.System_CurrencyROE_DIV = 1;
                    RoomDefaultInfoObj.System_CurrencyROE_MLP = 1;
                    RoomDefaultInfoObj.System_OneNightRate = RoomRatePer;
                    RoomDefaultInfoObj.System_TotalMarkup = 0;
                    RoomDefaultInfoObj.System_TotalTax = 0;
                    RoomDefaultInfoObj.DicAllNightRate = DicAllNightTotalRate;
                    RoomDefaultInfoObj.DicAllOfferTotalRate = DicRates1;
                    EHotelRoomDetailobj.RoomDefaultInfo = RoomDefaultInfoObj;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                //File.WriteAllText(FolderPath + "/AfterValuationResponse" + Guid.NewGuid().ToString().Replace('-', '_') + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss", CultureInfo.CreateSpecificCulture("en-US")) + ".txt", Convert.ToString(EHotelRoomDetailobj));
            }
            return EHotelRoomDetailobj;
        }

    }
}
