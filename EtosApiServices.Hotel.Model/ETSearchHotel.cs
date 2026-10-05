using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Model
{
    public class ETSearchHotel
    {
        public ETSearchHotel(List<ETSearchHotel> HList)
        {
            var sorthotelitem = HList.OrderByDescending(g => g.SRating).FirstOrDefault();
            if (sorthotelitem != null)
            {
                this.Name = sorthotelitem.Name;
                this.Address = sorthotelitem.Address;
                this.Location = sorthotelitem.Location;
                this.SRating = sorthotelitem.SRating;
                this.HCode = string.Join(",", HList.Select(f => f.HCode).ToArray().Distinct());
                this.Latitude = sorthotelitem.Latitude;
                this.Longitude = sorthotelitem.Longitude;
                this.MImage = sorthotelitem.MImage;
                this.AImage = HList.Where(d => d.AImage != null).SelectMany(f => f.AImage).Select(i => i).ToList();
                this.Supplier = string.Join(",", HList.Select(f => f.Supplier).ToArray().Distinct());
                if (HList.Select(f => f.SupplierAliasName).Count() > 0)
                    this.SupplierAliasName = string.Join(",", HList.Select(f => f.SupplierAliasName).ToArray().Distinct());
                else
                    this.SupplierAliasName = this.Supplier;
                this.PropertyType = "Hotel";
                this.Currency = sorthotelitem.Currency;
                this.HotelString = string.Join(",", HList.Select(f => f.HotelString).ToArray().Distinct());
                this.ExtraInfo = string.Join(",", HList.Select(f => f.ExtraInfo).ToArray().Distinct());
                decimal MinHotelPrice = 0;
                MinHotelPrice = HList.Min(d => d.NRate);
                this.NRate = MinHotelPrice;
                this.TRate = HList.Min(d => d.TRate);
                this.ORate = MinHotelPrice;
                this.Amenities = sorthotelitem.Amenities;
                this.GiataID = sorthotelitem.GiataID;
                this.TotalTax = sorthotelitem.TotalTax;
                this.TotalMarkUp = sorthotelitem.TotalMarkUp;
                this.TaxType = sorthotelitem.TaxType;
                this.isAvailable = sorthotelitem.isAvailable;
                this.Transportation = sorthotelitem.Transportation;
            }
        }
        public string? Name { get; set; }//Name
        public string? Address { get; set; }//Address
        public string? Location { get; set; }//location
        public string? SRating { get; set; }//Star Rating
        public string? HCode { get; set; }//Hotel Code
        public string? Latitude { get; set; }//Latitude
        public string? Longitude { get; set; }//longitude
        public string? MImage { get; set; }//main img
        public List<string>? AImage { get; set; }//main img
        public string? Supplier { get; set; }//min per night rate
        public string? SupplierAliasName { get; set; }//min per night rate
        public string? PropertyType { get; set; }//Property Type
        public string? ExtraInfo { get; set; }//API ExtraInfo
        public string? HotelString { get; set; }//hotelstring api~hotelid i.e. hc
        public string? HotelIndex { get; set; }//Is Offer
        public List<string>? AmenitiesIds { get; set; }//Amenity Ids
        public List<string>? Amenities { get; set; }//Amenity Ids
        public List<string>? MealPlans { get; set; }//Meal Plans
        public List<string>? RoomTypes { get; set; }//Room Types
        public string? ZoneName { get; set; }//ZoneName
        public int IsPromotion { get; set; }//Is Promotion
        public int PromoName { get; set; }//PromoName
        public string? Currency { get; set; }//main img
        public decimal NRate { get; set; }//min per night rate
        public decimal TRate { get; set; }//min per night rate
        public decimal ORate { get; set; }//Offer per night rate
        public string? IsOffer { get; set; }//Is Offer       
        public string? OfferCount { get; set; }  /// Offer Count
        public int GiataID { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalMarkUp { get; set; }
        public string? TaxType { get; set; }
        public int isAvailable { get; set; }
        public int IsHotelPackage { get; set; }
        public string? Transportation { get; set; }
                
    }
}
