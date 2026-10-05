using System;
using System.Collections.Generic;
using System.Data;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Model
{
    public class EHotelDetail
    {
        public EHotelDetail()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public EHotelDetail(List<EHotelDetail> EHotelDetailLst, EHotelDetailRequest req)
        {
            var ObjEHotelRoomDetailLst = new List<EHotelRoomDetail>();
            var EHotelImageGallaryObjLst = new List<EHotelImageGallary>();
            var EAminitiesObjLst = new List<EAminities>();
            int RoomIndex = 0;

            if (EHotelDetailLst != null && EHotelDetailLst.Count > 0)
            {
                foreach (var item in EHotelDetailLst)
                {
                    if (item.EHotelImageGallaryLst != null)
                    {
                        foreach (var item1 in item.EHotelImageGallaryLst)
                        {
                            EHotelImageGallaryObjLst.Add(item1);
                        }
                    }
                    if (item.EAminitiesLst != null)
                    {
                        foreach (var item1 in item.EAminitiesLst)
                        {
                            EAminitiesObjLst.Add(item1);
                        }
                    }
                    if (item.EHotelRoomDetailLst != null)
                    {
                        foreach (var item1 in item.EHotelRoomDetailLst)
                        {
                            var ghrd = item1;
                            ghrd.RoomIndex = RoomIndex;
                            var RawHotelRoomSHAObj = new RawHotelRoomSHA();
                            RawHotelRoomSHAObj.CheckInDate = req.CheckInDate_.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            RawHotelRoomSHAObj.CheckOutDate = req.CheckOutDate_.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            RawHotelRoomSHAObj.AdultStr = string.Join("-", req.RoomOccupancyDetails.Select(f => f.AdultCount));
                            RawHotelRoomSHAObj.ChildStr = string.Join("-", req.RoomOccupancyDetails.Select(f => f.ChildCount));
                            RawHotelRoomSHAObj.FreeCancellationDate = ghrd.FreeCostCancellationDate;
                            RawHotelRoomSHAObj.HotelID = ghrd.HotelID;
                            RawHotelRoomSHAObj.HotelName = item.Name ?? "";
                            RawHotelRoomSHAObj.IsCancellation = Convert.ToString(ghrd.IsCanclePolicy);
                            RawHotelRoomSHAObj.IsFreeCancellation = Convert.ToString(ghrd.IsFreeCostCancellationDate);
                            RawHotelRoomSHAObj.RoomCount = Convert.ToString(req.TotalNoRoom);
                            RawHotelRoomSHAObj.RoomMeal = ghrd.RoomMeals;
                            RawHotelRoomSHAObj.RoomName = string.Format("{0} {1}", ghrd.RoomBasicClassType, ghrd.RoomType);
                            RawHotelRoomSHAObj.IsOffer = ghrd.IsOffer ? "1" : "0";
                            RawHotelRoomSHAObj.OfferName = ghrd.OfferName;
                            RawHotelRoomSHAObj.RoomIndex = Convert.ToString(ghrd.RoomIndex);
                            ghrd.RawHotelRoomSHA = RawHotelRoomSHAObj;
                            ghrd.RoomSHA = RawHotelRoomSHAObj.GetConcateString();
                            ObjEHotelRoomDetailLst.Add(item1);
                            RoomIndex++;
                        }
                    }
                }

                var EhotelDetailg = new EHotelDetail();
                EhotelDetailg = EHotelDetailLst.FirstOrDefault();
                this.Name = EhotelDetailg?.Name;
                this.CheckInTime = EhotelDetailg?.CheckInTime;
                this.CheckOutTime = EhotelDetailg?.CheckOutTime;
                this.Address = EhotelDetailg?.Address;
                this.location = EhotelDetailg?.location;
                this.StarRating = EhotelDetailg?.StarRating;
                this.HotelCode = EhotelDetailg?.HotelCode;
                this.latitude = EhotelDetailg?.latitude;
                this.longitude = EhotelDetailg?.longitude;
                this.CityName = EhotelDetailg?.CityName;
                this.MainImage = EhotelDetailg?.MainImage;
                this.Currency = EhotelDetailg?.Currency;
                this.MinOneRate = EhotelDetailg?.MinOneRate;
                this.AllNightRate = EhotelDetailg?.AllNightRate;
                this.Supplier = EhotelDetailg?.Supplier;
                this.HotelString = EhotelDetailg?.HotelString;
                this.Is_Offer = EhotelDetailg?.Is_Offer;
                this.Property_Type = EhotelDetailg?.Property_Type;
                this.EHotelImageGallaryLst = EHotelImageGallaryObjLst;
                this.EAminitiesLst = EAminitiesObjLst;
                this.Description = EhotelDetailg?.Description;
                this.HotelContactDetails = EhotelDetailg?.HotelContactDetails;
                this.HotelEmail = EhotelDetailg?.HotelEmail;
                this.HotelURL = EhotelDetailg?.HotelURL;
                this.HotelDetail = EhotelDetailg?.HotelDetail;
                this.HDType = EtosApiServices.Hotel.Model.HDType.Giata;
                this.GiataID = EhotelDetailg?.GiataID;
                this.GiataCityID = EhotelDetailg?.GiataCityID;
                this.Transportation = EhotelDetailg?.Transportation;
                this.EHotelRoomDetailLst = ObjEHotelRoomDetailLst;
            }
        }

        public EHotelDetail(HotelBedsModelClass.HotelBedsHotelDetail.Hotel hotelstaticobj, List<EHotelRoomDetail> EHotelRoomDetailLst_, string UserCurrency, string APICode)
        {
            if (hotelstaticobj != null)
            {
                try
                {

                    this.Name = hotelstaticobj.name?.content ?? string.Empty;
                    this.Address = hotelstaticobj.address?.content ?? string.Empty;
                    this.location = hotelstaticobj.city?.content ?? string.Empty;
                    int SRating_ = 0;
                    int.TryParse(Convert.ToString(hotelstaticobj.ranking), out SRating_);
                    this.StarRating = Convert.ToString(SRating_);
                    this.HotelCode = hotelstaticobj.code.ToString();
                    this.latitude = Convert.ToString(hotelstaticobj?.coordinates?.latitude);
                    this.longitude = Convert.ToString(hotelstaticobj?.coordinates?.longitude);

                    var EHotelImageGallaryLst_ = new List<EHotelImageGallary>();
                    if (hotelstaticobj.images != null && hotelstaticobj.images.Count() > 0)
                    {
                        foreach (var img in hotelstaticobj.images)
                        {
                            var EHotelImageGallaryObj = new EHotelImageGallary();
                            EHotelImageGallaryObj.HotelId = this.HotelCode;
                            EHotelImageGallaryObj.HotelName = this.Name;
                            EHotelImageGallaryObj.ImageActualPath = img.path ?? "";
                            EHotelImageGallaryObj.ImageName = img.imageTypeCodeDes ?? "";
                            EHotelImageGallaryObj.ImageThumPath = img.path ?? "";
                            EHotelImageGallaryLst_.Add(EHotelImageGallaryObj);
                        }
                        if (EHotelImageGallaryLst_ != null && EHotelImageGallaryLst_.Count > 0)
                        {
                            this.MainImage = EHotelImageGallaryLst_.FirstOrDefault().ImageActualPath;
                        }
                    }
                    this.Currency = UserCurrency;
                    this.Supplier = APICode;
                    this.HotelString = string.Format("{0}~{1}", APICode, hotelstaticobj.code);
                    this.Is_Offer = "0";
                    this.Property_Type = hotelstaticobj.ranking.ToString();
                    this.EHotelImageGallaryLst = EHotelImageGallaryLst_;
                    var EAminitiesLst_ = new List<EAminities>();
                    if (hotelstaticobj.facilities != null && hotelstaticobj.facilities.Count() > 0)
                    {
                        foreach (var am in hotelstaticobj.facilities)
                        {
                            var EAminitiesObj = new EAminities();
                            EAminitiesObj.AminitieName = am.facilityCodeDesc ?? "";
                            EAminitiesObj.AminitieCategory = am.facilityGroupCode.ToString() ?? "";
                            EAminitiesLst_.Add(EAminitiesObj);
                        }
                    }
                    this.EAminitiesLst = EAminitiesLst_;
                    this.Description = hotelstaticobj.description?.content ?? string.Empty;
                    this.HotelContactDetails = string.Join(',', hotelstaticobj?.phones?.Select(f => f.phoneNumber ?? string.Empty));
                    this.HotelEmail = hotelstaticobj.email ?? string.Empty;
                    this.HotelURL = string.Empty;
                    this.HDType = EtosApiServices.Hotel.Model.HDType.Normal;
                    this.GiataID = string.Empty;
                    this.GiataCityID = string.Empty;
                    this.CheckInTime = string.Empty;
                    this.CheckOutTime = string.Empty;
                    this.CurrentDeals = string.Empty;
                    this.TotalTax = 0;
                    this.OtherService = new DataTable();
                    this.OfferInclusion = string.Empty;
                    if (EHotelRoomDetailLst_ != null && EHotelRoomDetailLst_.Count != 0)
                    {
                        //var lowroom = EHotelRoomDetailLst_.OrderBy(g => g.TotalRate).FirstOrDefault();
                        var lowroom = EHotelRoomDetailLst_.OrderBy(g => g.TotalRate).FirstOrDefault();
                        this.MinOneRate = lowroom.TotalRate.ToString("0.00");
                        this.AllNightRate = lowroom.AllNigntOfferRate.ToString("0.00");
                        this.AllNightRate = Convert.ToString(EHotelRoomDetailLst_.AsEnumerable().Select(pp => pp.AllNightTotalRate).Min());
                        this.Is_Offer = EHotelRoomDetailLst_.Where(g => g.IsOffer).ToList().Count > 0 ? "1" : "0";
                        this.EHotelRoomDetailLst = EHotelRoomDetailLst_;
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        public EHotelDetail(TBOModelClass.Tbov7HotelGiataHotelCodeList.Hotel hotelstaticobj, List<EHotelRoomDetail> EHotelRoomDetailLst_, string UserCurrency, string APICode)
        {
            if (hotelstaticobj != null)
            {
                try
                {
                    this.Name = hotelstaticobj.HotelName;
                    this.Address = hotelstaticobj.HotelAddress;
                    this.location = hotelstaticobj.CityName;
                    int SRating_ = 0;
                    int.TryParse(Convert.ToString(hotelstaticobj.StarRating), out SRating_);
                    this.StarRating = Convert.ToString(SRating_);
                    this.HotelCode = hotelstaticobj.HotelCode;
                    this.latitude = Convert.ToString(hotelstaticobj.Latitude);
                    this.longitude = Convert.ToString(hotelstaticobj.Longitude);

                    var EHotelImageGallaryLst_ = new List<EHotelImageGallary>();
                    if (hotelstaticobj.HotelDetails != null && hotelstaticobj.HotelDetails.ImageUrls != null && hotelstaticobj.HotelDetails.ImageUrls.ImageUrl != null)
                    {
                        foreach (var img in hotelstaticobj.HotelDetails.ImageUrls.ImageUrl)
                        {
                            var EHotelImageGallaryObj = new EHotelImageGallary();
                            EHotelImageGallaryObj.HotelId = this.HotelCode;
                            EHotelImageGallaryObj.HotelName = this.Name;
                            EHotelImageGallaryObj.ImageActualPath = img;
                            EHotelImageGallaryObj.ImageName = hotelstaticobj.HotelName;
                            EHotelImageGallaryObj.ImageThumPath = img;
                            EHotelImageGallaryLst_.Add(EHotelImageGallaryObj);
                        }
                        if (EHotelImageGallaryLst_ != null && EHotelImageGallaryLst_.Count > 0)
                        {
                            this.MainImage = EHotelImageGallaryLst_.FirstOrDefault().ImageActualPath?? string.Empty;
                        }
                    }
                    this.Currency = UserCurrency;
                    this.Supplier = APICode;
                    this.HotelString = string.Format("{0}~{1}", APICode, hotelstaticobj.HotelCode);
                    this.Is_Offer = "0";
                    this.Property_Type = hotelstaticobj.StarRating;
                    this.EHotelImageGallaryLst = EHotelImageGallaryLst_;
                    var EAminitiesLst_ = new List<EAminities>();
                    if (hotelstaticobj.HotelDetails != null && hotelstaticobj.HotelDetails.HotelFacilities != null && hotelstaticobj.HotelDetails.HotelFacilities.HotelFacility != null)
                    {
                        foreach (var am in hotelstaticobj.HotelDetails.HotelFacilities.HotelFacility)
                        {
                            var EAminitiesObj = new EAminities();
                            EAminitiesObj.AminitieName = am;
                            EAminitiesObj.AminitieCategory = am;
                            EAminitiesLst_.Add(EAminitiesObj);
                        }
                    }
                    this.EAminitiesLst = EAminitiesLst_;
                    this.Description = hotelstaticobj?.HotelDetails?.Description?? string.Empty;
                    this.HotelContactDetails = hotelstaticobj?.HotelDetails?.PhoneNumber;
                    this.HotelEmail = hotelstaticobj?.HotelDetails?.Email;
                    this.HotelURL = string.Empty;
                    this.HDType = EtosApiServices.Hotel.Model.HDType.Normal;
                    this.GiataID = string.Empty;
                    this.GiataCityID = string.Empty;
                    this.CheckInTime = string.Empty;
                    this.CheckOutTime = string.Empty;
                    this.CurrentDeals = string.Empty;
                    this.TotalTax = 0;
                    this.OtherService = new DataTable();
                    this.OfferInclusion = string.Empty;
                    if (EHotelRoomDetailLst_ != null && EHotelRoomDetailLst_.Count != 0)
                    {
                        //var lowroom = EHotelRoomDetailLst_.OrderBy(g => g.TotalRate).FirstOrDefault();
                        var lowroom = EHotelRoomDetailLst_.OrderBy(g => g.TotalRate).FirstOrDefault();
                        this.MinOneRate = lowroom?.TotalRate.ToString("0.00");
                        this.AllNightRate = lowroom?.AllNigntOfferRate.ToString("0.00");
                        this.AllNightRate = Convert.ToString(EHotelRoomDetailLst_.AsEnumerable().Select(pp => pp.AllNightTotalRate).Min());
                        this.Is_Offer = EHotelRoomDetailLst_.Where(g => g.IsOffer).ToList().Count > 0 ? "1" : "0";
                        this.EHotelRoomDetailLst = EHotelRoomDetailLst_;
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        public string? Name { get; set; }//Name
        public string? Address { get; set; }//Address 
        public string? location { get; set; }//Address Near location
        public string? StarRating { get; set; }//star Rating
        public string? HotelCode { get; set; }//ID
        public string? latitude { get; set; }//latitude
        public string? longitude { get; set; }//longitude
        public string? CityName { get; set; }//City Name

        private string? _MainImage;
        public string? MainImage// main img
        {
            get => _MainImage;
            set => _MainImage = value ?? string.Empty;
        }
        public string? Currency { get; set; }//currency
        public string? MinOneRate { get; set; }//min per night rate
        public string? AllNightRate { get; set; }//aproximal all night rate
        public string? Supplier { get; set; }//supplier
        public string? SupplierAliasName { get; set; }//supplier
        public string? HotelString { get; set; }//hotelstring api~hotelid i.e. hc
        public string? Is_Offer { get; set; }//Is Offer
        public string? Property_Type { get; set; }//Is Offer
        public List<EHotelImageGallary>? EHotelImageGallaryLst { get; set; }
        public List<EAminities>? EAminitiesLst { get; set; }

        private string? _Giata_F;
        public string? Giata_F
        {
            get => _Giata_F;

            set
            {
                if (value == null)
                {
                    _Giata_F = string.Empty;
                    return;
                }

                if (value.Contains('!'))
                {
                    var parts = value.Split(new[] { '!' }, StringSplitOptions.RemoveEmptyEntries);
                    var sb = new StringBuilder();

                    for (int i = 0; i < parts.Length; i += 4)
                    {
                        sb.Append("<div class='col-sm-12'>");
                        for (int j = i; j < Math.Min(i + 4, parts.Length); j++)
                        {
                            sb.Append(parts[j]);
                        }
                        sb.Append("</div>");
                    }

                    _Giata_F = sb.ToString();
                }
                else
                {
                    _Giata_F = value;
                }
            }
        }
        public string? Description { get; set; }

        private string? _hotelcontactdetails;
        public string? HotelContactDetails
        {
            get => _hotelcontactdetails;
            set => _hotelcontactdetails = value ?? string.Empty;
        }
        public string? HotelEmail { get; set; }
        public string? HotelURL { get; set; }
        public object? HotelDetail { get; set; }
        public HDType? HDType { get; set; }
        public string? GiataID { get; set; }//Is Offer
        public string? GiataCityID { get; set; }//Is Offer
        public List<EHotelRoomDetail>? EHotelRoomDetailLst { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string? CurrentDeals { get; set; }
        public decimal TotalTax { get; set; }//aproximal all night rate
        public DataTable? OtherService { get; set; }
        public string? OfferInclusion { get; set; }
        public int IsHotelPackage { get; set; }
        public string? Transportation { get; set; }
        public Dictionary<string, List<EHotelRoomDetail>>? EHotelRoomSRDetailLst { get; set; }
        public Dictionary<string, decimal>? DicOtherServiceOffer { get; set; }
        public DataTable? EarlyCheckInLateCheckout { get; set; }
        public DataTable? MealInclusionExclusion { get; set; }
        public string? EarlyCheckInLateCheckoutjson { get; set; }
        public string? MealInclusionExclusionjson { get; set; }
    }

    public enum HDType
    {
        Normal = 0,
        Giata = 1
    }

    public abstract class EImageGallary
    {
        public string? ImageThumPath { get; set; }
        public string? ImageActualPath { get; set; }
        public string? ImageName { get; set; }
    }

    public class EHotelImageGallary : EImageGallary
    {
        public EHotelImageGallary()
        {
        }

        public EHotelImageGallary(EHotelImageGallary HotelImageGallaryObj)
        {
            this.HotelId = HotelImageGallaryObj.HotelId;
            this.HotelName = HotelImageGallaryObj.HotelName;
            this.ImageActualPath = HotelImageGallaryObj.ImageName;
            this.ImageName = HotelImageGallaryObj.ImageName;
            this.ImageThumPath = HotelImageGallaryObj.ImageName;
        }
        public string? HotelId { get; set; }
        public string? HotelName { get; set; }
    }

    public class EAminities
    {
        public EAminities()
        {
            img = string.Empty;
            _AminitieName = string.Empty;
            AminitieCategory = string.Empty;
        }
        public int fcid { get; set; }//Facility ID
        public string img { get; set; }

        private string FacilityImage = "room-check.png";

        private string _AminitieName;

        public int _issh = 0;
        public string AminitieName
        {
            get
            {
                return _AminitieName;
            }
            set
            {
                _AminitieName = value;
                this.img = GetFacilityImages(value, out _issh);
            }
        }
        public int topAminities { get; set; }
        public string AminitieCategory { get; set; }
        private string GetFacilityImages(string? Facility, out int issh)
        {
            issh = 0;
            string Result = FacilityImage;
            if (string.IsNullOrWhiteSpace(Facility))
            {
                return Result;
            }

            switch (Facility.Trim())
            {
                case "Free Wifi":
                    fcid = 0;
                    issh = 1;
                    Result = "internet.png";
                    break;
                case "Wifi":
                    fcid = 0;
                    issh = 1;
                    Result = "internet.png";
                    break;
                case "Internet access":
                    fcid = 0;
                    issh = 1;
                    Result = "internet.png";
                    break;
                case "Internet Cafe":
                    fcid = 0;
                    issh = 1;
                    Result = "internet.png";
                    break;
                case "Free Internet access (Lounge)":
                    fcid = 0;
                    issh = 1;
                    Result = "internet.png";
                    break;
                case "Housekeeping":
                    fcid = 4;
                    issh = 1;
                    Result = "housekeeping.png";
                    break;
                case "TV":
                    fcid = 5;
                    issh = 1;
                    Result = "television.png";
                    break;
                case "Flat Tv Screen":
                    fcid = 5;
                    issh = 1;
                    Result = "television.png";
                    break;
                case "24H. Check In":
                    fcid = 6;
                    issh = 1;
                    Result = FacilityImage;
                    break;
                case "Early Check-in":
                    fcid = 6;
                    issh = 1;
                    Result = FacilityImage;
                    break;
                case "Dry cleaning service":
                    fcid = 7;
                    issh = 1;
                    Result = "roomservice.png";
                    break;
                case "Room Service":
                    fcid = 7;
                    issh = 1;
                    Result = "roomservice.png";
                    break;
                case "Wake-Up service":
                    fcid = 7;
                    issh = 1;
                    Result = "roomservice.png";
                    break;
                case "Maid service":
                    fcid = 7;
                    issh = 1;
                    Result = "roomservice.png";
                    break;
                case "24 Hours 24 Hours front desk facility ":
                    fcid = 7;
                    issh = 1;
                    Result = "roomservice.png";
                    break;
                case "24 H Room Service":
                    fcid = 7;
                    issh = 1;
                    Result = "roomservice.png";
                    break;
                case "Concierge":
                    fcid = 8;
                    issh = 1;
                    Result = "concierge.png";
                    break;
                case "Spa":
                    fcid = 9;
                    issh = 1;
                    Result = "spa.png";
                    break;
                case "Health Spa":
                    fcid = 9;
                    issh = 1;
                    Result = "spa.png";
                    break;
                case "Wellness center":
                    fcid = 9;
                    issh = 1;
                    Result = "spa.png";
                    break;
                case "Hamam":
                    fcid = 9;
                    issh = 1;
                    Result = "spa.png";
                    break;
                case "Newspapers":
                    fcid = 10;
                    issh = 1;
                    Result = "newspapers.png";
                    break;
                case "Free newspaper":
                    fcid = 10;
                    issh = 1;
                    Result = "newspapers.png";
                    break;
                case "News Stand":
                    fcid = 10;
                    issh = 1;
                    Result = "newspapers.png";
                    break;
                case "Sauna":
                    fcid = 11;
                    issh = 1;
                    Result = "sauna.png";
                    break;
                case "Minibar":
                    fcid = 12;
                    issh = 1;
                    Result = "minibar.png";
                    break;
                case "Laundry Service":
                    fcid = 13;
                    issh = 1;
                    Result = "laundryservice.png";
                    break;
                case "Shuttle service":
                    fcid = 14;
                    issh = 1;
                    Result = FacilityImage;
                    break;
                case "Shuttle to airport":
                    fcid = 14;
                    issh = 1;
                    Result = FacilityImage;
                    break;
                case "Adult Only":
                    fcid = 15;
                    issh = 1;
                    Result = FacilityImage;
                    break;
                case "Self Parking":
                    fcid = 16;
                    issh = 1;
                    Result = "parking.png";
                    break;
                case "RV/Truck parking":
                    fcid = 16;
                    issh = 1;
                    Result = "parking.png";
                    break;
                case "Car parking":
                    fcid = 16;
                    issh = 1;
                    Result = "parking.png";
                    break;
                case "Coach parking":
                    fcid = 16;
                    issh = 1;
                    Result = "parking.png";
                    break;
                case "Valet parking":
                    fcid = 16;
                    issh = 1;
                    Result = "parking.png";
                    break;
                case "Parking":
                    fcid = 16;
                    issh = 1;
                    Result = "parking.png";
                    break;
                case "Free car parking":
                    fcid = 16;
                    issh = 1;
                    Result = "parking.png";
                    break;
                case "Outdoor pool":
                    fcid = 17;
                    issh = 1;
                    Result = "swimming.png";
                    break;
                case "Children’s Pool":
                    fcid = 17;
                    issh = 1;
                    Result = "swimming.png";
                    break;
                case "Pool bar":
                    fcid = 17;
                    issh = 1;
                    Result = "swimming.png";
                    break;
                case "Swimming pool":
                    fcid = 17;
                    issh = 1;
                    Result = "swimming.png";
                    break;
                case "Indoor pool":
                    fcid = 17;
                    issh = 1;
                    Result = "swimming.png";
                    break;
                case "Salt water pool":
                    fcid = 17;
                    issh = 1;
                    Result = "swimming.png";
                    break;
                case "Kids pool":
                    fcid = 17;
                    issh = 1;
                    Result = "swimming.png";
                    break;
                case "Coffee":
                    fcid = 18;
                    issh = 1;
                    Result = "cafeteria.png";
                    break;
                case "Cafe":
                    fcid = 18;
                    issh = 1;
                    Result = "cafeteria.png";
                    break;
                case "Restaurants & Bars Overview":
                    fcid = 18;
                    issh = 1;
                    Result = "cafeteria.png";
                    break;
                case "Bar/Snack/CafEn'":
                    fcid = 18;
                    issh = 1;
                    Result = "cafeteria.png";
                    break;
                case "Restaurant(s)":
                    fcid = 2;
                    issh = 1;
                    Result = "restaurant.png";
                    break;
                case "Fitness Center":
                    fcid = 1;
                    issh = 1;
                    Result = "fitnesscenter.png";
                    break;
                case "Gymnasium":
                    fcid = 1;
                    issh = 1;
                    Result = "fitnesscenter.png";
                    break;
                case "Gym":
                    fcid = 1;
                    issh = 1;
                    Result = "fitnesscenter.png";
                    break;
                case "Air conditioning":
                    fcid = 19;
                    issh = 1;
                    Result = "airconditioning.png";
                    break;
                case "Ac Room":
                    fcid = 19;
                    issh = 1;
                    Result = "airconditioning.png";
                    break;
                case "Air Conditioner":
                    fcid = 19;
                    issh = 1;
                    Result = "airconditioning.png";
                    break;
                case "Bell boy":
                    fcid = 21;
                    issh = 1;
                    Result = "bellboy.png";
                    break;
                case "Bar":
                    fcid = 3;
                    issh = 1;
                    Result = "bar.png";
                    break;
                case "Bar/Lounge":
                    fcid = 3;
                    issh = 1;
                    Result = "bar.png";
                    break;
                case "Bars":
                    fcid = 3;
                    issh = 1;
                    Result = "bar.png";
                    break;
                case "Lounge":
                    fcid = 3;
                    issh = 1;
                    Result = "bar.png";
                    break;
                case "Business Center":
                    fcid = 22;
                    issh = 1;
                    Result = "businesscenter.png";
                    break;
                case "Meeting room":
                    fcid = 22;
                    issh = 1;
                    Result = "businesscenter.png";
                    break;
                case "Conference Hall(s)":
                    fcid = 22;
                    issh = 1;
                    Result = "businesscenter.png";
                    break;
                case "Conference room":
                    fcid = 22;
                    issh = 1;
                    Result = "businesscenter.png";
                    break;
                case "Currency exchange":
                    fcid = 23;
                    issh = 1;
                    Result = "currencyexchange.png";
                    break;
                case "Tourist center":
                    fcid = 24;
                    issh = 1;
                    Result = FacilityImage;
                    break;
                case "Travel services":
                    fcid = 24;
                    issh = 1;
                    Result = FacilityImage;
                    break;
                default:
                    Result = FacilityImage;
                    break;
            }
            return Result;
        }
    }

    public class EHotelRoomDetail
    {
        public EHotelRoomDetail()
        {
            HotelID = string.Empty;
            MealOrderNo = 999;
            this.IsRoomUpgrade = false;
            this.OfferTypeIds = string.Empty;
        }
        public EHotelRoomDetail(EHotelRoomDetail Obj)
        {
            this.RoomBasicClassType = Obj.RoomBasicClassType;
            this.RoomIndex = Obj.RoomIndex;
            this.RoomNo = Obj.RoomNo;
            this.HotelID = Obj.HotelID;
            this.RoomType = Obj.RoomType;
            this.RoomTypeId = Obj.RoomTypeId;
            this.RoomMeals = Obj.RoomMeals;
            this.RoomMealsID = Obj.RoomMealsID;
            this.RoomOccupancyDetails = Obj.RoomOccupancyDetails;
            this.IsOffer = Obj.IsOffer;
            this.OfferName = Obj.OfferName;
            this.TotalRate = Obj.TotalRate;
            this.AllNightTotalRate = Obj.AllNightTotalRate;
            this.AllNigntOfferRate = Obj.AllNigntOfferRate;
            this.Currency = Obj.Currency;
            this.Supplier = Obj.Supplier;
            this.IsSupplier = Obj.IsSupplier;
            this.SupplierName = Obj.SupplierName;
            this.IsCanclePolicy = Obj.IsCanclePolicy;
            this.CanclePolicy = Obj.CanclePolicy;
            this.IsFreeCostCancellationDate = Obj.IsFreeCostCancellationDate;
            this.FreeCostCancellationDate = Obj.FreeCostCancellationDate;
            this.IsErrata = Obj.IsErrata;
            this.Errata = Obj.Errata;
            this.ExtraInfo = Obj.ExtraInfo;
            this.AvilableRoom = Obj.AvilableRoom;
            this.Issharedbedding = Obj.Issharedbedding;
            this.IsExtraBed = Obj.IsExtraBed;
            this.IsMealPlanUpgrad = Obj.IsMealPlanUpgrad;
            this.ViewCalculation = Obj.ViewCalculation;
            this.HotelRoomAminities = Obj.HotelRoomAminities;
            this.HotelRoomImages = Obj.HotelRoomImages;
            this.SupplierRoomDefaultInfo = Obj.SupplierRoomDefaultInfo;
            this.RoomDefaultInfo = Obj.RoomDefaultInfo;
            this.EMealPlanUpgradeInfoObj = Obj.EMealPlanUpgradeInfoObj;
            this.GMXMealPlanUpgradeInfo = Obj.GMXMealPlanUpgradeInfo;
            this.RawHotelRoomSHA = Obj.RawHotelRoomSHA;
            this.RoomSHA = Obj.RoomSHA;
            this.ERoomInfo = Obj.ERoomInfo;
            this.CancellationPolicyRes = Obj.CancellationPolicyRes;
            this.OrderNo = Obj.OrderNo;
            this.MealOrderNo = Obj.MealOrderNo;
            this.RTMDescription = Obj.RTMDescription;
            this.RTMRoomSize = Obj.RTMRoomSize;
            this.PromoClassLst = Obj.PromoClassLst;

            this.RAWRateClsLst = Obj.RAWRateClsLst;
            this.OfferCount = Obj.OfferCount;
            this.TotalTax = Obj.TotalTax;
            this.IsTaxType = Obj.IsTaxType;
            this.OfferInclusion = Obj.OfferInclusion;

            this.ERoomTypeUpgradeInfoObj = Obj.ERoomTypeUpgradeInfoObj;
            this.ERoomTypeUpgradeInfo = Obj.ERoomTypeUpgradeInfo;
            this.IsRoomUpgrade = Obj.IsRoomUpgrade;

            this.EHotelSupplimentInfo_ = Obj.EHotelSupplimentInfo_;
            this.HotelSupplimentInfoLst = Obj.HotelSupplimentInfoLst;
            this.IsSupplimentInfo = Obj.IsSupplimentInfo;

            this.BHRDID = Obj.BHRDID;

            this.ServiceAPIID = Obj.ServiceAPIID;

            this.IsHotelPackage = Obj.IsHotelPackage;
            this.PackageId = Obj.PackageId;
            this.PackageName = Obj.PackageName;

            this.IsMultiRoom = Obj.IsMultiRoom;
            this.MultiRooms = Obj.MultiRooms;

            this.CheckInDate = Obj.CheckInDate;
            this.CheckOutDate = Obj.CheckOutDate;

            this.AdultrateDateWiseRate = Obj.AdultrateDateWiseRate;
            this.ChildrateDateWiseRate = Obj.ChildrateDateWiseRate;
            this.OfferTypeIds = Obj.OfferTypeIds;
            this.MealOrderNo = Obj.MealOrderNo;
            this.IsRoomUpgrade = Obj.IsRoomUpgrade;
            this.IsHavePromoRate = Obj.IsHavePromoRate;

            this.IsHavePromoRate = Obj.IsHavePromoRate;
            this.otherserviceoffercost = Obj.otherserviceoffercost;
            this.IsTransferOfferAvailable = Obj.IsTransferOfferAvailable;
            this.OtherServiceOffer = Obj.OtherServiceOffer;
            this.otherserviceoffercost = Obj.otherserviceoffercost;
            this.Inclusions = Obj.Inclusions;
            this.Exclusions = Obj.Exclusions;
            this.TermsCondition = Obj.TermsCondition;
            this.MandantoryOS = Obj.MandantoryOS;
            this.MandantoryOScost = Obj.MandantoryOScost;
            this.MandantoryOSDesc = Obj.MandantoryOSDesc;
            this.MandantorySC = Obj.MandantorySC;
            this.MandantorySCcost = Obj.MandantorySCcost;
            this.MandantorySCDesc = Obj.MandantorySCDesc;

            this.RateCalculation = Obj.RateCalculation;
            this.RunNo = Obj.RunNo;
            this.AllocationDetails = Obj.AllocationDetails;
        }
        public EHotelRoomDetail(List<EHotelRoomDetail> EHotelRoomDetailLst)
        {
            try
            {
                var DefaultData = EHotelRoomDetailLst.FirstOrDefault();
                this.OtherServiceOffer = DefaultData.OtherServiceOffer;
                this.otherserviceoffercost = DefaultData.otherserviceoffercost;

                this.MandantoryOS = DefaultData.MandantoryOS;
                this.MandantoryOScost = DefaultData.MandantoryOScost;
                this.MandantoryOSDesc = DefaultData.MandantoryOSDesc;

                this.MandantorySC = DefaultData.MandantorySC;
                this.MandantorySCcost = DefaultData.MandantorySCcost;
                this.MandantorySCDesc = DefaultData.MandantorySCDesc;

                this.RoomBasicClassType = string.Join(",", EHotelRoomDetailLst.Select(f => f.RoomBasicClassType));
                this.RoomIndex = DefaultData.RoomIndex;
                this.RoomNo = DefaultData.RoomNo;
                this.HotelID = DefaultData.HotelID;
                this.RoomType = string.Join(" , ", EHotelRoomDetailLst.Select(f => f.RoomType));
                this.RoomTypeId = string.Join("|", EHotelRoomDetailLst.Select(f => f.RoomTypeId));
                this.RoomMeals = string.Join("|", EHotelRoomDetailLst.Select(f => f.RoomMeals).Distinct());
                this.RoomMealsID = string.Join("|", EHotelRoomDetailLst.Select(f => f.RoomMealsID));

                var RawOccupancyDetails = new List<ERoomOccupancyInfo>();
                foreach (var d in DefaultData.RoomOccupancyDetails)
                {
                    d.RoomType = this.RoomType;
                    RawOccupancyDetails.Add(d);
                }
                this.RoomOccupancyDetails = RawOccupancyDetails;
                this.IsOffer = EHotelRoomDetailLst.Where(d => d.IsOffer.Equals(true)).ToList().Count != 0;
                var OfferData = EHotelRoomDetailLst.Where(d => d.IsOffer.Equals(true)).Select(f => string.Format("{0}({1})", f.OfferName, f.RoomType)).Distinct().ToList();
                this.OfferName = string.Join(" <br/> ", OfferData);
                //this.TotalRate = DefaultData.IsHotelPackage.Equals(1) ? DefaultData.TotalRate : EHotelRoomDetailLst.Sum(v => v.TotalRate);
                this.TotalRate = EHotelRoomDetailLst.Sum(v => v.TotalRate);
                this.TermsCondition = DefaultData.TermsCondition;
                this.Inclusions = DefaultData.Inclusions;
                this.Exclusions = DefaultData.Exclusions;
                //this.AllNightTotalRate = DefaultData.IsHotelPackage.Equals(1) ? DefaultData.AllNightTotalRate : EHotelRoomDetailLst.Sum(v => v.AllNightTotalRate);
                //this.AllNigntOfferRate = DefaultData.IsHotelPackage.Equals(1) ? DefaultData.AllNigntOfferRate : EHotelRoomDetailLst.Sum(v => v.AllNigntOfferRate);
                this.AllNightTotalRate = EHotelRoomDetailLst.Sum(v => v.AllNightTotalRate);
                this.AllNigntOfferRate = EHotelRoomDetailLst.Sum(v => v.AllNigntOfferRate);
                this.Currency = DefaultData.Currency;
                this.Supplier = string.Join("|", EHotelRoomDetailLst.Select(f => f.Supplier).Distinct());
                this.IsSupplier = EHotelRoomDetailLst.Where(d => d.IsSupplier.Equals(true)).ToList().Count != 0;
                this.SupplierName = string.Join("|", EHotelRoomDetailLst.Select(f => f.SupplierName).Distinct());
                this.IsCanclePolicy = EHotelRoomDetailLst.Where(d => d.IsCanclePolicy.Equals(true)).ToList().Count != 0;
                this.CanclePolicy = string.Join("<br/>", EHotelRoomDetailLst.Select(f => f.CanclePolicy));
                this.IsFreeCostCancellationDate = EHotelRoomDetailLst.Where(d => d.IsFreeCostCancellationDate.Equals(true)).ToList().Count != 0;
                this.FreeCostCancellationDate = string.Join("|", EHotelRoomDetailLst.Select(f => f.FreeCostCancellationDate));
                this.IsErrata = EHotelRoomDetailLst.Where(d => d.IsErrata.Equals(true)).ToList().Count != 0;
                this.Errata = string.Join("<br/>", EHotelRoomDetailLst.Select(f => f.Errata).Distinct());
                this.ExtraInfo = string.Join("|", EHotelRoomDetailLst.Select(f => f.ExtraInfo));
                this.AvilableRoom = EHotelRoomDetailLst.Sum(v => v.AvilableRoom);
                this.Issharedbedding = EHotelRoomDetailLst.Where(d => d.Issharedbedding.Equals(true)).ToList().Count != 0;
                this.IsExtraBed = EHotelRoomDetailLst.Where(d => d.IsExtraBed.Equals(true)).ToList().Count != 0;
                this.IsMealPlanUpgrad = EHotelRoomDetailLst.Where(d => d.IsMealPlanUpgrad.Equals(true)).ToList().Count != 0;
                this.ViewCalculation = new EViewCalculation(EHotelRoomDetailLst.Select(f => f.ViewCalculation).ToList());
                this.HotelRoomAminities = DefaultData.HotelRoomAminities;
                this.HotelRoomImages = DefaultData.HotelRoomImages;
                this.SupplierRoomDefaultInfo = new ESupplierRoomDefaultInfo(EHotelRoomDetailLst.Select(f => f.SupplierRoomDefaultInfo).ToList());
                this.RoomDefaultInfo = new ERoomDefaultInfo(EHotelRoomDetailLst.Select(f => f.RoomDefaultInfo).ToList());
                this.EMealPlanUpgradeInfoObj = new EMealPlanUpgradeInfo(EHotelRoomDetailLst.Select(f => f.EMealPlanUpgradeInfoObj).ToList());
                this.GMXMealPlanUpgradeInfo = DefaultData.GMXMealPlanUpgradeInfo;
                this.RawHotelRoomSHA = DefaultData.RawHotelRoomSHA;
                this.RoomSHA = DefaultData.RoomSHA;
                this.ERoomInfo = DefaultData.ERoomInfo;
                this.CancellationPolicyRes = DefaultData.CancellationPolicyRes;
                this.OrderNo = DefaultData.OrderNo;
                this.MealOrderNo = DefaultData.MealOrderNo;
                this.RTMDescription = DefaultData.RTMDescription;
                this.RTMRoomSize = DefaultData.RTMRoomSize;
                this.PromoClassLst = DefaultData.PromoClassLst;

                this.RAWRateClsLst = DefaultData.RAWRateClsLst;
                this.OfferCount = OfferData.Count;
                this.TotalTax = EHotelRoomDetailLst.Sum(v => v.TotalTax);
                this.IsTaxType = EHotelRoomDetailLst.Where(d => d.IsTaxType.Equals(true)).ToList().Count != 0;
                this.OfferInclusion = string.Join("<br/>", EHotelRoomDetailLst.Select(f => f.OfferInclusion));
                this.ERoomTypeUpgradeInfoObj = new ERoomTypeUpgradeClass(EHotelRoomDetailLst.Select(f => f.ERoomTypeUpgradeInfoObj).ToList());
                this.ERoomTypeUpgradeInfo = DefaultData.ERoomTypeUpgradeInfo;
                this.IsRoomUpgrade = EHotelRoomDetailLst.Where(d => d.IsRoomUpgrade.Equals(true)).ToList().Count != 0;
                this.EHotelSupplimentInfo_ = new EHotelSupplimentInfo(EHotelRoomDetailLst.Select(f => f.EHotelSupplimentInfo_).ToList());
                this.HotelSupplimentInfoLst = DefaultData.HotelSupplimentInfoLst;
                this.IsSupplimentInfo = EHotelRoomDetailLst.Where(d => d.IsSupplimentInfo.Equals(true)).ToList().Count != 0;
                this.BHRDID = string.Join("|", EHotelRoomDetailLst.Select(f => f.BHRDID));
                this.ServiceAPIID = DefaultData.ServiceAPIID;
                this.IsHotelPackage = DefaultData.IsHotelPackage;
                this.PackageId = DefaultData.PackageId;
                this.PackageName = DefaultData.PackageName;
                this.IsMultiRoom = EHotelRoomDetailLst.Count > 1 ? 1 : 0;
                this.MultiRooms = EHotelRoomDetailLst;
                var dicAdultrateDateWiseRate = new Dictionary<string, Dictionary<DateTime, double>>();
                var dicChildrateDateWiseRate = new Dictionary<string, Dictionary<DateTime, double>>();
                foreach (var ff in EHotelRoomDetailLst)
                {
                    foreach (var t1 in ff.AdultrateDateWiseRate)
                    {
                        dicAdultrateDateWiseRate.Add(t1.Key, t1.Value);
                    }
                    foreach (var t2 in ff.ChildrateDateWiseRate)
                    {
                        dicChildrateDateWiseRate.Add(t2.Key, t2.Value);
                    }
                }
                this.AdultrateDateWiseRate = dicAdultrateDateWiseRate;
                this.ChildrateDateWiseRate = dicChildrateDateWiseRate;
                this.OfferTypeIds = string.Join("|", EHotelRoomDetailLst.Where(e => !string.IsNullOrEmpty(e.OfferTypeIds)).Select(f => f.OfferTypeIds.Split(',')).SelectMany(g => g).Distinct());
                this.IsHavePromoRate = EHotelRoomDetailLst.Where(n => n.IsHavePromoRate).ToArray().Length > 0;
                this.otherserviceoffercost = DefaultData.otherserviceoffercost;
                this.IsTransferOfferAvailable = DefaultData.IsTransferOfferAvailable;
                this.OtherServiceOffer = DefaultData.OtherServiceOffer;
                this.otherserviceoffercost = DefaultData.otherserviceoffercost;
                this.Inclusions = DefaultData.Inclusions;
                this.Exclusions = DefaultData.Exclusions;
                this.TermsCondition = DefaultData.TermsCondition;
                this.MandantoryOS = DefaultData.MandantoryOS;
                this.MandantoryOScost = DefaultData.MandantoryOScost;
                this.MandantoryOSDesc = DefaultData.MandantoryOSDesc;
                this.MandantorySC = DefaultData.MandantorySC;
                this.MandantorySCcost = DefaultData.MandantorySCcost;
                this.MandantorySCDesc = DefaultData.MandantorySCDesc;

                this.RunNo = DefaultData.RunNo;
                this.AllocationDetails = DefaultData.AllocationDetails;
                this.RateCalculation = MergeRateCalculation(EHotelRoomDetailLst);
            }
            catch (Exception ex)
            {

            }
        }
        public Dictionary<string, object> MergeRateCalculation(List<EHotelRoomDetail> EHotelRoomDetailLst)
        {
            var RateCalculationDic = new Dictionary<string, object>();
            try
            {
                decimal Base_Room_Rate = 0;
                decimal Service_Charges = 0;
                decimal Extra_Fees = 0;
                decimal Extra_Fees_Value = 0;
                decimal Input_Tax = 0;
                decimal Output_Tax = 0;
                decimal OfferDiscount = 0;
                string OfferName = string.Empty;
                string Input_Tax_Value = string.Empty;
                string Input_Tax_CalType = string.Empty;
                string Payable_To_Supplier = string.Empty;

                foreach (var ff in EHotelRoomDetailLst)
                {
                    foreach (var t1 in ff.RateCalculation)
                    {
                        switch (t1.Key)
                        {
                            case ContractedHotelConstant.Base_Room_Rate:
                                Base_Room_Rate += Convert.ToDecimal(t1.Value);
                                break;
                            case ContractedHotelConstant.Service_Charges:
                                Service_Charges += Convert.ToDecimal(t1.Value);
                                break;
                            case ContractedHotelConstant.Extra_Fees:
                                Extra_Fees += Convert.ToDecimal(t1.Value);
                                break;
                            case ContractedHotelConstant.Extra_Fees_Value:
                                Extra_Fees_Value = Convert.ToDecimal(t1.Value);
                                break;
                            case ContractedHotelConstant.Input_Tax:
                                Input_Tax += Convert.ToDecimal(t1.Value);
                                break;
                            case ContractedHotelConstant.Output_Tax:
                                Output_Tax += Convert.ToDecimal(t1.Value);
                                break;
                            case ContractedHotelConstant.Base_Room_Rate_OfferDiscount:
                                OfferDiscount += Convert.ToDecimal(t1.Value);
                                break;
                            case ContractedHotelConstant.Base_Room_Rate_OfferName:
                                OfferName = Convert.ToString(t1.Value);
                                break;
                        }
                    }
                }

                RateCalculationDic.Add(ContractedHotelConstant.Base_Room_Rate, Base_Room_Rate);
                RateCalculationDic.Add(ContractedHotelConstant.Service_Charges, Service_Charges);
                RateCalculationDic.Add(ContractedHotelConstant.Service_Charges_Value, "0");
                RateCalculationDic.Add(ContractedHotelConstant.Service_Charges_CalType, "1");
                RateCalculationDic.Add(ContractedHotelConstant.Extra_Fees, Extra_Fees);
                RateCalculationDic.Add(ContractedHotelConstant.Extra_Fees_Value, Extra_Fees_Value);
                RateCalculationDic.Add(ContractedHotelConstant.Extra_Fees_CalType, "1");
                RateCalculationDic.Add(ContractedHotelConstant.Extra_Fees_Desc, "Munipality Fees");
                RateCalculationDic.Add(ContractedHotelConstant.Input_Tax, Input_Tax);
                RateCalculationDic.Add(ContractedHotelConstant.Input_Tax_Value, Input_Tax_Value);
                RateCalculationDic.Add(ContractedHotelConstant.Input_Tax_CalType, Input_Tax_CalType);
                RateCalculationDic.Add(ContractedHotelConstant.Payable_To_Supplier, Payable_To_Supplier);
                RateCalculationDic.Add(ContractedHotelConstant.Output_Tax, Output_Tax);
                RateCalculationDic.Add(ContractedHotelConstant.Base_Room_Rate_OfferDiscount, OfferDiscount);
                RateCalculationDic.Add(ContractedHotelConstant.Base_Room_Rate_OfferName, OfferName);

            }
            catch (Exception ex)
            {

            }
            return RateCalculationDic;
        }
        public EHotelRoomDetail(EHotelRoomDetail oldObj, bool IsPriceChange, EHotelRoomDetail newObj)
        {
            try
            {
                this.RoomBasicClassType = oldObj.RoomBasicClassType;
                this.RoomIndex = oldObj.RoomIndex;
                this.RoomNo = oldObj.RoomNo;
                this.HotelID = oldObj.HotelID;
                this.RoomType = oldObj.RoomType;
                this.RoomTypeId = oldObj.RoomTypeId;
                this.RoomMeals = oldObj.RoomMeals;
                this.RoomMealsID = oldObj.RoomMealsID;
                this.RoomOccupancyDetails = oldObj.RoomOccupancyDetails;
                this.IsOffer = oldObj.IsOffer;
                this.OfferName = oldObj.OfferName;
                this.TotalRate = oldObj.TotalRate;
                this.AllNightTotalRate = oldObj.AllNightTotalRate;
                this.AllNigntOfferRate = oldObj.AllNigntOfferRate;
                this.Currency = oldObj.Currency;
                this.Supplier = oldObj.Supplier;
                this.IsSupplier = oldObj.IsSupplier;
                this.SupplierName = oldObj.SupplierName;
                this.IsCanclePolicy = oldObj.IsCanclePolicy;
                this.CanclePolicy = oldObj.CanclePolicy;
                this.IsFreeCostCancellationDate = oldObj.IsFreeCostCancellationDate;
                this.FreeCostCancellationDate = oldObj.FreeCostCancellationDate;
                this.IsErrata = oldObj.IsErrata;
                this.Errata = oldObj.Errata;
                this.ExtraInfo = oldObj.ExtraInfo;
                this.AvilableRoom = oldObj.AvilableRoom;
                this.Issharedbedding = oldObj.Issharedbedding;
                this.IsExtraBed = oldObj.IsExtraBed;
                this.IsMealPlanUpgrad = oldObj.IsMealPlanUpgrad;
                this.ViewCalculation = oldObj.ViewCalculation;
                this.HotelRoomAminities = oldObj.HotelRoomAminities;
                this.HotelRoomImages = oldObj.HotelRoomImages;
                this.SupplierRoomDefaultInfo = oldObj.SupplierRoomDefaultInfo;
                this.RoomDefaultInfo = oldObj.RoomDefaultInfo;
                this.EMealPlanUpgradeInfoObj = oldObj.EMealPlanUpgradeInfoObj;
                this.GMXMealPlanUpgradeInfo = oldObj.GMXMealPlanUpgradeInfo;
                this.RawHotelRoomSHA = oldObj.RawHotelRoomSHA;
                this.RoomSHA = oldObj.RoomSHA;
                this.ERoomInfo = oldObj.ERoomInfo;
                this.CancellationPolicyRes = oldObj.CancellationPolicyRes;
                this.OrderNo = oldObj.OrderNo;
                this.MealOrderNo = oldObj.MealOrderNo;
                this.RTMDescription = oldObj.RTMDescription;
                this.RTMRoomSize = oldObj.RTMRoomSize;
                this.PromoClassLst = oldObj.PromoClassLst;

                this.RAWRateClsLst = oldObj.RAWRateClsLst;
                this.OfferCount = oldObj.OfferCount;
                this.TotalTax = oldObj.TotalTax;
                this.IsTaxType = oldObj.IsTaxType;
                this.OfferInclusion = oldObj.OfferInclusion;
                this.BHRDID = oldObj.BHRDID;
                this.ServiceAPIID = oldObj.ServiceAPIID;
                if (newObj != null)
                {
                    if (IsPriceChange)
                    {
                        this.TotalRate = newObj.TotalRate;
                        this.AllNightTotalRate = newObj.AllNightTotalRate;
                        this.AllNigntOfferRate = newObj.AllNigntOfferRate;
                        this.IsCanclePolicy = newObj.IsCanclePolicy;
                        this.CanclePolicy = newObj.CanclePolicy;
                        this.IsFreeCostCancellationDate = newObj.IsFreeCostCancellationDate;
                        this.FreeCostCancellationDate = newObj.FreeCostCancellationDate;
                        this.IsErrata = newObj.IsErrata;
                        this.Errata = newObj.Errata;
                        this.ExtraInfo = newObj.ExtraInfo;
                        this.ViewCalculation = newObj.ViewCalculation;
                        this.SupplierRoomDefaultInfo = newObj.SupplierRoomDefaultInfo;
                        this.RoomDefaultInfo = newObj.RoomDefaultInfo;
                        this.RawHotelRoomSHA = newObj.RawHotelRoomSHA;
                        this.RoomSHA = newObj.RoomSHA;
                        this.ERoomInfo = newObj.ERoomInfo;
                        this.CancellationPolicyRes = newObj.CancellationPolicyRes;
                        this.RAWRateClsLst = newObj.RAWRateClsLst;
                    }
                    else
                    {

                        this.ExtraInfo = newObj.ExtraInfo;
                        if (oldObj.SupplierName == "STU")
                        {
                            this.IsCanclePolicy = newObj.IsCanclePolicy;
                            this.CanclePolicy = newObj.CanclePolicy;
                            this.IsFreeCostCancellationDate = newObj.IsFreeCostCancellationDate;
                            this.FreeCostCancellationDate = newObj.FreeCostCancellationDate;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Errorlog.InsertLogError(ex, 1);
            }
        }
        public string RoomBasicClassType { get; set; }
        public int RoomIndex { get; set; }
        public int RoomNo { get; set; }
        public int RoomCount { get; set; }
        public string HotelID { get; set; }
        public string RoomType { get; set; }
        public string RoomTypeId { get; set; }
        public string RoomMeals { get; set; }
        public string RoomMealsName { get; set; }
        public string RoomMealsID { get; set; }
        public int Room_Max_adult { get; set; }
        public int Room_Max_child { get; set; }
        public List<ERoomOccupancyInfo> RoomOccupancyDetails { get; set; }
        public List<string>? ImportantNotes { get; set; }
        public bool IsOffer { get; set; }
        public string OfferName { get; set; }
        public decimal TotalRate { get; set; }
        public decimal AllNightTotalRate { get; set; }
        public decimal AllNigntOfferRate { get; set; }
        public string Currency { get; set; }
        public string Supplier { get; set; }
        public bool IsSupplier { get; set; }
        public string SupplierName { get; set; }
        public bool IsCanclePolicy { get; set; }
        public string CanclePolicy { get; set; }
        public bool IsFreeCostCancellationDate { get; set; }
        public string FreeCostCancellationDate { get; set; }
        public bool IsErrata { get; set; }
        public string Errata { get; set; }
        public string ExtraInfo { get; set; }
        public int AvilableRoom { get; set; }
        public bool Issharedbedding { get; set; }
        public bool IsExtraBed { get; set; }
        public bool IsMealPlanUpgrad { get; set; }
        public EViewCalculation ViewCalculation { get; set; }
        public List<EHotelRoomAminities> HotelRoomAminities { get; set; }
        public List<EHotelRoomImages> HotelRoomImages { get; set; }
        public ESupplierRoomDefaultInfo SupplierRoomDefaultInfo { get; set; }
        public ERoomDefaultInfo RoomDefaultInfo { get; set; }
        public EMealPlanUpgradeInfo EMealPlanUpgradeInfoObj { get; set; }
        public GMXMealPlanUpgradeInfo GMXMealPlanUpgradeInfo { get; set; }
        public RawHotelRoomSHA RawHotelRoomSHA { get; set; }
        public string RoomSHA { get; set; }
        public ERoomInfo ERoomInfo { get; set; }
        public MainCancellationRawInfo CancellationPolicyRes { get; set; }
        public int OrderNo { get; set; }
        public int MealOrderNo { get; set; }
        public string RTMDescription { get; set; }
        public string RTMRoomSize { get; set; }
        public List<PromoClass> PromoClassLst { get; set; }
        public List<RAWRateCls> RAWRateClsLst { get; set; }
        public int OfferCount { get; set; }
        public decimal TotalTax { get; set; }
        public bool IsTaxType { get; set; }
        public string OfferInclusion { get; set; }
        public ERoomTypeUpgradeClass ERoomTypeUpgradeInfoObj { get; set; }
        public RoomTypeUpgradeClass ERoomTypeUpgradeInfo { get; set; }
        public bool IsRoomUpgrade { get; set; }
        public EHotelSupplimentInfo EHotelSupplimentInfo_ { get; set; }
        public List<HotelSupplimentInfo> HotelSupplimentInfoLst { get; set; }
        public bool IsSupplimentInfo { get; set; }
        public string BHRDID { get; set; }
        public int ServiceAPIID { get; set; }
        public int IsHotelPackage { get; set; }
        public int PackageId { get; set; }
        public string PackageName { get; set; }
        public string PackageCode { get; set; }
        public string SeasonId { get; set; }
        public int IsPromo { get; set; }
        public int IsMultiRoom { get; set; }
        public List<EHotelRoomDetail> MultiRooms { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public Dictionary<string, Dictionary<DateTime, double>> AdultrateDateWiseRate { get; set; }
        public Dictionary<string, Dictionary<DateTime, double>> ChildrateDateWiseRate { get; set; }
        public string OfferTypeIds { get; set; }
        public bool IsHavePromoRate { get; set; }
        public bool IsTransferOfferAvailable { get; set; }
        public DataTable OtherServiceOffer { get; set; }
        public Dictionary<string, OtherServiceOffer> otherserviceoffercost { get; set; }
        public List<string> Inclusions { get; set; }
        public List<string> Exclusions { get; set; }
        public string TermsCondition { get; set; }
        public List<string> MandantoryOS { get; set; }
        public decimal MandantoryOScost { get; set; }
        public List<string> MandantoryOSDesc { get; set; }
        public List<string> MandantorySC { get; set; }
        public decimal MandantorySCcost { get; set; }
        public List<string> MandantorySCDesc { get; set; }
        public List<string> MandantoryOSServiceType { get; set; }
        public string MandantoryOSServiceTypeDesc { get; set; }
        public string MandantoryOSServiceInclusions { get; set; }
        public bool IsExtraServiceMandantory { get; set; }
        public List<string> NonMandantoryOS { get; set; }
        public decimal NonMandantoryOScost { get; set; }
        public List<string> NonMandantoryOSDesc { get; set; }
        public List<string> NonMandantoryOSServiceType { get; set; }
        public string NonMandantoryOSServiceTypeDesc { get; set; }
        public string NonMandantoryOSServiceInclusions { get; set; }
        public List<string> NonMandantorySC { get; set; }
        public decimal NonMandantorySCcost { get; set; }
        public List<string> NonMandantorySCDesc { get; set; }
        public bool IsExtraServiceNonMandantory { get; set; }
        public Dictionary<string, object> RateCalculation { get; set; }
        public decimal GreenTaxDiscount { get; set; }
        public decimal GreenTax { get; set; }

        //addded by rohit for DOTW
        public string RunNo { get; set; }
        public string AllocationDetails { get; set; }//addded by rohit for DOTW
        public string OthereServiceRate { get; set; }
        //for earlycheckinlatecheckout
        public EarlycheckinLateCheckout EarlycheckinLateCheckout { get; set; }
        public object selectedEarlycheckinLateCheckout { get; set; }

        //for MealInclusionExclusion
        public MealInclusionExclusion MealInclusionExclusion { get; set; }
        public object selectedMealInclusionExclusion { get; set; }
    }

    public class OtherServiceOffer
    {
        public OtherServiceOffer()
        {
            this.OfferCost = 0;
            this.OtherServiceCost = 0;
            this.OfferIds = new List<string>();
            this.OtherServiceIds = new List<string>();
        }
        public List<string> OfferIds { get; set; }
        public List<string> OtherServiceIds { get; set; }
        public decimal OfferCost { get; set; }
        public decimal OtherServiceCost { get; set; }
    }

    public class PromoClass
    {
        public PromoClass()
        {

        }
        public int IsPromo { get; set; }
        public string PromoName { get; set; }
        public string PromoDes { get; set; }
        public string PromoCode { get; set; }
        public int IsPromoPerOrVal { get; set; }
        public decimal PromoDiscount { get; set; }
        public int IsPerNightOrOverall { get; set; }
        public string FreeInclusion { get; set; }
    }

    public class MainCancellationRawInfo
    {
        public int APIID { get; set; }
        public ContractHotelCancellationRawInfo ContractHotelCancellationRawInfoObj { get; set; }
    }

    public class ContractHotelCancellationRawInfo
    {
        public string? Cancellation_Dt { get; set; }
        public int HotelMarketId { get; set; }
        public int HotelId { get; set; }
        public int RoomTypeId { get; set; }
        public string? StartDateStr { get; set; }
        public int StartDate { get; set; }
        public int EndDate { get; set; }
        public decimal BookingCost { get; set; }
        public string? Currency { get; set; }
        public string? NightWiseRates { get; set; }
        public string? HotelSeasonID { get; set; }
    }

    public class HotelRoomBestRate
    {
        public HotelRoomBestRate()
        {

        }
        public List<string> MealPlanIDs { get; set; }
        public List<string> RoomTypes { get; set; }
        public decimal r { get; set; }
        public decimal or { get; set; }
        public decimal t { get; set; }
        public decimal tm { get; set; }
        public bool ist { get; set; }
        public decimal tr { get; set; }
        public bool isoffer { get; set; }
        public int ishp { get; set; }
        public decimal e { get; set; } //extra service
    }

    public class EMealPlanUpgradeInfo
    {
        public EMealPlanUpgradeInfo()
        {

        }

        public EMealPlanUpgradeInfo(List<EMealPlanUpgradeInfo> objlst)
        {
            try
            {
                var group = from item in objlst.SelectMany(f => f.UpgradeMealInfo).Select(b => b)
                            group item by new { MealPlanID = item.MealPlanID }
                                into hjh
                            select new GMXMealPlanUpgradeInfo(hjh.ToList());
                if (group != null && group.Count() != 0)
                {
                    this.UpgradeMealInfo = group.ToList();
                }
            }
            catch (Exception ex)
            {

            }
        }
        public List<GMXMealPlanUpgradeInfo> UpgradeMealInfo { get; set; }
    }

    public class GMXMealPlanUpgradeInfo
    {
        public GMXMealPlanUpgradeInfo()
        {

        }

        public GMXMealPlanUpgradeInfo(List<GMXMealPlanUpgradeInfo> objlst)
        {
            var defaultobj = objlst.FirstOrDefault();
            this.Index = defaultobj.Index;
            this.MealPlanDesc = defaultobj.MealPlanDesc;
            this.MealPlanID = defaultobj.MealPlanID;
            this.ARMealPlanCost = objlst.Sum(b => b.ARMealPlanCost);
            this.ARCurrency = defaultobj.ARCurrency;
            this.ARROE = defaultobj.ARROE;
            this.APCurrency = defaultobj.APCurrency;
            this.APROE = defaultobj.APROE;
            this.APMealPlanCost = objlst.Sum(b => b.APMealPlanCost);
            this.SysProfitWithTax = objlst.Sum(b => b.SysProfitWithTax);
            this.SysProfit = objlst.Sum(b => b.SysProfit);
            this.SysTax = objlst.Sum(b => b.SysTax);
            this.InputTax = objlst.Sum(b => b.InputTax);
            this.MarkUP = objlst.Sum(b => b.MarkUP);
            this.OutputTax = objlst.Sum(b => b.OutputTax);
        }
        public int Index { get; set; }
        public string MealPlanDesc { get; set; }
        public int MealPlanID { get; set; }
        public decimal ARMealPlanCost { get; set; }
        public string ARCurrency { get; set; }
        public decimal ARROE { get; set; }
        public string APCurrency { get; set; }
        public decimal APROE { get; set; }
        public decimal APMealPlanCost { get; set; }
        public decimal SysProfitWithTax { get; set; }
        public decimal SysProfit { get; set; }
        public decimal SysTax { get; set; }

        public decimal InputTax { get; set; }
        public decimal MarkUP { get; set; }
        public decimal OutputTax { get; set; }


        public decimal discount { get; set; }


        public string MealPlanInclusion { get; set; }
        public string MealPlanExclusion { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
    }

    public class ERoomInfo
    {
        public int THDTraceID { get; set; }
        public int RoomIndex { get; set; }
    }

    public class ERoomDefaultInfo
    {
        public ERoomDefaultInfo()
        {

            this.AdultSurcharge = 0;
            this.ChildSurcharge = 0;
            this.SurchargeID = "0";
            this.SurchargeDesc = "0";
            this.IsCompulsory = "0";
        }
        public ERoomDefaultInfo(List<ERoomDefaultInfo> objlst)
        {
            var defaultobj = objlst.FirstOrDefault();
            this.Supplier_Currency = defaultobj.Supplier_Currency;
            this.Supplier_AllNightRate = objlst.Sum(g => g.Supplier_AllNightRate);
            this.Supplier_AllNightOfferRate = objlst.Sum(g => g.Supplier_AllNightOfferRate);
            this.System_CurrencyID = defaultobj.System_CurrencyID;
            this.System_CurrencyCode = defaultobj.System_CurrencyCode;
            this.System_CurrencyROE_MLP = objlst.Sum(g => g.System_CurrencyROE_MLP);
            this.System_CurrencyROE_DIV = objlst.Sum(g => g.System_CurrencyROE_DIV);
            this.System_OneNightRate = objlst.Sum(g => g.System_OneNightRate);
            this.System_AllNightRate = objlst.Sum(g => g.System_AllNightRate);
            this.System_AllNightOfferRate = objlst.Sum(g => g.System_AllNightOfferRate);
            this.EHotelMarkupInfoLst = objlst.SelectMany(g => g.EHotelMarkupInfoLst).Select(m => m).ToList();
            this.Sell_WithMKAllNightRate = objlst.Sum(g => g.Sell_WithMKAllNightRate);
            this.Sell_WithMKAllNightOfferRate = objlst.Sum(g => g.Sell_WithMKAllNightOfferRate);
            this.System_TotalMarkup = objlst.Sum(g => g.System_TotalMarkup);
            this.System_TotalTax = objlst.Sum(g => g.System_TotalTax);
            this.Purchase_CurrencyID = defaultobj.Purchase_CurrencyID;
            this.Purchase_CurrencyCode = defaultobj.Purchase_CurrencyCode;
            this.Purchase_CurrencyROE_MLP = objlst.Sum(g => g.Purchase_CurrencyROE_MLP);
            this.Purchase_CurrencyROE_DIV = objlst.Sum(g => g.Purchase_CurrencyROE_DIV);
            this.Purchase_FinalAllNightRate = objlst.Sum(g => g.Purchase_FinalAllNightRate);
            this.Purchase_ServiceInfo = objlst.Where(n => n.Purchase_ServiceInfo != null).SelectMany(g => g.Purchase_ServiceInfo).Select(m => m).ToList();
            this.AdultSurcharge = objlst.Sum(g => g.AdultSurcharge);
            this.ChildSurcharge = objlst.Sum(g => g.ChildSurcharge);
            this.SurchargeID = defaultobj.SurchargeID;
            this.SurchargeDesc = defaultobj.SurchargeDesc;
            this.IsCompulsory = defaultobj.IsCompulsory;
            this.DicAllNightRate = defaultobj.DicAllNightRate;
            this.DicAllOfferTotalRate = defaultobj.DicAllOfferTotalRate;
        }

        public string Supplier_Currency { get; set; }
        public decimal Supplier_AllNightRate { get; set; }
        public decimal Supplier_AllNightOfferRate { get; set; }
        public int System_CurrencyID { get; set; }
        public string System_CurrencyCode { get; set; }
        public decimal System_CurrencyROE_MLP { get; set; }
        public decimal System_CurrencyROE_DIV { get; set; }
        public decimal System_OneNightRate { get; set; }
        public decimal System_AllNightRate { get; set; }
        public decimal System_AllNightOfferRate { get; set; }
        public List<EHotelMarkupInfo> EHotelMarkupInfoLst { get; set; }
        public decimal Sell_WithMKAllNightRate { get; set; }
        public decimal Sell_WithMKAllNightOfferRate { get; set; }
        public decimal System_TotalMarkup { get; set; }
        public decimal System_TotalTax { get; set; }
        public int Purchase_CurrencyID { get; set; }
        public string Purchase_CurrencyCode { get; set; }
        public decimal Purchase_CurrencyROE_MLP { get; set; }
        public decimal Purchase_CurrencyROE_DIV { get; set; }
        public decimal Purchase_FinalAllNightRate { get; set; }
        public List<Purchase_ServiceInfo> Purchase_ServiceInfo { get; set; }

        public decimal AdultSurcharge { get; set; }
        public decimal ChildSurcharge { get; set; }
        public string SurchargeID { get; set; }
        public string SurchargeDesc { get; set; }
        public string IsCompulsory { get; set; }
        public Dictionary<string, object> DicAllNightRate { get; set; }
        public Dictionary<string, object> DicAllOfferTotalRate { get; set; }
    }

    public class Purchase_ServiceInfo
    {
        public int OrderNo { get; set; }
        public string? ServiceName { get; set; }
        public string? Purchase_FinalRate { get; set; }
    }

    public class EHotelMarkupInfo
    {
        public EHotelMarkupInfo()
        {
            OrderNo = 0;
        }
        public int OrderNo { get; set; }
        public decimal BaseValue { get; set; }
        public bool MarkupType { get; set; }
        public decimal MarkupValue { get; set; }
        public bool IsTaxOnMarkup { get; set; }
        public int MarkupTaxID { get; set; }
        public decimal MarkupTax { get; set; }
        public decimal MarkUpOnBaseValue { get; set; }
        public decimal TaxOnBaseValue { get; set; }
        public decimal BaseValueWithMarkUp { get; set; }
        public decimal Total_BaseValue_MarkUp_Tax { get; set; }
    }
    public class EHotelRoomAminities
    {
        public EHotelRoomAminities()
        {

        }
        public string AminitieName { get; set; }
    }
    public class EHotelRoomImages : etosImageGallary
    {
        public EHotelRoomImages()
        {
        }

        public EHotelRoomImages(EHotelImageGallary EHotelImageGallary_)
        {
            this.ImageActualPath = EHotelImageGallary_.ImageActualPath;
            this.ImageName = EHotelImageGallary_.ImageName;
            this.ImageThumPath = EHotelImageGallary_.ImageThumPath;
        }
        public string RoomTypeID { get; set; }
        public string RoomTypeName { get; set; }
    }

    public class ESupplierRoomDefaultInfo
    {
        public ESupplierRoomDefaultInfo()
        {
            this.ComissionVal = string.Empty;
            this.IsComissionPerOrVal = string.Empty;
            this.Commissions = 0;
        }
        public ESupplierRoomDefaultInfo(List<ESupplierRoomDefaultInfo> ESupplierRoomDefaultInfoLst)
        {
            var Default = ESupplierRoomDefaultInfoLst.FirstOrDefault();
            this.Supplier_ID = Default.Supplier_ID;
            this.Supplier_Name = Default.Supplier_Name;
            this.SupplierCode = Default.SupplierCode;
            this.Supplier_Currency = Default.Supplier_Currency;
            this.Supplier_MinNightRate = ESupplierRoomDefaultInfoLst.Sum(f => f.Supplier_MinNightRate);
            this.Supplier_AllNightRate = ESupplierRoomDefaultInfoLst.Sum(f => f.Supplier_AllNightRate);
            this.Supplier_AllNightOfferRate = ESupplierRoomDefaultInfoLst.Sum(f => f.Supplier_AllNightOfferRate);
            this.ComissionVal = Default.ComissionVal;
            this.IsComissionPerOrVal = Default.IsComissionPerOrVal;
            this.Commissions = ESupplierRoomDefaultInfoLst.Sum(f => f.Commissions);
        }
        public int Supplier_ID { get; set; }
        public string Supplier_Name { get; set; }
        public string SupplierCode { get; set; }
        public string Supplier_Currency { get; set; }
        public decimal Supplier_MinNightRate { get; set; }
        public decimal Supplier_AllNightRate { get; set; }
        public decimal Supplier_AllNightOfferRate { get; set; }
        public string ComissionVal { get; set; }
        public string IsComissionPerOrVal { get; set; }
        public decimal Commissions { get; set; }
    }
    public class EViewCalculation
    {
        public EViewCalculation()
        {

        }
        public EViewCalculation(EViewCalculation evcobj)
        {
            this.HotelName = evcobj.HotelName;
            this.OtherCharges = evcobj.OtherCharges;
            this.OtherChargesDesc = evcobj.OtherChargesDesc;
            this.IsOffer = evcobj.IsOffer;
            this.OfferStr = evcobj.OfferStr;
            this.Rates = evcobj.Rates;
            this.Currency = evcobj.Currency;
            this.ROE = evcobj.ROE;
            this.DiscountRate = evcobj.DiscountRate;
            this.AddExtraService = evcobj.AddExtraService;
        }

        public EViewCalculation(List<EViewCalculation> EViewCalculationLst)
        {
            try
            {
                var Defaultdata = EViewCalculationLst.FirstOrDefault();
                this.HotelName = Defaultdata.HotelName;
                this.OtherCharges = EViewCalculationLst.Sum(c => c.OtherCharges);
                this.OtherChargesDesc = string.Join("<br/>", EViewCalculationLst.Select(c => c.OtherChargesDesc));
                this.IsOffer = EViewCalculationLst.Where(d => d.IsOffer.Equals(true)).ToList().Count != 0;
                this.OfferStr = string.Join("<br/>", EViewCalculationLst.Select(c => c.OtherChargesDesc));
                this.Currency = Defaultdata.Currency;
                this.ROE = Defaultdata.ROE;
                this.DiscountRate = EViewCalculationLst.Sum(c => c.DiscountRate);
                this.AddExtraService = EViewCalculationLst.Sum(c => c.AddExtraService);
                var RatesLst = new List<EDateWiseRoomRate>();
                foreach (var dd in EViewCalculationLst)
                {
                    foreach (var dd1 in dd.Rates)
                    {
                        RatesLst.Add(dd1);
                    }
                }
                var grop = from r in RatesLst
                           group r by new { date = r.Date } into grp
                           select new EDateWiseRoomRate() { Date = grp.Key.date, Rate = grp.Sum(g => g.Rate) };
                this.Rates = grop.ToList();

            }
            catch (Exception ex)
            {
            }

        }
        public string HotelName { get; set; }
        public decimal OtherCharges { get; set; }
        public string OtherChargesDesc { get; set; }
        public bool IsOffer { get; set; }
        public string OfferStr { get; set; }
        public List<EDateWiseRoomRate> Rates { get; set; }
        public string Currency { get; set; }
        public decimal ROE { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal AddExtraService { get; set; }

    }
    public class EDateWiseRoomRate
    {
        public string? Date { get; set; }
        public decimal Rate { get; set; }
    }
    public class GMXImages
    {
        public GMXImages()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public string url { get; set; }//Image Path
        public string des { get; set; }//Image Description
    }
    public abstract class etosImageGallary
    {
        public string? ImageThumPath { get; set; }
        public string? ImageActualPath { get; set; }
        public string? ImageName { get; set; }
    }
    public class SupplierRoomDefaultInfo
    {
        public SupplierRoomDefaultInfo()
        {
            ComissionVal = string.Empty;
            IsComissionPerOrVal = string.Empty;
            Commissions = 0;
        }
        public int SupplierID { get; set; }
        public string SupplierCurrency { get; set; }
        public decimal SupplierAllNightRate { get; set; }
        public string SupplierMarkupType { get; set; }
        public decimal SupplierMarkup { get; set; }
        public decimal SupplierTotalMarkupValue { get; set; }
        public decimal SysCurrencyRate { get; set; }
        public decimal SysTotalProfit { get; set; }
        public decimal SysProfit { get; set; }
        public decimal SysTax { get; set; }
        public string SupplierCode { get; set; }
        public string ComissionVal { get; set; }
        public string IsComissionPerOrVal { get; set; }
        public decimal Commissions { get; set; }
        //public GMX_SupplierMarkup GMX_SupplierMarkupObj { get; set; }
    }
    public class GMXRoomOccupancyInfo
    {
        public int RoomNo { get; set; }
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public List<int>? ChildAges { get; set; }
        private string? ChildAgesStr { get; set; }
    }

    public class RAWRateCls
    {
        public DateTime RateDate { get; set; }
        public decimal SGL_MID { get; set; }
        public int SGL { get; set; }
        public decimal DBL_MID { get; set; }
        public int DBL { get; set; }
        public decimal TPL_MID { get; set; }
        public int TPL { get; set; }
        public decimal QUAD_MID { get; set; }
        public int QUAD { get; set; }
        public decimal HEXA_MID { get; set; }
        public int HEXA { get; set; }
        public decimal C02_MID { get; set; }
        public int InfantCount { get; set; }
        public decimal CHDILD_MID { get; set; }
        public int ChildCount { get; set; }
        public decimal TEEN_MID { get; set; }
        public int TeenagerCount { get; set; }
        public decimal TEENAGER_MID { get; set; }
        public int TeenagerOwnCount { get; set; }
    }

    public class RoomTypeUpgradeClass
    {
        public RoomTypeUpgradeClass()
        {
            RoomTypeDesc = string.Empty;
            RoomTypeID = 0;
        }
        public RoomTypeUpgradeClass(string RTD, int RTID)
        {
            RoomTypeDesc = RTD;
            RoomTypeID = RTID;
        }

        public RoomTypeUpgradeClass(List<RoomTypeUpgradeClass> objlst)
        {
            var defaultobj = objlst.FirstOrDefault();
            this.Index = defaultobj.Index;
            this.RoomTypeDesc = defaultobj.RoomTypeDesc;
            this.RoomTypeID = defaultobj.RoomTypeID;
            this.ARRoomTypeCost = objlst.Sum(g => g.ARRoomTypeCost);
            this.ARCurrency = defaultobj.ARCurrency;
            this.ARROE = defaultobj.ARROE;
            this.APCurrency = defaultobj.APCurrency;
            this.APROE = defaultobj.APROE;
            this.APRoomTypeCost = objlst.Sum(g => g.APRoomTypeCost);
            this.SysProfitWithTax = objlst.Sum(g => g.SysProfitWithTax);
            this.SysProfit = objlst.Sum(g => g.SysProfit);
            this.SysTax = objlst.Sum(g => g.SysTax);
            this.InputTax = objlst.Sum(g => g.InputTax);
            this.MarkUP = objlst.Sum(g => g.MarkUP);
            this.OutputTax = objlst.Sum(g => g.OutputTax);
        }

        public int Index { get; set; }
        public string RoomTypeDesc { get; set; }
        public int RoomTypeID { get; set; }
        public decimal ARRoomTypeCost { get; set; }
        public string ARCurrency { get; set; }
        public decimal ARROE { get; set; }
        public string APCurrency { get; set; }
        public decimal APROE { get; set; }
        public decimal APRoomTypeCost { get; set; }
        public decimal SysProfitWithTax { get; set; }
        public decimal SysProfit { get; set; }
        public decimal SysTax { get; set; }
        public decimal InputTax { get; set; }
        public decimal MarkUP { get; set; }
        public decimal OutputTax { get; set; }
    }

    public class ERoomTypeUpgradeClass
    {
        public ERoomTypeUpgradeClass()
        {

        }
        public ERoomTypeUpgradeClass(List<ERoomTypeUpgradeClass> objlst)
        {
            try
            {
                var group = from item in objlst.SelectMany(f => f.RoomTypeUpgradeInfo).Select(b => b)
                            group item by new { MealPlanID = item.RoomTypeID }
                                into hjh
                            select new RoomTypeUpgradeClass(hjh.ToList());
                if (group != null && group.Count() != 0)
                {
                    this.RoomTypeUpgradeInfo = group.ToList();
                }
            }
            catch (Exception ex)
            {

            }
        }
        public List<RoomTypeUpgradeClass> RoomTypeUpgradeInfo { get; set; }
    }

    public class HotelSupplimentInfo
    {
        public HotelSupplimentInfo()
        {

        }
        public HotelSupplimentInfo(int HotelID, int RoomTypeID)
        {
            this.Index = -1;
            this.HotelID = HotelID;
            this.RoomTypeID = RoomTypeID;
            this.IsCompulsory = 0;
            this.SupplimentID = 0;
            this.SupplimentDesc = "None";
            this.HotelMarketID = 0;
            this.AdultSurcharge = 0;
            this.ChildSurcharge = 0;
            this.SupplimentSurcharge = 0;
        }

        public HotelSupplimentInfo(List<HotelSupplimentInfo> objlst)
        {
            var defaultobj = objlst.FirstOrDefault();
            this.Index = defaultobj.Index;
            this.RoomTypeID = defaultobj.RoomTypeID;
            this.IsCompulsory = defaultobj.IsCompulsory;
            this.SupplimentID = defaultobj.SupplimentID;
            this.SupplimentDesc = defaultobj.SupplimentDesc;
            this.HotelMarketID = defaultobj.HotelMarketID;
            this.AdultSurcharge = objlst.Sum(g => g.AdultSurcharge);
            this.ChildSurcharge = objlst.Sum(g => g.ChildSurcharge);
            this.SupplimentSurcharge = objlst.Sum(g => g.SupplimentSurcharge);

        }
        public int Index { get; set; }
        public int HotelID { get; set; }
        public int RoomTypeID { get; set; }
        public int IsCompulsory { get; set; }
        public int SupplimentID { get; set; }
        public string SupplimentDesc { get; set; }
        public int HotelMarketID { get; set; }
        public decimal AdultSurcharge { get; set; }
        public decimal ChildSurcharge { get; set; }
        public decimal SupplimentSurcharge { get; set; }
        public decimal APSupplimentSurcharge { get; set; }
        public decimal InputTax { get; set; }
        public decimal MarkUP { get; set; }
        public decimal OutputTax { get; set; }
    }

    public class EHotelSupplimentInfo
    {
        public EHotelSupplimentInfo()
        {

        }

        public EHotelSupplimentInfo(List<EHotelSupplimentInfo> objlst)
        {

            try
            {
                var group = from item in objlst.SelectMany(f => f.HotelSupplimentInfo_).Select(b => b)
                            group item by new { MealPlanID = item.SupplimentID }
                                into hjh
                            select new HotelSupplimentInfo(hjh.ToList());
                if (group != null && group.Count() != 0)
                {
                    this.HotelSupplimentInfo_ = group.ToList();
                }
            }
            catch (Exception ex)
            {

            }
        }
        public List<HotelSupplimentInfo> HotelSupplimentInfo_ { get; set; }

    }

    public class EarlycheckinLateCheckoutList
    {
        public EarlycheckinLateCheckoutList()
        {

        }

        public object index { get; set; }
        public string HotelID { get; set; }
        public string RoomTypeID { get; set; }
        public object IsAfterBefore { get; set; }
        public object EarlycheckinLatecheckout { get; set; }
        public object FullNights { get; set; }
        public object HotelMarketId { get; set; }
        public object HotelSeasonId { get; set; }
        public object Name { get; set; }
        public object ToTime { get; set; }
        public object ChargePercentage { get; set; }
        public object TotalPrice { get; set; }
    }

    public class EarlycheckinLateCheckout
    {
        public EarlycheckinLateCheckout()
        {

        }
        public List<EarlycheckinLateCheckoutList> EarlycheckinLateCheckoutList_ { get; set; }
    }

    //For MealInclusionExclusion
    public class MealInclusionExclusionList
    {
        public MealInclusionExclusionList()
        {

        }
        public object index { get; set; }
        public object HotelID { get; set; }
        public object MealId { get; set; }
        public object Inclusion { get; set; }
        public object Exclusion { get; set; }
        public object DocumentName { get; set; }
        public object DocumentPath { get; set; }

    }

    public class MealInclusionExclusion
    {
        public MealInclusionExclusion()
        {

        }
        public List<MealInclusionExclusionList> MealInclusionExclusionList_ { get; set; }
    }

    public class EHotelCancellationPolicy
    {
        public EHotelCancellationPolicy()
        {

        }
        public class EHotelCancellationPolicy_Req
        {
            public EHotelCancellationPolicy_Req()
            {
                //
                // TODO: Add constructor logic here
                //

            }
            public string Supplier { get; set; }
            public string ProductInfo { get; set; }
            public string RoomProductInfo { get; set; }
            public string CheckIn { get; set; }
            public string CheckOut { get; set; }
            public string Extrainfo { get; set; }
            public string Night { get; set; }
            public string AdultCount { get; set; }
            public string ChildCount { get; set; }
            public string AllNigntOfferRate { get; set; }
            public int ClientID { get; set; }
            public string ApiKey { get; set; }
            public int BookingID { get; set; }
            public string HRDID { get; set; }
            public int RoomIndex { get; set; }
            public string SpecialRequest { get; set; }
            public string NationalityCode { get; set; }
            public string RateType { get; set; }
            public int AgentContactId { get; set; }
            public int AgentId { get; set; }
        }

        public class EHotelCancellationPolicy_Res
        {
            public string? FreeCostCancellationDate { get; set; }
            public bool IsFreeCostCancellationDate { get; set; }
            public bool IsCanclePolicy { get; set; }
            public string? CanclePolicy { get; set; }
            public string? HotelNorms { get; set; }
            public string? IsHotelNorms { get; set; }
        }
    }
}
