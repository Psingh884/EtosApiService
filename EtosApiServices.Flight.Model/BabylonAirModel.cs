using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EtosApiServices.Flight.Model
{
    public class BabylonTokenRequest
    {
        [XmlRoot(ElementName = "TokenRequest")]
        public class TokenRequest
        {
            [XmlElement(ElementName = "APIKey")]
            public string? APIKey { get; set; }

            [XmlElement(ElementName = "SecretKey")]
            public string? SecretKey { get; set; }
        }
    }

    public class BabylonTokenResponse
    {
        [XmlRoot(ElementName = "TokenResponse")]
        public class TokenResponse
        {
            [XmlElement(ElementName = "Token")]
            public string? Token { get; set; }

            [XmlElement(ElementName = "ResponseDateTime")]
            public DateTime ResponseDateTime { get; set; }
        }

        public class BabylonToken
        {
            public TokenResponse? TokenResponse { get; set; }
        }
    }

    public class BabylonFlightSearchRequest
    {
        [XmlRoot(ElementName = "OriginDepCriteria")]
        public class OriginDepCriteria
        {
            [XmlElement(ElementName = "IATALocationCode")]
            public string? IATALocationCode { get; set; }

            [XmlElement(ElementName = "Date")]
            public string? Date { get; set; }
        }

        [XmlRoot(ElementName = "DestArrivalCriteria")]
        public class DestArrivalCriteria
        {
            [XmlElement(ElementName = "IATALocationCode")]
            public string? IATALocationCode { get; set; }
        }

        [XmlRoot(ElementName = "OriginDestCriteria")]
        public class OriginDestCriteria
        {
            [XmlElement(ElementName = "OriginDepCriteria")]
            public OriginDepCriteria OriginDepCriteria { get; set; }

            [XmlElement(ElementName = "DestArrivalCriteria")]
            public DestArrivalCriteria DestArrivalCriteria { get; set; }
        }

        [XmlRoot(ElementName = "FlightCriteria")]
        public class FlightCriteria
        {
            [XmlElement(ElementName = "OriginDestCriteria")]
            public OriginDestCriteria[] OriginDestCriteria { get; set; }
        }

        [XmlRoot(ElementName = "Pax")]
        public class Pax
        {
            [XmlElement(ElementName = "PaxID")]
            public string? PaxID { get; set; }

            [XmlElement(ElementName = "PTC")]
            public string? PTC { get; set; }
        }

        [XmlRoot(ElementName = "Paxs")]
        public class Paxs
        {
            [XmlElement(ElementName = "Pax")]
            public Pax[] Pax { get; set; }
        }

        [XmlRoot(ElementName = "CabinTypeCriteria")]
        public class CabinTypeCriteria
        {
            [XmlElement(ElementName = "CabinTypeCode")]
            public int CabinTypeCode { get; set; }
        }

        [XmlRoot(ElementName = "ShoppingCriteria")]
        public class ShoppingCriteria
        {
            [XmlElement(ElementName = "CabinTypeCriteria")]
            public CabinTypeCriteria CabinTypeCriteria { get; set; }
        }

        [XmlRoot(ElementName = "Request")]
        public class Request
        {
            [XmlElement(ElementName = "FlightCriteria")]
            public FlightCriteria FlightCriteria { get; set; }

            [XmlElement(ElementName = "Paxs")]
            public Paxs Paxs { get; set; }

            [XmlElement(ElementName = "ShoppingCriteria")]
            public ShoppingCriteria ShoppingCriteria { get; set; }
        }

        [XmlRoot(ElementName = "TravelAgency")]
        public class TravelAgency
        {
            [XmlElement(ElementName = "AgencyID")]
            public string? AgencyID { get; set; }
        }

        [XmlRoot(ElementName = "Sender")]
        public class Sender
        {
            [XmlElement(ElementName = "TravelAgency")]
            public TravelAgency TravelAgency { get; set; }
        }

        [XmlRoot(ElementName = "Party")]
        public class Party
        {
            [XmlElement(ElementName = "Sender")]
            public Sender Sender { get; set; }
        }

        [XmlRoot(ElementName = "IATA_AirShoppingRQ")]
        public class IATAAirShoppingRQ
        {
            [XmlElement(ElementName = "Request")]
            public Request Request { get; set; }

            [XmlElement(ElementName = "Party")]
            public Party Party { get; set; }
        }
    }

    public class BabylonFlightSearchResponse
    {

        public class ShoppingResponse
        {
            public string? OwnerCode { get; set; }
            public string? ShoppingResponseID { get; set; }

        }
        public class AllOffersSummary
        {
            public string? MatchedOfferQty { get; set; }

        }
        public class TotalAmount
        {
            public string? CurCode { get; set; }
            public string? text { get; set; }

        }
        public class BaseAmount
        {
            public string? CurCode { get; set; }
            public string? text { get; set; }

        }
        public class TotalTaxAmount
        {
            public string? CurCode { get; set; }
            public string? text { get; set; }
        }
        public class TaxSummary
        {
            public TotalTaxAmount TotalTaxAmount { get; set; }
        }
        public class TotalPrice
        {
            public TotalAmount TotalAmount { get; set; }
            public BaseAmount BaseAmount { get; set; }
            public TaxSummary TaxSummary { get; set; }
        }
        public class BaggageFlightAssociations
        {
            public Object? PaxSegmentRefID { get; set; }
        }
        public class WeightAllowance
        {
            public MaximumWeightMeasure MaximumWeightMeasure { get; set; }
        }
        public class MaximumWeightMeasure
        {
            public string? UnitCode { get; set; }
            public string? text { get; set; }
        }
        public class BaggageAllowance
        {
            public string? BaggageAllowanceRefID { get; set; }
            public Object? PaxRefID { get; set; }
            public BaggageFlightAssociations BaggageFlightAssociations { get; set; }
            public string? BaggageAllowanceID { get; set; }
            public string? TypeCode { get; set; }
            public WeightAllowance WeightAllowance { get; set; }
            public PieceAllowance PieceAllowance { get; set; }
        }
        public class JourneyPriceClass
        {
            public string? PaxJourneyRefID { get; set; }
            public Object? PriceClassRefID { get; set; }

        }
        public class JourneyOverview
        {
            public Object? JourneyPriceClass { get; set; }

        }
        public class Price
        {
            public BaseAmount BaseAmount { get; set; }
            public TotalAmount TotalAmount { get; set; }

        }
        public class ServiceAssociations
        {
            public Object? ServiceDefinitionRef { get; set; }
            public Object? PaxJourneyRefID { get; set; }

        }
        public class Service
        {
            public string? ServiceID { get; set; }
            public string? PaxType { get; set; }
            public Object? PaxRefID { get; set; }
            public ServiceAssociations ServiceAssociations { get; set; }

        }
        public class FarePriceType
        {
            public Object? FarePriceTypeCode { get; set; }
            public Price Price { get; set; }

        }
        public class CabinType
        {
            public string? CabinTypeCode { get; set; }
            public string? CabinTypeName { get; set; }

        }
        public class RBD
        {
            public string? RBD_Code { get; set; }

        }
        public class FareComponent
        {
            public CabinType CabinType { get; set; }
            public string? FareBasisCode { get; set; }
            public string? FareTypeCode { get; set; }
            public Object? PriceClassRef { get; set; }
            public RBD RBD { get; set; }
            public string? PaxSegmentRefID { get; set; }

        }
        public class FareDetail
        {
            public FarePriceType FarePriceType { get; set; }
            public string? FareRefText { get; set; }
            public Object? TourCode { get; set; }
            public Object? FareComponent { get; set; }

        }
        public class StopOverLocation
        {
            public string? MaximumDuration { get; set; }
            public string? PaxSegmentRefID { get; set; }

        }
        public class StopOverRestrictions
        {
            public Object? StopOverLocation { get; set; }

        }
        public class OfferItem
        {
            public string? OfferItemID { get; set; }
            public string? OwnerCode { get; set; }
            public Price Price { get; set; }
            public Service Service { get; set; }
            public FareDetail FareDetail { get; set; }
            public StopOverRestrictions StopOverRestrictions { get; set; }

        }
        public class Offer
        {
            public string? OfferID { get; set; }
            public string? OwnerCode { get; set; }
            public string? ValidatingCarrierCode { get; set; }
            public Object? OfferExpiration { get; set; }
            public TotalPrice TotalPrice { get; set; }
            public Object? BaggageAllowance { get; set; }
            public JourneyOverview JourneyOverview { get; set; }
            public Object? OfferItem { get; set; }

        }
        public class CarrierOffers
        {
            public IList<Offer> Offer { get; set; }

        }
        public class OffersGroup
        {
            public AllOffersSummary AllOffersSummary { get; set; }
            public IList<CarrierOffers> CarrierOffers { get; set; }

        }
        public class Pax
        {
            public string? PaxID { get; set; }
            public string? PTC { get; set; }

        }
        public class PaxList
        {
            public Object? Pax { get; set; }

        }
        public class Dep
        {
            public string? IATALocationCode { get; set; }
            public DateTime AircraftScheduledDateTime { get; set; }
            public Object? BoardingGateID { get; set; }
            public string? StationName { get; set; }
            public string? TerminalName { get; set; }

        }
        public class Arrival
        {
            public string? IATALocationCode { get; set; }
            public DateTime AircraftScheduledDateTime { get; set; }
            public string? StationName { get; set; }
            public string? TerminalName { get; set; }

        }
        public class MarketingCarrierInfo
        {
            public string? CarrierDesigCode { get; set; }
            public string? MarketingCarrierFlightNumberText { get; set; }

        }
        public class OperatingCarrierInfo
        {
            public string? CarrierDesigCode { get; set; }
            public string? OperatingCarrierFlightNumberText { get; set; }

        }
        public class CarrierAircraftType
        {
            public string? CarrierAircraftTypeCode { get; set; }
            public Object? CarrierAircraftTypeName { get; set; }

        }
        public class DatedOperatingLeg
        {
            public CarrierAircraftType CarrierAircraftType { get; set; }

        }
        public class PaxSegment
        {
            public string? PaxSegmentID { get; set; }
            public Dep Dep { get; set; }
            public Arrival Arrival { get; set; }
            public MarketingCarrierInfo MarketingCarrierInfo { get; set; }
            public OperatingCarrierInfo OperatingCarrierInfo { get; set; }
            public string? Duration { get; set; }
            public DatedOperatingLeg DatedOperatingLeg { get; set; }
            public Object? CodeShareFlightNumber { get; set; }

        }
        public class PaxSegmentList
        {
            public IList<PaxSegment> PaxSegment { get; set; }

        }
        public class PaxJourney
        {
            public string? PaxJourneyID { get; set; }
            public string? JourneyTime { get; set; }
            public Object? PaxSegmentRefID { get; set; }
            public string? OnPoint { get; set; }
            public string? OffPoint { get; set; }

        }
        public class PaxJourneyList
        {
            public IList<PaxJourney> PaxJourney { get; set; }

        }
        public class OriginDest
        {
            public string? OriginDestID { get; set; }
            public string? OriginCode { get; set; }
            public string? DestCode { get; set; }
            public IList<string> PaxJourneyRefID { get; set; }

        }
        public class OriginDestList
        {
            public IList<OriginDest> OriginDest { get; set; }

        }
        public class PieceAllowance
        {
            public string? TotalQty { get; set; }

        }
        public class BaggageAllowanceList
        {
            public IList<BaggageAllowance> BaggageAllowance { get; set; }
        }

        public class Desc
        {
            public string? DescText { get; set; }
            public Object? URL { get; set; }
        }
        public class PriceClass
        {
            public CabinType CabinType { get; set; }
            public Object? Code { get; set; }
            public string? Name { get; set; }
            public Object? Desc { get; set; }
            public Object? FareBasisCode { get; set; }
            public string? PriceClassID { get; set; }

        }
        public class PriceClassList
        {
            public IList<PriceClass> PriceClass { get; set; }

        }
        public class DataLists
        {
            public PaxList PaxList { get; set; }
            public PaxSegmentList PaxSegmentList { get; set; }
            public PaxJourneyList PaxJourneyList { get; set; }
            public OriginDestList OriginDestList { get; set; }
            public BaggageAllowanceList BaggageAllowanceList { get; set; }
            public PriceClassList PriceClassList { get; set; }

        }
        public class Response
        {
            public ShoppingResponse ShoppingResponse { get; set; }
            public OffersGroup OffersGroup { get; set; }
            public DataLists DataLists { get; set; }

        }
        public class IATA_AirShoppingRS
        {
            public Response Response { get; set; }

        }
        public class Babylon_AirShoppingRS
        {
            public IATA_AirShoppingRS IATA_AirShoppingRS { get; set; }

        }
    }

    public class BabylonFlightFareRulesRequest
    {
        public class Arrival
        {
            public string? IATALocationCode { get; set; }
            public string? AircraftScheduledDateTime { get; set; }
        }

        public class CoreRequest
        {
            public FareRef FareRef { get; set; }
        }

        public class Dep
        {
            public string? IATALocationCode { get; set; }
            public string? AircraftScheduledDateTime { get; set; }
        }

        public class FareRef
        {
            public string? AirlineDesigCode { get; set; }
            public string? FareRefText { get; set; }
            public string? FareBasisCode { get; set; }
            public string? FareTypeCode { get; set; }
            public string? PriceClassRefID { get; set; }
            public Arrival Arrival { get; set; }
            public Dep Dep { get; set; }
        }

        public class IATA_OrderRulesRQ
        {
            public Request Request { get; set; }
            public Party Party { get; set; }
        }

        public class Party
        {
            public Sender Sender { get; set; }
        }

        public class Request
        {
            public CoreRequest CoreRequest { get; set; }
        }

        public class IATAAirShoppingRequest
        {
            public IATA_OrderRulesRQ IATA_OrderRulesRQ { get; set; }
        }

        public class Sender
        {
            public TravelAgency TravelAgency { get; set; }
        }

        public class TravelAgency
        {
            public string? AgencyID { get; set; }
        }
    }

    public class BabylonFlightFareRulesResponse
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject?<Root>(myJsonResponse);
        public class Arrival
        {
            public string? IATALocationCode { get; set; }
            public string? AircraftScheduledDateTime { get; set; }
        }

        public class Dep
        {
            public string? IATALocationCode { get; set; }
            public string? AircraftScheduledDateTime { get; set; }
        }

        public class FareRule
        {
            public string? RuleCode { get; set; }
            public FareRuleText FareRuleText { get; set; }
        }

        public class FareRuleText
        {
            public string? RemarkText { get; set; }
        }

        public class IATA_OrderRulesRS
        {
            public Response Response { get; set; }
        }

        public class Response
        {
            public Rules Rules { get; set; }
        }

        public class IATAOrderRulesRS
        {
            public IATA_OrderRulesRS IATA_OrderRulesRS { get; set; }
        }

        public class Rules
        {
            public string? AirlineDesigCode { get; set; }
            public Dep Dep { get; set; }
            public Arrival Arrival { get; set; }
            public Object? FareRule { get; set; }
        }


    }

    public class BabylonFlightOfferFareRequest
    {
        [XmlRoot(ElementName = "SelectedOfferItem")]
        public class SelectedOfferItem
        {

            [XmlElement(ElementName = "OfferItemRefID")]
            public string? OfferItemRefID { get; set; }

            [XmlElement(ElementName = "PaxRefID")]
            public List<string> PaxRefID { get; set; }
        }

        [XmlRoot(ElementName = "SelectedOffer")]
        public class SelectedOffer
        {

            [XmlElement(ElementName = "OfferRefID")]
            public string? OfferRefID { get; set; }

            [XmlElement(ElementName = "OwnerCode")]
            public string? OwnerCode { get; set; }

            [XmlElement(ElementName = "ShoppingResponseRefID")]
            public string? ShoppingResponseRefID { get; set; }

            [XmlElement(ElementName = "SelectedOfferItem")]
            public List<SelectedOfferItem> SelectedOfferItem { get; set; }
        }

        [XmlRoot(ElementName = "PricedOffer")]
        public class PricedOffer
        {

            [XmlElement(ElementName = "SelectedOffer")]
            public SelectedOffer SelectedOffer { get; set; }
        }

        [XmlRoot(ElementName = "Pax")]
        public class Pax
        {

            [XmlElement(ElementName = "PaxID")]
            public string? PaxID { get; set; }

            [XmlElement(ElementName = "PTC")]
            public string? PTC { get; set; }
        }

        [XmlRoot(ElementName = "PaxList")]
        public class PaxList
        {

            [XmlElement(ElementName = "Pax")]
            public List<Pax> Pax { get; set; }
        }

        [XmlRoot(ElementName = "DataLists")]
        public class DataLists
        {

            [XmlElement(ElementName = "PaxList")]
            public PaxList PaxList { get; set; }
        }

        [XmlRoot(ElementName = "Request")]
        public class Request
        {

            [XmlElement(ElementName = "PricedOffer")]
            public PricedOffer PricedOffer { get; set; }

            [XmlElement(ElementName = "DataLists")]
            public DataLists DataLists { get; set; }
        }

        [XmlRoot(ElementName = "IATA_OfferPriceRQ")]
        public class IATAOfferPriceRQ
        {

            [XmlElement(ElementName = "Request")]
            public Request Request { get; set; }
        }
    }

    public class BabylonFlightOfferFareResponse
    {
        public class AllowedOperations
        {
            public string? HoldPNR { get; set; }
        }

        public class Amount
        {
            public string? Code { get; set; }

            public string? CurCode { get; set; }

            public string? text { get; set; }
        }

        public class Arrival
        {
            public string? IATALocationCode { get; set; }
            public DateTime AircraftScheduledDateTime { get; set; }
            public string? StationName { get; set; }
            public string? TerminalName { get; set; }
        }

        public class BaggageAllowance
        {
            public string? BaggageAllowanceID { get; set; }
            public string? TypeCode { get; set; }
            public PieceAllowance PieceAllowance { get; set; }
            public WeightAllowance WeightAllowance { get; set; }
            public string? BaggageAllowanceRefID { get; set; }
            public string? PaxRefID { get; set; }
            public BaggageFlightAssociations BaggageFlightAssociations { get; set; }
        }

        public class BaggageAllowanceList
        {
            public Object? BaggageAllowance { get; set; }
        }

        public class BaggageFlightAssociations
        {
            public Object? PaxSegmentRefID { get; set; }
        }

        public class BaseAmount
        {
            public string? CurCode { get; set; }
            public decimal text { get; set; }
        }

        public class CabinType
        {
            public string? CabinTypeCode { get; set; }
            public string? CabinTypeName { get; set; }
        }

        public class CarrierAircraftType
        {
            public string? CarrierAircraftTypeCode { get; set; }
            public Object? CarrierAircraftTypeName { get; set; }
        }

        public class DataLists
        {
            public PaxList PaxList { get; set; }
            public PaxSegmentList PaxSegmentList { get; set; }
            public PaxJourneyList PaxJourneyList { get; set; }
            public Object? PriceClassList { get; set; }
            public OriginDestList OriginDestList { get; set; }
            public BaggageAllowanceList BaggageAllowanceList { get; set; }
            public Object? ServiceDefinitionList { get; set; }
        }

        public class DatedOperatingLeg
        {
            public CarrierAircraftType CarrierAircraftType { get; set; }
        }

        public class Dep
        {
            public string? IATALocationCode { get; set; }
            public DateTime AircraftScheduledDateTime { get; set; }
            public Object? BoardingGateID { get; set; }
            public string? StationName { get; set; }
            public string? TerminalName { get; set; }
        }

        public class FareComponent
        {
            public CabinType CabinType { get; set; }
            public string? FareBasisCode { get; set; }
            public Object? FareTypeCode { get; set; }
            public Object? PriceClassRef { get; set; }
            public RBD RBD { get; set; }
            public string? PaxSegmentRefID { get; set; }
        }

        public class FareDetail
        {
            public FarePriceType FarePriceType { get; set; }
            public Object? FareRefText { get; set; }
            public Object? TourCode { get; set; }
            public Object? FareComponent { get; set; }
        }

        public class FarePriceType
        {
            public Object? FarePriceTypeCode { get; set; }
            public Price Price { get; set; }
        }

        public class IATA_OfferPriceRS
        {
            public Response Response { get; set; }
        }

        public class JourneyOverview
        {
            public Object? JourneyPriceClass { get; set; }
        }

        public class JourneyPriceClass
        {
            public string? PaxJourneyRefID { get; set; }
            public Object? PriceClassRefID { get; set; }
        }

        public class MarketingCarrierInfo
        {
            public string? CarrierDesigCode { get; set; }
            public string? MarketingCarrierFlightNumberText { get; set; }
        }

        public class OfferItem
        {
            public string? OfferItemID { get; set; }
            public Price Price { get; set; }
            public Service Service { get; set; }
            public FareDetail FareDetail { get; set; }
            public StopOverRestrictions StopOverRestrictions { get; set; }
        }

        public class OperatingCarrierInfo
        {
            public string? CarrierDesigCode { get; set; }
            public string? OperatingCarrierFlightNumberText { get; set; }
        }

        public class OriginDest
        {
            public string? DestCode { get; set; }
            public string? OriginCode { get; set; }
            public string? PaxJourneyRefID { get; set; }
            public string? OriginDestID { get; set; }
        }

        public class OriginDestList
        {
            public Object? OriginDest { get; set; }
        }

        public class Pax
        {
            public string? PaxID { get; set; }
            public string? PTC { get; set; }
        }

        public class PaxJourney
        {
            public string? PaxJourneyID { get; set; }
            public string? JourneyTime { get; set; }
            public Object? PaxSegmentRefID { get; set; }
            public string? OnPoint { get; set; }
            public string? OffPoint { get; set; }
        }

        public class PaxJourneyList
        {
            public Object? PaxJourney { get; set; }
        }

        public class PaxList
        {
            public Object? Pax { get; set; }
        }

        public class PaxSegment
        {
            public string? PaxSegmentID { get; set; }
            public Dep Dep { get; set; }
            public Arrival Arrival { get; set; }
            public MarketingCarrierInfo MarketingCarrierInfo { get; set; }
            public OperatingCarrierInfo OperatingCarrierInfo { get; set; }
            public string? Duration { get; set; }
            public DatedOperatingLeg DatedOperatingLeg { get; set; }
            public Object? CodeShareFlightNumber { get; set; }
            public string? SegId { get; set; }
        }

        public class PaxSegmentList
        {
            public Object? PaxSegment { get; set; }
        }

        public class PieceAllowance
        {
            public string? TotalQty { get; set; }
        }

        public class WeightAllowance
        {
            public MaximumWeightMeasure MaximumWeightMeasure { get; set; }
        }
        public class MaximumWeightMeasure
        {
            public string? UnitCode { get; set; }
            public string? text { get; set; }
        }

        public class Price
        {
            public BaseAmount BaseAmount { get; set; }
            public TotalAmount TotalAmount { get; set; }
            public TaxSummary TaxSummary { get; set; }
        }

        public class PricedOffer
        {
            public string? OfferID { get; set; }
            public string? OwnerCode { get; set; }
            public TotalPrice TotalPrice { get; set; }
            public JourneyOverview JourneyOverview { get; set; }
            public Object? OfferItem { get; set; }
            public Object? BaggageAllowance { get; set; }
        }

        public class RBD
        {
            public string? RBD_Code { get; set; }
        }

        public class Response
        {
            public ShoppingResponse ShoppingResponse { get; set; }
            public DataLists DataLists { get; set; }
            public PricedOffer PricedOffer { get; set; }
            public AllowedOperations AllowedOperations { get; set; }
        }

        public class IATAOfferPriceRS
        {
            public IATA_OfferPriceRS IATA_OfferPriceRS { get; set; }
        }

        public class Service
        {
            public string? ServiceID { get; set; }
            public string? PaxType { get; set; }
            public Object? PaxRefID { get; set; }
            public ServiceAssociations ServiceAssociations { get; set; }
        }

        public class ServiceAssociations
        {
            public Object? PaxJourneyRefID { get; set; }
        }

        public class ShoppingResponse
        {
            public string? ShoppingResponseID { get; set; }
        }

        public class StopOverLocation
        {
            public string? MaximumDuration { get; set; }
            public string? PaxSegmentRefID { get; set; }
        }

        public class StopOverRestrictions
        {
            public Object? StopOverLocation { get; set; }
        }

        public class Tax
        {
            public Amount Amount { get; set; }
            public string? TaxCode { get; set; }
            public Object? DescText { get; set; }
        }

        public class TaxSummary
        {
            public TotalTaxAmount TotalTaxAmount { get; set; }
            public Object? Tax { get; set; }
        }

        public class TotalAmount
        {
            public string? CurCode { get; set; }
            public decimal text { get; set; }
        }

        public class TotalPrice
        {
            public TotalAmount TotalAmount { get; set; }
            public BaseAmount BaseAmount { get; set; }
            public TaxSummary TaxSummary { get; set; }
        }

        public class TotalTaxAmount
        {
            public string? CurCode { get; set; }
            public decimal text { get; set; }
        }
    }

    public class BabylonFlightCreateOrderRequest
    {
        [XmlRoot(ElementName = "TravelAgencySender")]
        public class TravelAgencySender
        {
            [XmlElement(ElementName = "AgencyID")]
            public string? AgencyID { get; set; }
        }

        [XmlRoot(ElementName = "Sender")]
        public class Sender
        {
            [XmlElement(ElementName = "TravelAgencySender")]
            public TravelAgencySender TravelAgencySender { get; set; }
        }

        [XmlRoot(ElementName = "Party")]
        public class Party
        {
            [XmlElement(ElementName = "Sender")]
            public Sender Sender { get; set; }
        }

        [XmlRoot(ElementName = "SelectedOfferItem")]
        public class SelectedOfferItem
        {
            [XmlElement(ElementName = "OfferItemRefID")]
            public string? OfferItemRefID { get; set; }

            [XmlElement(ElementName = "PaxRefID")]
            public List<string> PaxRefID { get; set; }
        }

        [XmlRoot(ElementName = "SelectedOffer")]
        public class SelectedOffer
        {
            [XmlElement(ElementName = "OfferRefID")]
            public string? OfferRefID { get; set; }

            [XmlElement(ElementName = "OwnerCode")]
            public string? OwnerCode { get; set; }

            [XmlElement(ElementName = "ShoppingResponseRefID")]
            public string? ShoppingResponseRefID { get; set; }

            [XmlElement(ElementName = "TotalOfferPriceAmount")]
            public decimal TotalOfferPriceAmount { get; set; }

            [XmlElement(ElementName = "Currency")]
            public string? Currency { get; set; }

            [XmlElement(ElementName = "SelectedOfferItem")]
            public List<SelectedOfferItem> SelectedOfferItem { get; set; }
        }

        [XmlRoot(ElementName = "CreateOrder")]
        public class CreateOrder
        {

            [XmlElement(ElementName = "SelectedOffer")]
            public SelectedOffer SelectedOffer { get; set; }
        }

        [XmlRoot(ElementName = "IdentityDoc")]
        public class IdentityDoc
        {

            [XmlElement(ElementName = "IdentityDocTypeCode")]
            public string? IdentityDocTypeCode { get; set; }

            [XmlElement(ElementName = "IdentityDocID")]
            public string? IdentityDocID { get; set; }

            [XmlElement(ElementName = "IssuingCountryCode")]
            public string? IssuingCountryCode { get; set; }

            [XmlElement(ElementName = "ExpiryDate")]
            public string? ExpiryDate { get; set; }

            [XmlElement(ElementName = "ResidenceCountryCode")]
            public string? ResidenceCountryCode { get; set; }
        }

        [XmlRoot(ElementName = "Individual")]
        public class Individual
        {

            [XmlElement(ElementName = "Birthdate")]
            public string? Birthdate { get; set; }

            [XmlElement(ElementName = "IndividualID")]
            public string? IndividualID { get; set; }

            [XmlElement(ElementName = "NameTitle")]
            public string? NameTitle { get; set; }

            [XmlElement(ElementName = "GivenName")]
            public string? GivenName { get; set; }

            [XmlElement(ElementName = "MiddleName")]
            public Object? MiddleName { get; set; }

            [XmlElement(ElementName = "Surname")]
            public string? Surname { get; set; }
        }

        [XmlRoot(ElementName = "Pax")]
        public class Pax
        {

            [XmlElement(ElementName = "IdentityDoc")]
            public IdentityDoc IdentityDoc { get; set; }

            [XmlElement(ElementName = "ContactInfoRefID")]
            public string? ContactInfoRefID { get; set; }

            [XmlElement(ElementName = "PaxID")]
            public string? PaxID { get; set; }

            [XmlElement(ElementName = "PTC")]
            public string? PTC { get; set; }

            [XmlElement(ElementName = "PaxRefID")]
            public string? PaxRefID { get; set; }

            [XmlElement(ElementName = "Individual")]
            public Individual Individual { get; set; }
        }

        [XmlRoot(ElementName = "PaxList")]
        public class PaxList
        {
            [XmlElement(ElementName = "Pax")]
            public List<Pax> Pax { get; set; }
        }

        [XmlRoot(ElementName = "EmailAddress")]
        public class EmailAddress
        {
            [XmlElement(ElementName = "ContactTypeText")]
            public string? ContactTypeText { get; set; }

            [XmlElement(ElementName = "EmailAddressText")]
            public string? EmailAddressText { get; set; }
        }

        [XmlRoot(ElementName = "Phone")]
        public class Phone
        {
            [XmlElement(ElementName = "CountryDialingCode")]
            public string? CountryDialingCode { get; set; }

            [XmlElement(ElementName = "ContactTypeText")]
            public string? ContactTypeText { get; set; }

            [XmlElement(ElementName = "PhoneNumber")]
            public string? PhoneNumber { get; set; }
        }

        [XmlRoot(ElementName = "ContactInfo")]
        public class ContactInfo
        {

            [XmlElement(ElementName = "ContactInfoID")]
            public string? ContactInfoID { get; set; }

            [XmlElement(ElementName = "EmailAddress")]
            public EmailAddress EmailAddress { get; set; }

            [XmlElement(ElementName = "Phone")]
            public Phone Phone { get; set; }
        }

        [XmlRoot(ElementName = "ContactInfoList")]
        public class ContactInfoList
        {

            [XmlElement(ElementName = "ContactInfo")]
            public ContactInfo ContactInfo { get; set; }
        }

        [XmlRoot(ElementName = "DataLists")]
        public class DataLists
        {

            [XmlElement(ElementName = "PaxList")]
            public PaxList PaxList { get; set; }

            [XmlElement(ElementName = "ContactInfoList")]
            public ContactInfoList ContactInfoList { get; set; }

            [XmlElement(ElementName = "PaxJourneyList")]
            public Object? PaxJourneyList { get; set; }

            [XmlElement(ElementName = "PaxSegmentList")]
            public Object? PaxSegmentList { get; set; }
        }

        [XmlRoot(ElementName = "PaymentMethodCriteria")]
        public class PaymentMethodCriteria
        {

            [XmlElement(ElementName = "TypeCode")]
            public string? TypeCode { get; set; }
        }

        [XmlRoot(ElementName = "PaymentFunctions")]
        public class PaymentFunctions
        {

            [XmlElement(ElementName = "PaymentMethodCriteria")]
            public PaymentMethodCriteria PaymentMethodCriteria { get; set; }
        }

        [XmlRoot(ElementName = "Request")]
        public class Request
        {

            [XmlElement(ElementName = "Party")]
            public Party Party { get; set; }

            [XmlElement(ElementName = "CreateOrder")]
            public CreateOrder CreateOrder { get; set; }

            [XmlElement(ElementName = "DataLists")]
            public DataLists DataLists { get; set; }

            [XmlElement(ElementName = "PaymentFunctions")]
            public PaymentFunctions PaymentFunctions { get; set; }
        }

        [XmlRoot(ElementName = "IATA_OrderCreateRQ")]
        public class IATAOrderCreateRQ
        {

            [XmlElement(ElementName = "Request")]
            public Request Request { get; set; }
        }

    }

    public class BabylonFlightCreateOrderResponse
    {
        public class Amount
        {
            public string? Code { get; set; }
            public string? CurCode { get; set; }
            public string? text { get; set; }
        }

        public class Arrival
        {
            public string? IATALocationCode { get; set; }
            public string? AircraftScheduledDateTime { get; set; }
            public Terminal Terminal { get; set; }
        }

        public class BaseAmount
        {
            public string? CurCode { get; set; }
            public string? text { get; set; }
        }

        public class BookingEntity
        {
            public Org Org { get; set; }
            public Carrier Carrier { get; set; }
        }

        public class BookingRef
        {
            public BookingEntity BookingEntity { get; set; }
            public string? BookingID { get; set; }
        }

        public class OriginalIssueInfo
        {
            public string? IssueDate { get; set; }
            public string? IssuingAirlineDesigCode { get; set; }
        }

        public class Ticket
        {
            public Coupon Coupon { get; set; }
            public string? TicketNumber { get; set; }
        }

        public class Coupon
        {
            public string? CouponNumber { get; set; }
            public string? CouponStatusCode { get; set; }
        }

        public class CabinType
        {
            public string? CabinTypeCode { get; set; }
            public string? CabinTypeName { get; set; }
        }

        public class Carrier
        {
            public string? AirlineDesigCode { get; set; }
        }

        public class DataLists
        {
            public PaxList PaxList { get; set; }
            public PaxSegmentList PaxSegmentList { get; set; }
            public PaxJourneyList PaxJourneyList { get; set; }
            public OriginDestList OriginDestList { get; set; }
            public Object? BaggageAllowanceList { get; set; }
            public Object? ServiceDefinition { get; set; }
        }

        public class Dep
        {
            public string? IATALocationCode { get; set; }
            public string? AircraftScheduledDateTime { get; set; }
            public Terminal Terminal { get; set; }
        }

        public class Equipment
        {
            public string? AircraftCode { get; set; }
            public string? Name { get; set; }
        }

        public class FareComponent
        {
            public CabinType CabinType { get; set; }
            public Object? FareBasisCode { get; set; }
            public Object? FareTypeCode { get; set; }
            public Object? PriceClassRef { get; set; }
            public RBD RBD { get; set; }
            public string? PaxSegmentRefID { get; set; }
        }

        public class FareDetail
        {
            public FarePriceType FarePriceType { get; set; }
        }

        public class FarePriceType
        {
            public Object? FareComponent { get; set; }
            public Object? FarePriceTypeCode { get; set; }
            public Price Price { get; set; }
        }

        public class FlightDetail
        {
            public FlightDuration FlightDuration { get; set; }
        }

        public class FlightDuration
        {
            public string? Value { get; set; }
        }

        public class Individual
        {
            public string? Birthdate { get; set; }
            public string? GivenName { get; set; }
            public string? Surname { get; set; }
        }

        public class MarketingCarrierInfo
        {
            public string? CarrierDesigCode { get; set; }
            public string? MarketingCarrierFlightNumberText { get; set; }
        }

        public class OperatingCarrierInfo
        {
            public string? CarrierDesigCode { get; set; }
            public string? OperatingCarrierFlightNumberText { get; set; }
        }

        public class Order
        {
            public string? OrderID { get; set; }
            public TotalPrice TotalPrice { get; set; }
            public string? StatusCode { get; set; }
            public string? QueueStatus { get; set; }
            public Object? BookingRef { get; set; }
            public Object? OrderItem { get; set; }
        }

        public class TicketDocInfo
        {
            public Object? BookingRef { get; set; }
            public OriginalIssueInfo OriginalIssueInfo { get; set; }
            public string? PaxRefID { get; set; }
            public Object? Ticket { get; set; }
        }

        public class OrderItem
        {
            public string? OrderItemID { get; set; }
            public Price Price { get; set; }
            public Object? Service { get; set; }
            public FareDetail FareDetail { get; set; }
        }

        public class Org
        {
            public string? OrgID { get; set; }
            public string? Name { get; set; }
        }

        public class OriginDest
        {
            public string? DestCode { get; set; }
            public string? OriginCode { get; set; }
            public string? PaxJourneyRefID { get; set; }
            public string? OriginDestID { get; set; }
        }

        public class OriginDestList
        {
            public Object? OriginDest { get; set; }
        }

        public class Pax
        {
            public string? PaxID { get; set; }
            public string? PTC { get; set; }
            public Individual Individual { get; set; }
        }

        public class PaxJourney
        {
            public string? PaxJourneyID { get; set; }
            public Object? JourneyTime { get; set; }
            public string? OnPoint { get; set; }
            public string? OffPoint { get; set; }
            public Object? PaxSegmentRefID { get; set; }
        }

        public class PaxJourneyList
        {
            public Object? PaxJourney { get; set; }
        }

        public class PaxList
        {
            public Object? Pax { get; set; }
        }

        public class PaxSegment
        {
            public string? PaxSegmentID { get; set; }
            public Dep Dep { get; set; }
            public Arrival Arrival { get; set; }
            public MarketingCarrierInfo MarketingCarrierInfo { get; set; }
            public OperatingCarrierInfo OperatingCarrierInfo { get; set; }
            public Equipment Equipment { get; set; }
            public FlightDetail FlightDetail { get; set; }
        }

        public class PaxSegmentList
        {
            public Object? PaxSegment { get; set; }
        }

        public class Price
        {
            public BaseAmount BaseAmount { get; set; }
            public TaxSummary TaxSummary { get; set; }
            public TotalAmount TotalAmount { get; set; }
        }

        public class RBD
        {
            public string? RBD_Code { get; set; }
        }

        public class Response
        {
            public Order Order { get; set; }
            public Object? TicketDocInfo { get; set; }
            public DataLists DataLists { get; set; }
        }

        public class Service
        {
            public ServiceAssociations ServiceAssociations { get; set; }
            public Object? PaxRefID { get; set; }
            public string? ServiceID { get; set; }
            public string? StatusCode { get; set; }
        }

        public class ServiceAssociations
        {
            public Object? PaxSegmentRefID { get; set; }
        }

        public class Tax
        {
            public Amount Amount { get; set; }
            public string? TaxCode { get; set; }
            public Object? DescText { get; set; }
        }

        public class TaxSummary
        {
            public TotalTaxAmount TotalTaxAmount { get; set; }
            public Object? Tax { get; set; }
        }

        public class Terminal
        {
            public Object? Name { get; set; }
        }

        public class TotalAmount
        {
            public string? CurCode { get; set; }
            public string? text { get; set; }
        }

        public class TotalPrice
        {
            public TotalAmount TotalAmount { get; set; }
            public BaseAmount BaseAmount { get; set; }
            public TaxSummary TaxSummary { get; set; }
        }

        public class TotalTaxAmount
        {
            public string? CurCode { get; set; }
            public string? text { get; set; }
        }


        public class IATA_OrderViewRS
        {
            public Response Response { get; set; }
        }
        public class Babylon_AirCreateOrderRS
        {
            public IATA_OrderViewRS IATA_OrderViewRS { get; set; }
        }
    }

    public class BabylonFlightOrderChangeRequest
    {
        [XmlRoot(ElementName = "MessageDoc")]
        public class MessageDoc
        {

            [XmlElement(ElementName = "RefVersionNumber")]
            public double RefVersionNumber { get; set; }

            [XmlElement(ElementName = "Name")]
            public string? Name { get; set; }
        }

        [XmlRoot(ElementName = "TravelAgencySender")]
        public class TravelAgencySender
        {

            [XmlElement(ElementName = "PseudoCity")]
            public string? PseudoCity { get; set; }

            [XmlElement(ElementName = "AgencyID")]
            public string? AgencyID { get; set; }
        }

        [XmlRoot(ElementName = "Sender")]
        public class Sender
        {

            [XmlElement(ElementName = "TravelAgencySender")]
            public TravelAgencySender TravelAgencySender { get; set; }
        }

        [XmlRoot(ElementName = "Party")]
        public class Party
        {

            [XmlElement(ElementName = "Sender")]
            public Sender Sender { get; set; }
        }

        [XmlRoot(ElementName = "Order")]
        public class Order
        {

            [XmlElement(ElementName = "OrderID")]
            public string? OrderID { get; set; }

            [XmlElement(ElementName = "OwnerCode")]
            public string? OwnerCode { get; set; }
        }

        [XmlRoot(ElementName = "PaymentMethodCriteria")]
        public class PaymentMethodCriteria
        {

            [XmlElement(ElementName = "TypeCode")]
            public string? TypeCode { get; set; }
        }

        [XmlRoot(ElementName = "PaymentFunctions")]
        public class PaymentFunctions
        {

            [XmlElement(ElementName = "PaymentMethodCriteria")]
            public PaymentMethodCriteria PaymentMethodCriteria { get; set; }
        }

        [XmlRoot(ElementName = "Individual")]
        public class Individual
        {

            [XmlElement(ElementName = "Birthdate")]
            public string? Birthdate { get; set; }

            [XmlElement(ElementName = "NameTitle")]
            public string? NameTitle { get; set; }

            [XmlElement(ElementName = "GivenName")]
            public string? GivenName { get; set; }

            [XmlElement(ElementName = "Surname")]
            public string? Surname { get; set; }
        }

        [XmlRoot(ElementName = "Pax")]
        public class Pax
        {

            [XmlElement(ElementName = "PaxID")]
            public string? PaxID { get; set; }

            [XmlElement(ElementName = "PTC")]
            public string? PTC { get; set; }

            [XmlElement(ElementName = "Individual")]
            public Individual Individual { get; set; }
        }

        [XmlRoot(ElementName = "PaxList")]
        public class PaxList
        {

            [XmlElement(ElementName = "Pax")]
            public List<Pax> Pax { get; set; }
        }

        [XmlRoot(ElementName = "DataLists")]
        public class DataLists
        {

            [XmlElement(ElementName = "PaxList")]
            public PaxList PaxList { get; set; }
        }

        [XmlRoot(ElementName = "Request")]
        public class Request
        {

            [XmlElement(ElementName = "Order")]
            public Order Order { get; set; }

            [XmlElement(ElementName = "PaymentFunctions")]
            public PaymentFunctions PaymentFunctions { get; set; }

            [XmlElement(ElementName = "DataLists")]
            public DataLists DataLists { get; set; }
        }

        [XmlRoot(ElementName = "IATA_OrderChangeRQ")]
        public class IATAOrderChangeRQ
        {

            [XmlElement(ElementName = "MessageDoc")]
            public MessageDoc MessageDoc { get; set; }

            [XmlElement(ElementName = "Party")]
            public Party Party { get; set; }

            [XmlElement(ElementName = "Request")]
            public Request Request { get; set; }
        }
    }

    public class BabylonIATA_OrderRetrieveRequest
    {

        [XmlRoot(ElementName = "BookingRefFilterCriteria")]
        public class BookingRefFilterCriteria
        {

            [XmlElement(ElementName = "BookingEntity")]
            public Object? BookingEntity { get; set; }

            [XmlElement(ElementName = "BookingID")]
            public Object? BookingID { get; set; }
        }

        [XmlRoot(ElementName = "Order")]
        public class Order
        {

            [XmlElement(ElementName = "OrderID")]
            public string? OrderID { get; set; }

            [XmlElement(ElementName = "OwnerCode")]
            public Object? OwnerCode { get; set; }
        }

        [XmlRoot(ElementName = "OrderFilterCriteria")]
        public class OrderFilterCriteria
        {

            [XmlElement(ElementName = "BookingRefFilterCriteria")]
            public BookingRefFilterCriteria? BookingRefFilterCriteria { get; set; }

            [XmlElement(ElementName = "Order")]
            public Order? Order { get; set; }
        }

        [XmlRoot(ElementName = "Request")]
        public class Request
        {

            [XmlElement(ElementName = "OrderFilterCriteria")]
            public OrderFilterCriteria? OrderFilterCriteria { get; set; }
        }

        [XmlRoot(ElementName = "IATA_OrderRetrieveRQ")]
        public class IATAOrderRetrieveRQ
        {

            [XmlElement(ElementName = "Request")]
            public Request Request { get; set; }
        }
    }
}
