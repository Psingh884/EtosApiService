using EtosApiServices.Model.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Model
{
    public class EHotelSearchRequest
    {
        public int ClientID { get; set; }
        public string? ApiKey { get; set; }
        public int LangId { get; set; }
        public int SupplierID { get; set; }


        public DateTime CheckInDate_;

        public DateTime CheckOutDate_;
        public string? CheckInDate
        {
            get { return CheckInDate_.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture); }
            set { DateTime.TryParseExact(value, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out CheckInDate_); }
        }

        public string? CheckOutDate
        {
            get { return CheckOutDate_.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture); }
            set { DateTime.TryParseExact(value, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out CheckOutDate_); }
        }
        public int Nights { get; set; }
        public int TotalNoRoom { get; set; }
        public string? NationalityCode { get; set; }
        public string? CurrencyCode { get; set; }
        public EHotelSearchType? SearchType { get; set; }
        public ESectorInfo? DestinationInfo { get; set; }
        public List<ERoomOccupancyInfo>? RoomOccupancyDetails { get; set; }
        public ERequestedUserInfo? RequestedUserInfo { get; set; }
        public int Agent_ContactedID { get; set; }
        public string? tblSearchTraceID { get; set; }
        public int IsFeatured { get; set; }
        public string? SearchURL { get; set; }
        public int IsHotelPromotion { get; set; }
        public List<int>? HotelPromotionIds { get; set; }
        public int IsSplitRoom { get; set; }
        public string? SearchTraceKey { get; set; }
        public int ismodifysearch { get; set; }

        [SwaggerExclude]
        public int PlayerId { get; set; }

        [SwaggerExclude]
        public string? DBConnectioString { get; set; }
    }

    public class ERoomOccupancyInfo
    {
        public ERoomOccupancyInfo()
        {
            this.RoomNo = 1;
            this.RoomType = string.Empty;
            this.AdultCount = 2;
            this.ChildCount = 0;
            this.ChildAges = new List<int>();
            this.ChildAgesStr = string.Empty;
        }
        public int RoomNo { get; set; }
        public string RoomType { get; set; }
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public List<int> ChildAges { get; set; }
        public string? ChildAgesStr { get; set; }
    }

    public class ERequestedUserInfo
    {
        public ReqUserType UserType { get; set; }
        public int LoginEmployeeID { get; set; }
        public int AgentID { get; set; }
        public int Agent_ContactedID { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
    }

    public enum ReqUserType
    {
        B2C = 0,
        B2B_Sub_Agent = 1,
        B2B_Corporate = 2
    }

    public enum EHotelSearchType
    {
        Giata = 1,
        Normal = 0
    }

    public class ESectorInfo
    {
        public ESectorInfo()
        {
            this.CityId = 0;
            this.CityShortName = string.Empty;
            this.CityFullName = string.Empty;
            this.CountryID = "0";
            this.CountryName = string.Empty;
            this.CountryCode3 = string.Empty;
            this.CountryCode2 = string.Empty;
            this.IsRadiusSearch = false;
            this.lat = 0;
            this.lng = 0;
            this.radiusinkm = "0";
            this.SectorExtraInfo = string.Empty;
            this.HotelIds = new List<string>();
            this.CityCode3 = string.Empty;
            this.CountryCode = string.Empty;
        }

        public int CityId { get; set; }
        public string CityFullName { get; set; }
        public string CountryID { get; set; }
        public string CountryName { get; set; }
        public string CountryCode3 { get; set; }
        public string CountryCode2 { get; set; }
        public string CityShortName { get; set; }
        public string CityCode3 { get; set; }
        /// <summary>
        /// Country Code In ISO-2 
        /// </summary>
        public string CountryCode { get; set; }
        public bool IsRadiusSearch { get; set; }
        public string radiusinkm { get; set; }

        private double lat_ = 0;
        [Required]
        public double lat
        {
            get { return lat_; }
            set { double.TryParse(Convert.ToString(value), out lat_); }
        }
        private double lng_ = 0;
        [Required]
        public double lng
        {
            get { return lng_; }
            set { double.TryParse(Convert.ToString(value), out lng_); }
        }

        [Required]
        public int RadiusinMeter { get; set; }
        public string SectorExtraInfo { get; set; }
        public List<string> HotelIds { get; set; }
    }
}
