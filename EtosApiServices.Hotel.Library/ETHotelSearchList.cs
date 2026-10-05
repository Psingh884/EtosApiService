using EtosApiServices.Hotel.Model;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Library
{
    public class ETHotelSearchList : ETSearchHotel
    {
        public ETHotelSearchList(int APIID, string APICode, HotelBedsAvailabilityResponse.Hotel ObjHotel, HotelBedsModelClass.HotelBedsHotelDetail.Hotel HBObj, EHotelSearchRequest SearchReq, MarkupClass MarkupClassObj, decimal APIROE, decimal UserROE) : base(new List<ETSearchHotel>())
        {
            if (ObjHotel == null)
                return;

            // Cache frequently used values
            var categoryName = ObjHotel.categoryName;
            var minRateObj = ObjHotel.minRate;
            var currency = ObjHotel.currency;
            var totalRooms = (SearchReq?.TotalNoRoom) ?? 1;
            var nights = (SearchReq?.Nights) ?? 1;

            this.Name = ObjHotel.name;
            this.Address = ObjHotel.destinationName;
            this.Location = ObjHotel.zoneName;

            // Parse star rating without allocations
            if (!string.IsNullOrEmpty(categoryName))
            {
                int star;
                int idx = categoryName.IndexOf(' ');
                string token = idx > 0 ? categoryName.Substring(0, idx) : categoryName;
                if (int.TryParse(token, out star))
                    this.SRating = star.ToString();
                else
                    this.SRating = categoryName;
            }
            else
            {
                this.SRating = null;
            }

            this.HCode = Convert.ToString(ObjHotel.code);
            this.Latitude = Convert.ToString(ObjHotel.latitude);
            this.Longitude = Convert.ToString(ObjHotel.longitude);
            this.Supplier = APICode;
            this.PropertyType = "Hotel";
            this.Currency = currency;
            this.HotelString = string.Concat(APICode, "~", ObjHotel.code);
            this.ExtraInfo = string.Empty;

            // Parse min rate safely
            decimal minHotelPrice = 0m;
            var minRateStr = Convert.ToString(minRateObj);
            if (!string.IsNullOrWhiteSpace(minRateStr))
                decimal.TryParse(minRateStr, out minHotelPrice);

            if (totalRooms <= 0) totalRooms = 1;
            minHotelPrice = minHotelPrice / totalRooms;
            if (APIROE != 0) minHotelPrice = (minHotelPrice / APIROE) * UserROE;

            var dicRates = APIMarkupSection.AddMarkupByRule(APIID, minHotelPrice, MarkupClassObj, this.Currency ?? string.Empty, nights, totalRooms);
            var allNightRates = Convert.ToDecimal(dicRates[APIMarkupSection.AR]);
            this.NRate = allNightRates;
            this.TRate = allNightRates * totalRooms;
            this.ORate = allNightRates;
            this.IsOffer = "0";

            if (HBObj != null)
            {
                this.Address = HBObj.address?.content ?? string.Empty;
                this.PropertyType = Convert.ToString(HBObj.accommodationTypeCode);

                var images = HBObj.images;
                if (images != null && images.Count > 0)
                {
                    this.MImage = images[0].path;
                    //this.AImage = paths;

                    //string main = null;
                    //int minOrder = int.MaxValue;
                    //var paths = new List<string>(images.Count);
                    //foreach (var img in images)
                    //{
                    //    if (img == null) continue;
                    //    paths.Add(img.path ?? "");
                    //    if (img.order < minOrder)
                    //    {
                    //        minOrder = img.order;
                    //        main = img.path ?? "";
                    //    }
                    //}
                    //this.MImage = main;
                    //this.AImage = paths;
                }

                var facilities = HBObj.facilities;
                if (facilities != null && facilities.Count > 0)
                {
                    //var amenities = new List<string>();
                    //foreach (var f in facilities)
                    //{
                    //    if (f != null && f.facilityGroupCode == 70)
                    //        amenities.Add(f.facilityCodeDesc ?? "");
                    //}
                    //if (amenities.Count > 0) this.Amenities = amenities;
                }
            }

            this.TotalTax = Convert.ToDecimal(dicRates[APIMarkupSection.Output_Tax]);
            this.TotalMarkUp = Convert.ToDecimal(dicRates[APIMarkupSection.MT2]);
            this.TaxType = Convert.ToString(dicRates[APIMarkupSection.TaxType]);
        }

        public ETHotelSearchList(int APIID, string APICode, string SessionId, TBOHotelSearchResponse.HotelResult ObjHotel, TBOModelClass.Tbov7HotelGiataHotelCodeList.Hotel TBObj, EHotelSearchRequest SearchReq, MarkupClass MarkupClassObj, decimal APIROE, decimal UserROE, decimal tt) : base(new List<ETSearchHotel>())
        {
            try
            {
                if (ObjHotel != null)
                {
                    int Rating = 0;
                    if (!string.IsNullOrEmpty(TBObj.StarRating))
                    {
                        Rating = (int)Enum.Parse(typeof(TboApiv7StarRating), TBObj.StarRating);
                    }
                    this.Name = TBObj.HotelName;
                    this.Address = TBObj.HotelAddress;
                    this.Location = TBObj.CityName;
                    this.SRating = Convert.ToString(Rating);
                    this.HCode = TBObj.HotelCode;
                    this.Latitude = TBObj.Latitude;
                    this.Longitude = TBObj.Longitude;
                    //this.MImage = Obj.HotelInfo.HotelPicture;
                    if (TBObj.HotelDetails != null && TBObj.HotelDetails.ImageUrls != null && TBObj.HotelDetails.ImageUrls.ImageUrl != null && TBObj.HotelDetails.ImageUrls.ImageUrl.Count != 0)
                    {
                        this.MImage = TBObj.HotelDetails.ImageUrls.ImageUrl[0] ?? string.Empty;
                        //this.AImage = TBObj.HotelDetails.ImageUrls.ImageUrl ?? new List<string>();
                    }
                    this.Supplier = APICode;
                    this.PropertyType = "Hotel";
                    this.Currency = ObjHotel.Currency;
                    this.HotelString = string.Format("{0}~{1}", APICode, TBObj.HotelCode);
                    this.ExtraInfo = string.Format("{0}~{1}", SessionId, "");
                    decimal MinHotelPrice = 0;
                    decimal.TryParse(Convert.ToString(ObjHotel?.Rooms?.Min(x => x.TotalFare)), out MinHotelPrice);
                    MinHotelPrice = (MinHotelPrice / APIROE) * UserROE;
                    var dicRates = APIMarkupSection.AddMarkupByRule(APIID, MinHotelPrice, MarkupClassObj, this.Currency ?? string.Empty, SearchReq.Nights, SearchReq.TotalNoRoom);
                    var AllNightRates = Convert.ToDecimal(dicRates[APIMarkupSection.AR]);
                    MinHotelPrice = AllNightRates / SearchReq.TotalNoRoom;
                    this.NRate = MinHotelPrice;
                    this.TRate = MinHotelPrice * SearchReq.TotalNoRoom;
                    this.ORate = MinHotelPrice;
                    this.IsOffer = "0";
                    if (TBObj.HotelDetails != null && TBObj.HotelDetails.HotelFacilities != null && TBObj.HotelDetails.HotelFacilities.HotelFacility != null && TBObj.HotelDetails.HotelFacilities.HotelFacility.Count != 0)
                    {
                        //this.Amenities = TBObj.HotelDetails.HotelFacilities.HotelFacility;
                    }
                    this.TotalTax = Convert.ToDecimal(dicRates[APIMarkupSection.Output_Tax]);
                    this.TotalMarkUp = Convert.ToDecimal(dicRates[APIMarkupSection.MT2]);
                    this.TaxType = Convert.ToString(dicRates[APIMarkupSection.TaxType]);
                }
            }
            catch (Exception ex)
            {
                //Errorlog.InsertLogError(ex, 1);
            }
        }

    }
}
