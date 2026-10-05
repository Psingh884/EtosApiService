using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using static EtosApiServices.Hotel.Model.HotelBedsAvailabilityRequest;
using static EtosApiServices.Hotel.Model.HotelBedsAvailabilityResponse;
using static EtosApiServices.Hotel.Model.HotelBedsFacility;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EtosApiServices.Hotel.Model
{
    public class HotelBedsModelClass
    {
        public static Dictionary<string, string> Loadfacility = new Dictionary<string, string>();
        public static string HotelBedsFolderPath = string.Empty;
        public static string ImagePath = string.Empty;
        public HotelBedsModelClass()
        {
            Loadfacilities();
        }

        public void Loadfacilities()
        {
            try
            {
                string Filestr = File.ReadAllText(HotelBedsFolderPath + "HotelBedsFacilityCode.fact");
                var HotelBedsFacilityObj = JsonConvert.DeserializeObject<HotelBedsFacility.Root>(Filestr);
                if (HotelBedsFacilityObj != null && HotelBedsFacilityObj.facilities != null)
                {
                    var sdd = HotelBedsFacilityObj.facilities.Select(f => new { key = Convert.ToString(f.code) + "_" + Convert.ToString(f.facilityGroupCode), value = f.description != null ? f.description.content : string.Empty }).ToList();
                    foreach (var dd in sdd)
                    {
                        if (!Loadfacility.ContainsKey(dd.key))
                        {
                            Loadfacility.Add(dd.key, dd.value ?? "");
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }
        public static Dictionary<int, string> LoadfacilityGroups()
        {
            var Result = new Dictionary<int, string>();
            try
            {
                Result.Add(10, "Location");
                Result.Add(100, "Points of interest");
                Result.Add(20, "Hotel type");
                Result.Add(30, "Methods of payment");
                Result.Add(40, "Distances (in meters)");
                Result.Add(60, "Room facilities (Standard room)");
                Result.Add(61, "Room Distribution");
                Result.Add(62, "Room distribution Alternative");
                Result.Add(70, "Facilities");
                Result.Add(71, "Catering");
                Result.Add(72, "Business");
                Result.Add(73, "Entertainment");
                Result.Add(74, "Health");
                Result.Add(75, "Green Programmes - Worldwide");
                Result.Add(76, "Green Programmes - Europe");
                Result.Add(77, "Green Programmes - Americas");
                Result.Add(78, "Green Programmes - Asia-Pacific");
                Result.Add(79, "Green Programmes - Africa");
                Result.Add(80, "Meals");
                Result.Add(85, "Things to keep in mind");
                Result.Add(90, "Sports");
                Result.Add(91, "Healthy & Safety (COVID)");

            }
            catch (Exception ex)
            {

            }
            return Result;
        }
        public static Dictionary<string, string> LoadimageTypes()
        {
            var Result = new Dictionary<string, string>();
            try
            {
                Result.Add("BAR", "Bar");
                Result.Add("COM", "Lobby");
                Result.Add("CON", "Conferences");
                Result.Add("DEP", "Sports and Entertainment");
                Result.Add("GEN", "General view");
                Result.Add("HAB", "Room");
                Result.Add("PIS", "Pool");
                Result.Add("PLA", "Beach");
                Result.Add("RES", "Restaurant");
                Result.Add("TER", "Terrace");


            }
            catch (Exception ex)
            {

            }
            return Result;
        }
        public class HotelBedsHotelDetail
        {
            public class HotelBedsHotelDetail_Root
            {
                public List<Hotel>? hotels { get; set; }
            }

            public class Hotel
            {
                [BsonId]
                public ObjectId _Id { get; set; }

                [DataMember]
                public string? Id { get; set; }
                public int code { get; set; }
                public Name? name { get; set; }
                public Description? description { get; set; }
                public Address? address { get; set; }
                public string? countryCode { get; set; }
                public string? stateCode { get; set; }
                public string? destinationCode { get; set; }
                public int zoneCode { get; set; }
                public Coordinates? coordinates { get; set; }
                public string? categoryCode { get; set; }
                public string? categoryGroupCode { get; set; }
                public string? chainCode { get; set; }
                public string? accommodationTypeCode { get; set; }
                public List<int>? segmentCodes { get; set; }
                public string? postalCode { get; set; }
                public City? city { get; set; }
                public string? email { get; set; }
                public string? license { get; set; }
                public List<Phone>? phones { get; set; }
                public List<Room>? rooms { get; set; }
                public List<Facility>? facilities { get; set; }
                public List<Terminal>? terminals { get; set; }
                public List<Issue>? issues { get; set; }
                public List<Image>? images { get; set; }
                public List<Wildcard>? wildcards { get; set; }
                public string? web { get; set; }
                public string? lastUpdate { get; set; }
                public string? S2C { get; set; }
                public int ranking { get; set; }
                public Geometry? Geometry { get; set; }
            }

            public class Name
            {
                public string? content { get; set; }
            }

            public class Coordinates
            {
                public double longitude { get; set; }
                public double latitude { get; set; }
            }
            public class Address
            {
                public string? content { get; set; }
                public string? street { get; set; }
                public string? number { get; set; }
                public string? floor { get; set; }
            }

            public class City
            {
                public string? content { get; set; }
            }

            public class Phone
            {
                public string? phoneNumber { get; set; }
                public string? phoneType { get; set; }
            }

            public class RoomFacility
            {
                private int _facilityCode;
                public int facilityCode
                {

                    get { return _facilityCode; }   // get method
                    set
                    {
                        _facilityCode = value;
                    }  // set method

                }
                public string? facilityCodeDesc { get; set; }

                private int facilityGroupCode_;
                public int facilityGroupCode
                {
                    get { return facilityGroupCode_; }   // get method
                    set
                    {
                        facilityGroupCode_ = value;
                        if (LoadfacilityGroups().ContainsKey(facilityGroupCode_))
                        {
                            this.facilityGroupCodeDesc = LoadfacilityGroups()[facilityGroupCode_];
                        }
                        if (Loadfacility.ContainsKey(Convert.ToString(_facilityCode) + "_" + Convert.ToString(facilityGroupCode_)))
                        {
                            this.facilityCodeDesc = Loadfacility[Convert.ToString(_facilityCode) + "_" + Convert.ToString(facilityGroupCode_)];
                        }
                    }  // set method

                }
                public string? facilityGroupCodeDesc { get; set; }

                public bool indLogic { get; set; }
                public int number { get; set; }
                public bool voucher { get; set; }
                public bool? indFee { get; set; }
                public bool? indYesOrNo { get; set; }
            }

            public class RoomStayFacility
            {
                private int _facilityCode;
                public int facilityCode
                {
                    get { return _facilityCode; }   // get method
                    set
                    {
                        _facilityCode = value;
                    }  // set method

                }
                public string? facilityCodeDesc { get; set; }

                private int facilityGroupCode_;
                public int facilityGroupCode
                {
                    get { return facilityGroupCode_; }   // get method
                    set
                    {
                        facilityGroupCode_ = value;
                        if (LoadfacilityGroups().ContainsKey(facilityGroupCode_))
                        {
                            this.facilityGroupCodeDesc = LoadfacilityGroups()[facilityGroupCode_];
                        }
                        if (Loadfacility.ContainsKey(Convert.ToString(_facilityCode) + "_" + Convert.ToString(facilityGroupCode_)))
                        {
                            this.facilityCodeDesc = Loadfacility[Convert.ToString(_facilityCode) + "_" + Convert.ToString(facilityGroupCode_)];
                        }
                    }  // set method

                }
                public string? facilityGroupCodeDesc { get; set; }

                public int number { get; set; }
            }

            public class RoomStay
            {
                public string? stayType { get; set; }
                public string? order { get; set; }
                public string? description { get; set; }
                public List<RoomStayFacility>? roomStayFacilities { get; set; }
            }

            public class Room
            {
                public string? roomCode { get; set; }
                public int minPax { get; set; }
                public int maxPax { get; set; }
                public int maxAdults { get; set; }
                public int maxChildren { get; set; }
                public int minAdults { get; set; }
                public string? roomType { get; set; }
                public string? characteristicCode { get; set; }
                public List<RoomFacility>? roomFacilities { get; set; }
                public List<RoomStay>? roomStays { get; set; }
                public string? PMSRoomCode { get; set; }
            }

            public class Image
            {
                public string? imageTypeCodeDes { get; set; }

                private string? imageTypeCode_;
                public string? imageTypeCode
                {
                    get { return imageTypeCode_; }   // get method
                    set
                    {
                        imageTypeCode_ = value;
                        if (LoadimageTypes().ContainsKey(imageTypeCode_))
                        {
                            imageTypeCodeDes = LoadimageTypes()[imageTypeCode_];
                        }
                    }  // set method

                }
                private string? path_;
                public string? path
                {

                    get { return path_; }
                    set
                    {
                        path_ = ImagePath + value;
                    }
                }
                public int order { get; set; }
                public int visualOrder { get; set; }
                public string? roomCode { get; set; }
                public string? roomType { get; set; }
                public string? characteristicCode { get; set; }
            }

            public class Facility
            {
                private int _facilityCode;
                public int facilityCode
                {
                    get { return _facilityCode; }
                    set { _facilityCode = value; }

                }
                public string? facilityCodeDesc { get; set; }

                private int facilityGroupCode_;
                public int facilityGroupCode
                {
                    get { return facilityGroupCode_; }   // get method
                    set
                    {
                        facilityGroupCode_ = value;
                        if (HotelBedsModelClass.LoadfacilityGroups().ContainsKey(facilityGroupCode_))
                        {
                            this.facilityGroupCodeDesc = HotelBedsModelClass.LoadfacilityGroups()[facilityGroupCode_];
                        }
                        if (HotelBedsModelClass.Loadfacility.ContainsKey(Convert.ToString(_facilityCode) + "_" + Convert.ToString(facilityGroupCode_)))
                        {
                            this.facilityCodeDesc = HotelBedsModelClass.Loadfacility[Convert.ToString(_facilityCode) + "_" + Convert.ToString(facilityGroupCode_)];
                        }
                    }  // set method
                }
                public string? facilityGroupCodeDesc { get; set; }
                public int order { get; set; }
                public bool indYesOrNo { get; set; }
                public int number { get; set; }
                public bool voucher { get; set; }
                public bool? indLogic { get; set; }
                public bool? indFee { get; set; }
                public int? distance { get; set; }
                public string? timeFrom { get; set; }
                public string? timeTo { get; set; }
                public double? amount { get; set; }
                public string? currency { get; set; }
                public string? applicationType { get; set; }
                public string? dateTo { get; set; }
            }

            public class Terminal
            {
                public string? terminalCode { get; set; }
                public int distance { get; set; }
            }

            public class Issue
            {
                public string? issueCode { get; set; }
                public string? issueType { get; set; }
                public string? dateFrom { get; set; }
                public string? dateTo { get; set; }
                public int order { get; set; }
                public bool alternative { get; set; }
            }

            public class Wildcard
            {
                public string? roomType { get; set; }
                public string? roomCode { get; set; }
                public string? characteristicCode { get; set; }
                public HotelRoomDescription? hotelRoomDescription { get; set; }
            }

            public class HotelRoomDescription
            {
                public string? content { get; set; }
            }

            public class Geometry
            {
                public string? type { get; set; }
                public List<double>? coordinates { get; set; }
            }

        }
    }

    public class HotelBedsAvailabilityRequest
    {
        public class HotelSearchRequest
        {
            [JsonPropertyName("stay")]
            public Stay? stay { get; set; }

            [JsonPropertyName("occupancies")]
            public List<Occupancy>? occupancies { get; set; }

            [JsonPropertyName("hotels")]
            public Hotels hotels { get; set; }

            [JsonPropertyName("geolocation")]
            public Geolocation? geolocation { get; set; }
        }

        public class Stay
        {
            [JsonPropertyName("checkIn")]
            public string? checkIn { get; set; }

            [JsonPropertyName("checkOut")]
            public string? checkOut { get; set; }
        }

        public class Occupancy
        {
            [JsonPropertyName("rooms")]
            public int rooms { get; set; }

            [JsonPropertyName("adults")]
            public int adults { get; set; }

            [JsonPropertyName("children")]
            public int children { get; set; }

            [JsonPropertyName("paxes")]
            public List<Pax>? paxes { get; set; }
        }

        public class Pax
        {
            [JsonPropertyName("type")]
            public string? type { get; set; }

            [JsonPropertyName("age")]
            public int age { get; set; }
        }

        public class Hotels
        {
            [JsonPropertyName("hotel")]
            public List<int> hotel { get; set; }
        }

        public class Geolocation
        {
            [JsonPropertyName("latitude")]
            public double latitude { get; set; }

            [JsonPropertyName("longitude")]
            public double longitude { get; set; }

            [JsonPropertyName("radius")]
            public int radius { get; set; }

            [JsonPropertyName("unit")]
            public string? unit { get; set; }
        }
    }

    public class HotelBedsAvailabilityResponse
    {
        public class HotelSearchResponse
        {
            public AuditData? auditData { get; set; }
            public HotelsResponse? hotels { get; set; }
            public hotelbeds_error? error { get; set; }
        }

        public class AuditData
        {
            public string? processTime { get; set; }
            public string? timestamp { get; set; }
            public string? requestHost { get; set; }
            public string? serverId { get; set; }
            public string? environment { get; set; }
            public string? release { get; set; }
            public string? token { get; set; }
            public string? @internal { get; set; }
        }

        public class HotelsResponse
        {
            public List<Hotel> hotels { get; set; } = [];
            public string? checkIn { get; set; }
            public int total { get; set; }
            public string? checkOut { get; set; }
        }

        public class hotelbeds_error
        {
            public string? code { get; set; }
            public string? message { get; set; }
        }

        public class Hotel
        {
            public int code { get; set; }
            public string? name { get; set; }
            public string? categoryCode { get; set; }
            public string? categoryName { get; set; }
            public string? destinationCode { get; set; }
            public string? destinationName { get; set; }
            public int zoneCode { get; set; }
            public string? zoneName { get; set; }
            public string? latitude { get; set; }
            public string? longitude { get; set; }
            public string? minRate { get; set; }
            public string? maxRate { get; set; }
            public string? currency { get; set; }
        }
    }

    public class HotelBedsDetailsAvailabilityResponse
    {
        public class HotelDetailsResponse
        {
            public AuditData? auditData { get; set; }
            public APIHotels? hotels { get; set; }
        }
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class AuditData
        {
            public string? processTime { get; set; }
            public string? timestamp { get; set; }
            public string? requestHost { get; set; }
            public string? serverId { get; set; }
            public string? environment { get; set; }
            public string? release { get; set; }
            public string? token { get; set; }
            public string? @internal { get; set; }
        }

        public class CancellationPolicy
        {
            public string? amount { get; set; }
            public DateTime from { get; set; }
        }

        public class APIHotels
        {
            public List<Hotels>? hotels { get; set; }
            public string? checkIn { get; set; }
            public string? checkOut { get; set; }
            public int total { get; set; }
        }
        public class Hotels
        {
            public int code { get; set; }
            public string? name { get; set; }
            public string? categoryCode { get; set; }
            public string? categoryName { get; set; }
            public string? destinationCode { get; set; }
            public string? destinationName { get; set; }
            public int zoneCode { get; set; }
            public string? zoneName { get; set; }
            public string? latitude { get; set; }
            public string? longitude { get; set; }
            public List<Room>? rooms { get; set; }
            public decimal minRate { get; set; }
            public decimal maxRate { get; set; }
            public string? currency { get; set; }
        }

        public class Offer
        {
            public string? code { get; set; }
            public string? name { get; set; }
            public string? amount { get; set; }
        }

        public class Rate
        {
            public string? rateKey { get; set; }
            public string? rateClass { get; set; }
            public string? rateType { get; set; }
            public decimal net { get; set; }
            public decimal discount { get; set; }
            public decimal discountPCT { get; set; }
            public decimal sellingRate { get; set; }
            public decimal hotelSellingRate { get; set; }
            public bool hotelMandatory { get; set; }
            public int allotment { get; set; }
            public decimal commission { get; set; }
            public decimal commissionVAT { get; set; }
            public decimal commissionPCT { get; set; }
            public string? rateCommentsId { get; set; }
            public string? rateComments { get; set; }
            public string? paymentType { get; set; }
            public bool packaging { get; set; }
            public string? boardCode { get; set; }
            public string? boardName { get; set; }
            public List<CancellationPolicy>? cancellationPolicies { get; set; }
            public ApiTaxes? taxes { get; set; }
            public int rooms { get; set; }
            public int adults { get; set; }
            public int children { get; set; }
            public string? childrenAges { get; set; }
            public List<Offer>? offers { get; set; }
            public List<ApiDailyRate>? dailyRate { get; set; }
            public List<Promotion>? promotions { get; set; }
        }

        public class ApiDailyRate
        {
            public int offset { get; set; }
            public decimal dailyNet { get; set; }
            public decimal dailySellingRate { get; set; }
        }

        public class Promotion
        {
            public string? code { get; set; }
            public string? name { get; set; }
            public string? remark { get; set; }
        }

        public class Room
        {
            public string? code { get; set; }
            public string? name { get; set; }
            public List<Rate>? rates { get; set; }
        }
        public class ApiTaxes
        {
            public List<Taxes>? taxes { get; set; }
            public bool allIncluded { get; set; }
        }
        public class Taxes
        {
            public bool allIncluded { get; set; }
            public bool included { get; set; }
            public string? amount { get; set; }
            public string? currency { get; set; }
            public string? type { get; set; }
            public string? clientAmount { get; set; }
            public string? clientCurrency { get; set; }
            public string? subType { get; set; }
        }
    }

    public class HotelBedscheckrates
    {
        public class HotelBedscheckratesReq
        {
            public List<RoomReq>? rooms { get; set; }
        }
        public class RoomReq
        {
            public string? rateKey { get; set; }
        }
    }

    public class CheckResponse
    {
        public class AuditData
        {
            public string? processTime { get; set; }
            public string? timestamp { get; set; }
            public string? requestHost { get; set; }
            public string? serverId { get; set; }
            public string? environment { get; set; }
            public string? release { get; set; }
            public string? token { get; set; }
            public string? @internal { get; set; }
        }

        public class CancellationPolicy
        {
            public string? amount { get; set; }
            public DateTime from { get; set; }
            public string? hotelAmount { get; set; }
            public string? hotelCurrency { get; set; }
        }

        public class Tax
        {
            public bool included { get; set; }
            public string? amount { get; set; }
            public string? currency { get; set; }
            public string? clientAmount { get; set; }
            public string? clientCurrency { get; set; }
            public List<Tax>? taxes { get; set; }
            public bool allIncluded { get; set; }
        }

        public class Rate
        {
            public string? rateKey { get; set; }
            public string? rateClass { get; set; }
            public string? rateType { get; set; }
            public string? net { get; set; }
            public string? rateComments { get; set; }
            public string? paymentType { get; set; }
            public bool packaging { get; set; }
            public string? boardCode { get; set; }
            public string? boardName { get; set; }
            public List<CancellationPolicy>? cancellationPolicies { get; set; }
            public object? taxes { get; set; }
            public int rooms { get; set; }
            public int adults { get; set; }
            public int children { get; set; }
            public decimal sellingRate { get; set; }
            public string? hotelSellingRate { get; set; }
            public string? hotelCurrency { get; set; }
            public bool hotelMandatory { get; set; }
            public int allotment { get; set; }
            public string? commission { get; set; }
            public string? commissionVAT { get; set; }
            public string? commissionPCT { get; set; }
            public string? rateCommentsId { get; set; }
            public string? rateup { get; set; }
        }

        public class Room
        {
            public string? code { get; set; }
            public string? name { get; set; }
            public List<Rate> rates { get; set; }
        }

        public class CreditCard
        {
            public string? code { get; set; }
            public string? name { get; set; }
            public string? paymentType { get; set; }
        }

        public class Upselling
        {
            public List<Room>? rooms { get; set; }
        }

        public class Hotel
        {
            public string? checkOut { get; set; }
            public string? checkIn { get; set; }
            public int code { get; set; }
            public string? name { get; set; }
            public string? categoryCode { get; set; }
            public string? categoryName { get; set; }
            public string? destinationCode { get; set; }
            public string? destinationName { get; set; }
            public int zoneCode { get; set; }
            public string? zoneName { get; set; }
            public string? latitude { get; set; }
            public string? longitude { get; set; }
            public List<Room>? rooms { get; set; }
            public string? totalNet { get; set; }
            public string? currency { get; set; }
            public List<CreditCard>? creditCards { get; set; }
            public Upselling? upselling { get; set; }
            public bool paymentDataRequired { get; set; }
        }
        public class Error
        {
            public string? code { get; set; }
            public string? message { get; set; }
        }

        public class HotelBedscheckratesRes
        {
            public AuditData? auditData { get; set; }
            public Hotel? hotel { get; set; }
            public Error? error { get; set; }

        }
    }

    public class HotelBedsFacility
    {
        public class Description
        {
            public string? languageCode { get; set; }
            public string? content { get; set; }
        }

        public class Facility
        {
            public int code { get; set; }
            public int facilityGroupCode { get; set; }
            public int facilityTypologyCode { get; set; }
            public Description? description { get; set; }
        }

        public class Root
        {
            public int from { get; set; }
            public int to { get; set; }
            public int total { get; set; }
            public List<Facility>? facilities { get; set; }
        }

    }

    public class HBRateCheck
    {
        public HBRateCheck()
        {

        }
        public string rateKeys { get; set; }
        public decimal RatePrice { get; set; }
        public string rateComments { get; set; }
        public string boardName { get; set; }
        public string Rooms { get; set; }
        public string CancellationAmount { get; set; }
        public DateTime CancellationDate { get; set; }
        public string CancellationStr { get; set; }
        public List<CancellationPolicy1> cancellationPolicies { get; set; }
        public class CancellationPolicy1
        {
            public string CancellationAmount { get; set; }
            public DateTime CancellationDate { get; set; }

        }

        public string boardCode { get; set; }
        public string rateCommentsId { get; set; }
        public string rateType { get; set; }
    }

    public class HotelBedsBookingRequest
    {
        public class Holder
        {
            public string? name { get; set; }
            public string? surname { get; set; }
        }

        public class Pax
        {
            public int roomId { get; set; }
            public string? type { get; set; }
            public string? name { get; set; }
            public string? surname { get; set; }
            public int age { get; set; }
        }

        public class Room
        {
            public string? rateKey { get; set; }
            public List<Pax>? paxes { get; set; }
        }

        public class HotelBedsRequest
        {
            public Holder? holder { get; set; }
            public List<Room>? rooms { get; set; }
            public string? clientReference { get; set; }
            public string? remark { get; set; }
        }
    }

    public class HotelBedsBookingResponse
    {
        public class AuditData
        {
            public string? processTime { get; set; }
            public string? timestamp { get; set; }
            public string? requestHost { get; set; }
            public string? serverId { get; set; }
            public string? environment { get; set; }
            public string? release { get; set; }
            public string? token { get; set; }
            public string? @internal { get; set; }
        }

        public class ModificationPolicies
        {
            public bool cancellation { get; set; }
            public bool modification { get; set; }
        }

        public class Holder
        {
            public string? name { get; set; }
            public string? surname { get; set; }
        }

        public class Pax
        {
            public int roomId { get; set; }
            public string? type { get; set; }
            public string? name { get; set; }
            public string? surname { get; set; }
        }

        public class CancellationPolicy
        {
            public string? amount { get; set; }
            public DateTime from { get; set; }
        }

        public class Rate
        {
            public string? rateClass { get; set; }
            public string? net { get; set; }
            public string? rateComments { get; set; }
            public string? paymentType { get; set; }
            public bool packaging { get; set; }
            public string? boardCode { get; set; }
            public string? boardName { get; set; }
            public List<CancellationPolicy>? cancellationPolicies { get; set; }
            public int rooms { get; set; }
            public int adults { get; set; }
            public int children { get; set; }
        }

        public class Room
        {
            public string? status { get; set; }
            public int id { get; set; }
            public string? code { get; set; }
            public string? name { get; set; }
            public List<Pax>? paxes { get; set; }
            public List<Rate>? rates { get; set; }
        }

        public class Supplier
        {
            public string? name { get; set; }
            public string? vatNumber { get; set; }
        }

        public class Hotel
        {
            public string? checkOut { get; set; }
            public string? checkIn { get; set; }
            public int code { get; set; }
            public string? name { get; set; }
            public string? categoryCode { get; set; }
            public string? categoryName { get; set; }
            public string? destinationCode { get; set; }
            public string? destinationName { get; set; }
            public int zoneCode { get; set; }
            public string? zoneName { get; set; }
            public string? latitude { get; set; }
            public string? longitude { get; set; }
            public List<Room>? rooms { get; set; }
            public string? totalNet { get; set; }
            public string? currency { get; set; }
            public Supplier? supplier { get; set; }
        }

        public class InvoiceCompany
        {
            public string? code { get; set; }
            public string? company { get; set; }
            public string? registrationNumber { get; set; }
        }

        public class Booking
        {
            public string? reference { get; set; }
            public string? clientReference { get; set; }
            public string? creationDate { get; set; }
            public string? status { get; set; }
            public ModificationPolicies? modificationPolicies { get; set; }
            public string? creationUser { get; set; }
            public Holder? holder { get; set; }
            public Hotel? hotel { get; set; }
            public string? remark { get; set; }
            public InvoiceCompany? invoiceCompany { get; set; }
            public decimal totalNet { get; set; }
            public double pendingAmount { get; set; }
            public string? currency { get; set; }
        }

        public class error
        {
            public string? code { get; set; }
            public string? message { get; set; }
        }
        public class HotelBedsResponse
        {
            public AuditData? auditData { get; set; }
            public error? error { get; set; }
            public Booking? booking { get; set; }
        }
    }

    public class HotelBedsBookingDetailsClass
    {
        public class AuditData
        {
            public string? processTime { get; set; }
            public string? timestamp { get; set; }
            public string? requestHost { get; set; }
            public string? serverId { get; set; }
            public string? environment { get; set; }
            public string? release { get; set; }
            public string? token { get; set; }
            public string? @internal { get; set; }
        }

        public class ModificationPolicies
        {
            public bool cancellation { get; set; }
            public bool modification { get; set; }
        }

        public class Holder
        {
            public string? name { get; set; }
            public string? surname { get; set; }
        }

        public class Pax
        {
            public int roomId { get; set; }
            public string? type { get; set; }
            public string? name { get; set; }
            public string? surname { get; set; }
        }

        public class CancellationPolicy
        {
            public string? amount { get; set; }
            public DateTime from { get; set; }
        }

        public class Tax
        {
            public bool included { get; set; }
            public string? amount { get; set; }
            public string? currency { get; set; }
            public string? clientAmount { get; set; }
            public string? clientCurrency { get; set; }
            public List<Tax>? taxes { get; set; }
            public bool allIncluded { get; set; }
        }

        public class Rate
        {
            public string? rateClass { get; set; }
            public string? net { get; set; }
            public string? rateComments { get; set; }
            public string? paymentType { get; set; }
            public bool packaging { get; set; }
            public string? boardCode { get; set; }
            public string? boardName { get; set; }
            public List<CancellationPolicy>? cancellationPolicies { get; set; }
            public object? taxes { get; set; }
            public int rooms { get; set; }
            public int adults { get; set; }
            public int children { get; set; }
        }

        public class Room
        {
            public string? status { get; set; }
            public int id { get; set; }
            public string? code { get; set; }
            public string? name { get; set; }
            public List<Pax>? paxes { get; set; }
            public List<Rate>? rates { get; set; }
        }

        public class Supplier
        {
            public string? name { get; set; }
            public string? vatNumber { get; set; }
        }

        public class Hotel
        {
            public string? checkOut { get; set; }
            public string? checkIn { get; set; }
            public int code { get; set; }
            public string? name { get; set; }
            public string? categoryCode { get; set; }
            public string? categoryName { get; set; }
            public string? destinationCode { get; set; }
            public string? destinationName { get; set; }
            public int zoneCode { get; set; }
            public string? zoneName { get; set; }
            public string? latitude { get; set; }
            public string? longitude { get; set; }
            public List<Room>? rooms { get; set; }
            public decimal totalNet { get; set; }
            public string? currency { get; set; }
            public decimal totalSellingRate { get; set; }
            public decimal cancellationAmount { get; set; }
            public Supplier? supplier { get; set; }
        }

        public class InvoiceCompany
        {
            public string? code { get; set; }
            public string? company { get; set; }
            public string? registrationNumber { get; set; }
        }

        public class Booking
        {
            public string? reference { get; set; }
            public string? clientReference { get; set; }
            public string? creationDate { get; set; }
            public string? status { get; set; }
            public ModificationPolicies? modificationPolicies { get; set; }
            public string? creationUser { get; set; }
            public Holder? holder { get; set; }
            public Hotel? hotel { get; set; }
            public string? remark { get; set; }
            public InvoiceCompany? invoiceCompany { get; set; }
            public double totalNet { get; set; }
            public double pendingAmount { get; set; }
            public string? currency { get; set; }
        }

        public class HotelBedsBookingDetail
        {
            public AuditData? auditData { get; set; }
            public Booking? booking { get; set; }
        }
    }
}
