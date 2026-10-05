using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EtosApiServices.Hotel.Model
{
    public class TBOModelClass
    {
        public class Tbov7HotelGiataHotelCodeList
        {

            [XmlRoot(ElementName = "Action", Namespace = "http://www.w3.org/2005/08/addressing")]
            public class Action
            {
                [XmlAttribute(AttributeName = "mustUnderstand", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
                public string? MustUnderstand { get; set; }
                [XmlText]
                public string? Text { get; set; }
            }

            [XmlRoot(ElementName = "Header", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
            public class Header
            {
                [XmlElement(ElementName = "Action", Namespace = "http://www.w3.org/2005/08/addressing")]
                public Action? Action { get; set; }
            }

            [XmlRoot(ElementName = "Status", Namespace = "http://TekTravel/HotelBookingApi")]
            public class Status
            {
                [XmlElement(ElementName = "StatusCode", Namespace = "http://TekTravel/HotelBookingApi")]
                public string? StatusCode { get; set; }
                [XmlElement(ElementName = "Description", Namespace = "http://TekTravel/HotelBookingApi")]
                public string? Description { get; set; }
            }

            [XmlRoot(ElementName = "Hotel", Namespace = "http://TekTravel/HotelBookingApi")]
            public class Hotel
            {
                [BsonId]
                public ObjectId _Id { get; set; }

                [DataMember]
                public string? Id { get; set; }

                [XmlAttribute(AttributeName = "CityCode")]
                public string? CityCode { get; set; }
                [XmlAttribute(AttributeName = "HotelCode")]
                public string? HotelCode { get; set; }
                [XmlAttribute(AttributeName = "HotelName")]
                public string? HotelName { get; set; }
                [XmlAttribute(AttributeName = "HotelAddress")]
                public string? HotelAddress { get; set; }
                [XmlAttribute(AttributeName = "Latitude")]
                public string? Latitude { get; set; }
                [XmlAttribute(AttributeName = "Longitude")]
                public string? Longitude { get; set; }
                [XmlAttribute(AttributeName = "StarRating")]
                public string? StarRating { get; set; }
                [XmlAttribute(AttributeName = "CountryName")]
                public string? CountryName { get; set; }
                [XmlAttribute(AttributeName = "CityName")]
                public string? CityName { get; set; }
                [XmlAttribute(AttributeName = "CountryCode")]
                public string? CountryCode { get; set; }
                public Tbov7HotelDetailsResponseInfo.HotelDetails? HotelDetails { get; set; }

                public Geometry? Geometry { get; set; }

                [DataMember]
                public object? Distance { get; set; }
            }

            [XmlRoot(ElementName = "Hotels", Namespace = "http://TekTravel/HotelBookingApi")]
            public class Hotels
            {
                [XmlElement(ElementName = "Hotel", Namespace = "http://TekTravel/HotelBookingApi")]
                public List<Hotel>? Hotel { get; set; }
            }

            [XmlRoot(ElementName = "GiataHotelCodesResponse", Namespace = "http://TekTravel/HotelBookingApi")]
            public class GiataHotelCodesResponse
            {
                [XmlElement(ElementName = "Status", Namespace = "http://TekTravel/HotelBookingApi")]
                public Status? Status { get; set; }
                [XmlElement(ElementName = "Hotels", Namespace = "http://TekTravel/HotelBookingApi")]
                public Hotels? Hotels { get; set; }
                [XmlAttribute(AttributeName = "xmlns")]
                public string? Xmlns { get; set; }
            }

            [XmlRoot(ElementName = "Body", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
            public class Body
            {
                [XmlElement(ElementName = "GiataHotelCodesResponse", Namespace = "http://TekTravel/HotelBookingApi")]
                public GiataHotelCodesResponse? GiataHotelCodesResponse { get; set; }
                [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
                public string? Xsi { get; set; }
                [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
                public string? Xsd { get; set; }
            }

            [XmlRoot(ElementName = "Envelope", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
            public class Envelope
            {
                [XmlElement(ElementName = "Header", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
                public Header? Header { get; set; }
                [XmlElement(ElementName = "Body", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
                public Body? Body { get; set; }
                [XmlAttribute(AttributeName = "s", Namespace = "http://www.w3.org/2000/xmlns/")]
                public string? S { get; set; }
                [XmlAttribute(AttributeName = "a", Namespace = "http://www.w3.org/2000/xmlns/")]
                public string? A { get; set; }
            }
        }

        public class Tbov7HotelDetailsResponseInfo
        {
            [XmlRoot(ElementName = "Action", Namespace = "http://www.w3.org/2005/08/addressing")]
            public class Action
            {
                [XmlAttribute(AttributeName = "mustUnderstand", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
                public string? MustUnderstand { get; set; }
                [XmlText]
                public string? Text { get; set; }
            }

            [XmlRoot(ElementName = "Header", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
            public class Header
            {
                [XmlElement(ElementName = "Action", Namespace = "http://www.w3.org/2005/08/addressing")]
                public Action? Action { get; set; }
            }

            [XmlRoot(ElementName = "Status", Namespace = "http://TekTravel/HotelBookingApi")]
            public class Status
            {
                [XmlElement(ElementName = "StatusCode", Namespace = "http://TekTravel/HotelBookingApi")]
                public string? StatusCode { get; set; }
                [XmlElement(ElementName = "Description", Namespace = "http://TekTravel/HotelBookingApi")]
                public string? Description { get; set; }
            }

            [XmlRoot(ElementName = "Attractions", Namespace = "http://TekTravel/HotelBookingApi")]
            public class Attractions
            {
                [XmlElement(ElementName = "Attraction", Namespace = "http://TekTravel/HotelBookingApi")]
                public string? Attraction { get; set; }
            }

            [XmlRoot(ElementName = "HotelFacilities", Namespace = "http://TekTravel/HotelBookingApi")]
            public class HotelFacilities
            {
                [XmlElement(ElementName = "HotelFacility", Namespace = "http://TekTravel/HotelBookingApi")]
                public List<string>? HotelFacility { get; set; }
            }

            [XmlRoot(ElementName = "ImageUrls", Namespace = "http://TekTravel/HotelBookingApi")]
            public class ImageUrls
            {
                [XmlElement(ElementName = "ImageUrl", Namespace = "http://TekTravel/HotelBookingApi")]
                public List<string>? ImageUrl { get; set; }
            }

            [XmlRoot(ElementName = "Images", Namespace = "http://TekTravel/HotelBookingApi")]
            public class Images
            {
                [XmlElement(ElementName = "Image", Namespace = "http://TekTravel/HotelBookingApi")]
                public List<string>? Image { get; set; }
            }

            [XmlRoot(ElementName = "Room", Namespace = "http://TekTravel/HotelBookingApi")]
            public class Room
            {
                [XmlElement(ElementName = "Images", Namespace = "http://TekTravel/HotelBookingApi")]
                public Images? Images { get; set; }
                [XmlAttribute(AttributeName = "RoomName")]
                public string? RoomName { get; set; }
                [XmlAttribute(AttributeName = "RoomTypeCode")]
                public string? RoomTypeCode { get; set; }
            }

            [XmlRoot(ElementName = "RoomInfo", Namespace = "http://TekTravel/HotelBookingApi")]
            public class RoomInfo
            {
                [XmlElement(ElementName = "Room", Namespace = "http://TekTravel/HotelBookingApi")]
                public List<Room>? Room { get; set; }
            }

            [XmlRoot(ElementName = "HotelDetails", Namespace = "http://TekTravel/HotelBookingApi")]
            public class HotelDetails
            {
                [XmlElement(ElementName = "Address", Namespace = "http://TekTravel/HotelBookingApi")]
                public string? Address { get; set; }
                [XmlElement(ElementName = "Attractions", Namespace = "http://TekTravel/HotelBookingApi")]
                public Attractions? Attractions { get; set; }
                [XmlElement(ElementName = "CountryName", Namespace = "http://TekTravel/HotelBookingApi")]
                public string? CountryName { get; set; }
                [XmlElement(ElementName = "Description", Namespace = "http://TekTravel/HotelBookingApi")]
                public string? Description { get; set; }
                [XmlElement(ElementName = "FaxNumber", Namespace = "http://TekTravel/HotelBookingApi")]
                public string? FaxNumber { get; set; }
                [XmlElement(ElementName = "HotelFacilities", Namespace = "http://TekTravel/HotelBookingApi")]
                public HotelFacilities? HotelFacilities { get; set; }
                [XmlElement(ElementName = "ImageUrls", Namespace = "http://TekTravel/HotelBookingApi")]
                public ImageUrls? ImageUrls { get; set; }
                [XmlElement(ElementName = "Map", Namespace = "http://TekTravel/HotelBookingApi")]
                public string? Map { get; set; }
                [XmlElement(ElementName = "PhoneNumber", Namespace = "http://TekTravel/HotelBookingApi")]
                public string? PhoneNumber { get; set; }
                [XmlElement(ElementName = "PinCode", Namespace = "http://TekTravel/HotelBookingApi")]
                public string? PinCode { get; set; }
                [XmlElement(ElementName = "RoomInfo", Namespace = "http://TekTravel/HotelBookingApi")]
                public RoomInfo? RoomInfo { get; set; }
                [XmlElement(ElementName = "TripAdvisorRating", Namespace = "http://TekTravel/HotelBookingApi")]
                public string? TripAdvisorRating { get; set; }
                [XmlElement(ElementName = "CityName", Namespace = "http://TekTravel/HotelBookingApi")]
                public string? CityName { get; set; }
                [XmlAttribute(AttributeName = "HotelCode")]
                public string? HotelCode { get; set; }
                [XmlAttribute(AttributeName = "HotelName")]
                public string? HotelName { get; set; }
                [XmlAttribute(AttributeName = "HotelRating")]
                public string? HotelRating { get; set; }

                [XmlAttribute(AttributeName = "Email")]
                public string? Email { get; set; }

                [XmlAttribute(AttributeName = "HotelPolicy")]
                public string? HotelPolicy { get; set; }

            }

            [XmlRoot(ElementName = "HotelDetailsResponse", Namespace = "http://TekTravel/HotelBookingApi")]
            public class HotelDetailsResponse
            {
                [XmlElement(ElementName = "Status", Namespace = "http://TekTravel/HotelBookingApi")]
                public Status? Status { get; set; }
                [XmlElement(ElementName = "HotelDetails", Namespace = "http://TekTravel/HotelBookingApi")]
                public HotelDetails? HotelDetails { get; set; }
                [XmlAttribute(AttributeName = "xmlns")]
                public string? Xmlns { get; set; }
            }

            [XmlRoot(ElementName = "Body", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
            public class Body
            {
                [XmlElement(ElementName = "HotelDetailsResponse", Namespace = "http://TekTravel/HotelBookingApi")]
                public HotelDetailsResponse? HotelDetailsResponse { get; set; }
                [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
                public string? Xsi { get; set; }
                [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
                public string? Xsd { get; set; }
            }

            [XmlRoot(ElementName = "Envelope", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
            public class Envelope
            {
                [XmlElement(ElementName = "Header", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
                public Header? Header { get; set; }
                [XmlElement(ElementName = "Body", Namespace = "http://www.w3.org/2003/05/soap-envelope")]
                public Body? Body { get; set; }
                [XmlAttribute(AttributeName = "s", Namespace = "http://www.w3.org/2000/xmlns/")]
                public string? S { get; set; }
                [XmlAttribute(AttributeName = "a", Namespace = "http://www.w3.org/2000/xmlns/")]
                public string? A { get; set; }
            }
        }
    }

    public class Geometry
    {
        public string? type { get; set; }
        public List<double>? coordinates { get; set; }
    }

    public class TBOHotelSearchRequest
    {
        public string? CheckIn { get; set; }
        public string? CheckOut { get; set; }
        public string? HotelCodes { get; set; }
        public string? GuestNationality { get; set; }
        public List<PaxRoom>? PaxRooms { get; set; }
        public double ResponseTime { get; set; }
        public bool IsDetailedResponse { get; set; }
        public TBOHotelSearchFilters? Filters { get; set; }

        public class PaxRoom
        {
            public int Adults { get; set; }
            public int Children { get; set; }
            public List<int>? ChildrenAges { get; set; }
        }

        public class TBOHotelSearchFilters
        {
            public bool Refundable { get; set; }
            public int NoOfRooms { get; set; }
            public string? MealType { get; set; }
        }
    }

    public class TBOHotelSearchResponse
    {
        public class CancelPolicy
        {
            public string? FromDate { get; set; }
            public string? ChargeType { get; set; }
            public double CancellationCharge { get; set; }
            public string? Index { get; set; }
        }

        public class HotelResult
        {
            public string? HotelCode { get; set; }
            public string? Currency { get; set; }
            public List<Room>? Rooms { get; set; }
        }

        public class Room
        {
            public List<string>? Name { get; set; }
            public string? BookingCode { get; set; }
            public string Inclusion { get; set; }
            public List<List<DayRate>>? DayRates { get; set; }
            public double TotalFare { get; set; }
            public double TotalTax { get; set; }
            public List<string>? RoomPromotion { get; set; }
            public List<CancelPolicy>? CancelPolicies { get; set; }
            public string? MealType { get; set; }
            public bool IsRefundable { get; set; }
            public bool WithTransfers { get; set; }
            public List<List<Supplement>>? Supplements { get; set; }
        }

        public class Supplement
        {
            public string? Index { get; set; }
            public string? Type { get; set; }
            public string? Description { get; set; }
            public string? Price { get; set; }
            public string? Currency { get; set; }
        }

        public class DayRate
        {
            public double BasePrice { get; set; }
        }

        public class SearchResponse
        {
            public Status? Status { get; set; }
            public List<HotelResult>? HotelResult { get; set; }
        }

        public class Status
        {
            public int Code { get; set; }
            public string? Description { get; set; }
        }
    }

    public class TBOHotelPreBookRequest
    {
        public class PreBookRequest
        {
            public string? BookingCode { get; set; }
            public string? PaymentMode { get; set; }
        }
    }

    public class TBOHotelPreBookResponse
    {
        public class CancelPolicy
        {
            public string? FromDate { get; set; }
            public string? ChargeType { get; set; }
            public double CancellationCharge { get; set; }
        }

        public class HotelResult
        {
            public string? HotelCode { get; set; }
            public string? Currency { get; set; }
            public List<Room>? Rooms { get; set; }
            public List<string>? RateConditions { get; set; }
        }

        public class Room
        {
            public List<string>? Name { get; set; }
            public string? BookingCode { get; set; }
            public string? Inclusion { get; set; }
            public List<List<DayRate>>? DayRates { get; set; }
            public double TotalFare { get; set; }
            public double TotalTax { get; set; }
            public List<object>? RoomPromotion { get; set; }
            public List<CancelPolicy>? CancelPolicies { get; set; }
            public string? MealType { get; set; }
            public bool IsRefundable { get; set; }
            public List<List<Supplement>>? Supplements { get; set; }
            public bool WithTransfers { get; set; }
            public List<string>? Amenities { get; set; }
        }

        public class DayRate
        {
            public double BasePrice { get; set; }
        }

        public class Supplement
        {
            public string? Index { get; set; }
            public string? Type { get; set; }
            public string? Description { get; set; }
            public string? Price { get; set; }
            public string? Currency { get; set; }
        }

        public class PreBookResponse
        {
            public Status? Status { get; set; }
            public List<HotelResult>? HotelResult { get; set; }
        }

        public class Status
        {
            public int Code { get; set; }
            public string? Description { get; set; }
        }
    }

    public class TBOHotelBookRequest
    {
        public class CustomerDetail
        {
            public List<CustomerName>? CustomerNames { get; set; }
        }

        public class CustomerName
        {
            public string? Title { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Type { get; set; }
        }

        public class BookingRequest
        {
            public string? BookingCode { get; set; }
            public List<CustomerDetail>? CustomerDetails { get; set; }
            public string? ClientReferenceId { get; set; }
            public string? BookingReferenceId { get; set; }
            public double TotalFare { get; set; }
            public string? EmailId { get; set; }
            public string? PhoneNumber { get; set; }
            public string? BookingType { get; set; }
        }
    }

    public class TBOHotelBookResponse
    {
        public class BookingResponse
        {
            public Status? Status { get; set; }
            public string? ClientReferenceId { get; set; }
            public string? ConfirmationNumber { get; set; }
        }

        public class Status
        {
            public int Code { get; set; }
            public string? Description { get; set; }
        }
    }

    public class TBOHotelCancelRequest
    {
        public class CancelRequest
        {
            public string? ConfirmationNumber { get; set; }
        }
    }

    public class TBOHotelCancelResponse
    {
        public class CancelResponse
        {
            public Status? Status { get; set; }
            public string? ConfirmationNumber { get; set; }
        }

        public class Status
        {
            public int Code { get; set; }
            public string? Description { get; set; }
        }
    }

    public enum TboApiv7StarRating
    {
        All = 0,
        OneStar = 1,
        TwoStar = 2,
        ThreeStar = 3,
        FourStar = 4,
        FiveStar = 5,
    }

    public enum TboApiv7CancellationChargeTypeForHotel
    {
        Fixed = 0,
        Percentage = 1,
        Night = 2,
    }
}
