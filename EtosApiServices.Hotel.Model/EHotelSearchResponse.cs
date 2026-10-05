using EtosApiServices.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Model
{
    public class EHotelSearchResponse
    {
        public EHotelSearchResponse(EHotelSearchRequest req)
        {
            InitializeDefaults(req);
        }

        public EHotelSearchResponse(EHotelSearchRequest req, List<ETSearchHotel> EHotelListlst, string FolderPath)
        {
            InitializeDefaults(req);

            if (EHotelListlst == null || EHotelListlst.Count == 0)
            {
                // keep defaults already set
                return;
            }

            this.HotelCount = Convert.ToString(EHotelListlst.Count);
            this.Request = req;
            this.ResponseCode = Status.successful;

            // Merge and normalize
            var merged = MergeEHotelList(EHotelListlst, FolderPath, Guid.NewGuid().ToString());
            var normalized = new List<ETSearchHotel>(merged.Count);

            int index = 0;
            foreach (var hitem in merged)
            {
                var newObj = hitem;
                newObj.HotelIndex = Convert.ToString(index++);

                if (int.TryParse(newObj.SRating, out var sRatingParsed))
                {
                    newObj.SRating = sRatingParsed.ToString();
                }
                else
                {
                    // default to 0 for any missing or invalid rating
                    newObj.SRating = string.IsNullOrWhiteSpace(newObj.SRating) ? "0" : "0";
                }

                normalized.Add(newObj);
            }

            this.ResponseData = normalized;
            this.MinRate = normalized.Count != 0 ? Convert.ToString(normalized.Select(f => Convert.ToDecimal(f.NRate)).Min()) : "0";
            this.MaxRate = normalized.Count != 0 ? Convert.ToString(normalized.Select(f => Convert.ToDecimal(f.NRate)).Max()) : "0";
            this.GMXHotelPropertyCategoryLst = normalized.Select(g => g.PropertyType).Distinct().ToList();

        }

        private void InitializeDefaults(EHotelSearchRequest req)
        {
            this.HotelNameLst = new List<string>();
            this.CategoryLst = new List<EHotelCategory>();
            this.LocationLst = new List<EHotelLocation>();
            this.OfferListHotels = string.Empty;
            this.MinRate = string.Empty;
            this.MaxRate = string.Empty;
            this.HotelCount = string.Empty;
            this.Request = req;
            this.ResponseCode = Status.successful;
            this.ResponseData = new List<ETSearchHotel>();
            this.GMXHotelPropertyCategoryLst = new List<string>();
            this.AmenitysnList = new List<string>();
            this.MealPlanList = new List<string>();
            this.AmenitysnListdef = new Dictionary<string, string>();
            this.MealPlanListdef = new Dictionary<string, string>();
            this.RoomTypeList = new List<string>();
            this.ZoneList = new List<EHotelLocation>();
        }

        public List<List<T>> GetSetOfHotelIds<T>(List<T> SHotelIDs, int RowCount)
        {
            var result = new List<List<T>>();
            if (SHotelIDs == null || SHotelIDs.Count == 0 || RowCount <= 0)
                return result;

            var distinct = SHotelIDs.Distinct().ToList();
            if (distinct.Count == 0)
                return result;

            int total = distinct.Count;
            int pages = (total + RowCount - 1) / RowCount; // ceil

            for (int i = 0; i < pages; i++)
            {
                result.Add(distinct.Skip(i * RowCount).Take(RowCount).ToList());
            }

            return result;
        }

        public List<ETSearchHotel> MergeEHotelList(List<ETSearchHotel> EHotelListlst, string FolderPath, string requestId)
        {
            var result = new List<ETSearchHotel>();
            if (EHotelListlst == null || EHotelListlst.Count == 0)
                return result;

            try
            {
                var groups = EHotelListlst
                    .Where(item => !string.IsNullOrWhiteSpace(item?.Name))
                    .GroupBy(item => item.Name!.Trim().ToLowerInvariant());

                foreach (var g in groups)
                {
                    var obj = new ETSearchHotel(g.ToList());
                    result.Add(obj);
                }
            }
            catch (Exception)
            {
                // swallow to preserve existing behavior; consider logging
            }

            return result;
        }

        public string? SearchTraceKey { get; set; }
        public List<string> HotelNameLst { get; set; }
        public List<EHotelCategory> CategoryLst { get; set; }
        public List<EHotelLocation> LocationLst { get; set; }
        public string? OfferListHotels { get; set; }
        public string? MinRate { get; set; }
        public string? MaxRate { get; set; }
        public string? HotelCount { get; set; }
        public string? tblSearchTraceID { get; set; }
        public EHotelSearchRequest Request { get; set; }
        public Status ResponseCode { get; set; }
        public List<ETSearchHotel> ResponseData { get; set; }
        public List<string> GMXHotelPropertyCategoryLst { get; set; }
        public List<string> AmenitysnList { get; set; }
        public List<string> MealPlanList { get; set; }
        public List<string> RoomTypeList { get; set; }
        public List<EHotelLocation> Propertype { get; set; }
        public Dictionary<string, string> MealPlanListdef { get; set; }
        public Dictionary<string, string> AmenitysnListdef { get; set; }
        public List<EHotelLocation> ZoneList { get; set; }
    }

    public class EHotelCategory
    {
        public string? Category { get; set; }
        public string? CategoryCount { get; set; }
    }
    public class EHotelLocation
    {
        public string? Location { get; set; }
        public string? LocationCount { get; set; }
    }
}
