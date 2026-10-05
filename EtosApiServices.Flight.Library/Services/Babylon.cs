using EtosApiServices.Data.Library.Common;
using EtosApiServices.Flight.Library.Contract;
using EtosApiServices.Flight.Library.Utilities;
using EtosApiServices.Flight.Model;
using EtosApiServices.Flight.Model.ApiSetting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EtosApiServices.Flight.Library.Services
{
    public class Babylon : IBabylon
    {
        readonly BabylonCredential _babylonCredential;
        private readonly BabylonApiRequest _babylonApiRequest;
        private readonly IAirApiCommonFunction _iairApiCommonFunction;
        private readonly AirlineNamesProvider _airlineNamesProvider;
        private readonly AirPortNamesProvider _airPortNamesProvider;
        private readonly AirlineCitiesProvider _airlineCitiesProvider;
        public Babylon(IAirApiCommonFunction iairApiCommonFunction, BabylonApiRequest babylonApiRequest, IOptions<BabylonCredential> babylonCredential, AirlineNamesProvider airlineNamesProvider, AirPortNamesProvider airPortNamesProvider, AirlineCitiesProvider airlineCitiesProvider)
        {
            _iairApiCommonFunction = iairApiCommonFunction ?? throw new ArgumentException(nameof(iairApiCommonFunction));
            _babylonApiRequest = babylonApiRequest ?? throw new ArgumentException(nameof(babylonApiRequest));
            _babylonCredential = babylonCredential?.Value ?? throw new ArgumentException(nameof(babylonCredential));
            _airlineNamesProvider = airlineNamesProvider ?? throw new ArgumentException(nameof(airlineNamesProvider));
            _airPortNamesProvider = airPortNamesProvider ?? throw new ArgumentException(nameof(airPortNamesProvider));
            _airlineCitiesProvider = airlineCitiesProvider ?? throw new ArgumentException(nameof(airlineCitiesProvider));
        }

        public async Task<List<FlightSerachResponse.FlightShoppingResponse>> BabylonSearch(FlightSearchRequest ObjFlightSearchRequest, FlightSupplierCredential ObjBabylonCredential)
        {
            List<FlightSerachResponse.FlightShoppingResponse> ObjFlightLst = new List<FlightSerachResponse.FlightShoppingResponse>();
            BabylonFlightSearchResponse.Babylon_AirShoppingRS ObjFlight = new BabylonFlightSearchResponse.Babylon_AirShoppingRS();
            try
            {
                BabylonSetting? ObjBabylonSetting = string.IsNullOrWhiteSpace(ObjBabylonCredential.APICredential) ? null : JsonConvert.DeserializeObject<BabylonSetting>(ObjBabylonCredential.APICredential);
                if (ObjBabylonSetting != null && !string.IsNullOrEmpty(ObjBabylonSetting.AppKey))
                {
                    ObjBabylonSetting.ApiSupplierName = ObjBabylonCredential.APISupplierName;
                    ObjBabylonSetting.ApiSupplierId = ObjBabylonCredential.APISupplerId;
                    var ObjToken = await _babylonApiRequest.GetToken(ObjBabylonSetting);
                    if (ObjToken != null && ObjToken.TokenResponse != null && !string.IsNullOrEmpty(ObjToken.TokenResponse.Token))
                    {
                        ObjFlight = await _babylonApiRequest.GetFlight(ObjFlightSearchRequest, ObjBabylonSetting, ObjToken.TokenResponse.Token.ToString());
                    }
                    if (ObjFlight != null && ObjFlight.IATA_AirShoppingRS != null && ObjFlight.IATA_AirShoppingRS.Response != null && ObjFlight.IATA_AirShoppingRS.Response.OffersGroup != null)
                    {
                        ObjFlightLst = await GetBabylonFlightList(ObjFlight, ObjFlightSearchRequest, ObjBabylonSetting.ApiSupplierId, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return ObjFlightLst;
        }

        public async Task<List<FlightSerachResponse.FlightShoppingResponse>> GetBabylonFlightList(BabylonFlightSearchResponse.Babylon_AirShoppingRS ObjFlight, FlightSearchRequest ObjFlightSearchRequest, int ApiSupplierId, int ServiceId)
        {
            List<FlightSerachResponse.FlightShoppingResponse> ObjFlightLst = new List<FlightSerachResponse.FlightShoppingResponse>();
            try
            {
                if (ObjFlight != null && ObjFlight.IATA_AirShoppingRS != null && ObjFlight.IATA_AirShoppingRS.Response != null && ObjFlight.IATA_AirShoppingRS.Response.OffersGroup != null)
                {
                    decimal conversionrate = await _iairApiCommonFunction.GetCurrencyConversionRate(ObjFlightSearchRequest.Currency ?? "", ObjFlightSearchRequest.CompanyID, ObjFlightSearchRequest.DBConnectioString ?? "");
                    decimal BaseROE = await _iairApiCommonFunction.GetCurrencyConversionRate(ObjFlightSearchRequest.Currency ?? "", ObjFlightSearchRequest.CompanyID, ObjFlightSearchRequest.DBConnectioString ?? "");
                    decimal FlightBaseROE = 1;
                    int i = 0;
                    foreach (BabylonFlightSearchResponse.CarrierOffers FlightIndex in ObjFlight.IATA_AirShoppingRS.Response.OffersGroup.CarrierOffers ?? new List<BabylonFlightSearchResponse.CarrierOffers>())
                    {
                        int k = 0;
                        foreach (BabylonFlightSearchResponse.Offer FlightSearch in FlightIndex.Offer)
                        {
                            if (k == 0) { FlightBaseROE = await _iairApiCommonFunction.GetCurrencyConversionRate(FlightSearch.TotalPrice.BaseAmount.CurCode ?? "", ObjFlightSearchRequest.CompanyID, ObjFlightSearchRequest.DBConnectioString ?? ""); }

                            List<BabylonFlightSearchResponse.JourneyPriceClass>? JourneyPriceClass = new List<BabylonFlightSearchResponse.JourneyPriceClass>();
                            List<BabylonFlightSearchResponse.PaxJourney>? JourneyList = new List<BabylonFlightSearchResponse.PaxJourney>();
                            List<BabylonFlightSearchResponse.FareComponent>? FareComponentList = new List<BabylonFlightSearchResponse.FareComponent>();
                            List<BabylonFlightSearchResponse.OfferItem>? PriceOfferItem = new List<BabylonFlightSearchResponse.OfferItem>();
                            List<BabylonFlightSearchResponse.BaggageAllowance>? BaggageAllowanceLst = new List<BabylonFlightSearchResponse.BaggageAllowance>();

                            if (!(Convert.ToString(FlightSearch.JourneyOverview.JourneyPriceClass) ?? "").Contains("["))
                            {
                                var journeyPriceClassObj = JsonConvert.DeserializeObject<BabylonFlightSearchResponse.JourneyPriceClass>(Convert.ToString(FlightSearch.JourneyOverview.JourneyPriceClass) ?? "");
                                if (journeyPriceClassObj != null)
                                {
                                    JourneyPriceClass.Add(journeyPriceClassObj);
                                }
                            }
                            else { JourneyPriceClass = JsonConvert.DeserializeObject<List<BabylonFlightSearchResponse.JourneyPriceClass>>(Convert.ToString(FlightSearch.JourneyOverview.JourneyPriceClass) ?? ""); }

                            foreach (BabylonFlightSearchResponse.JourneyPriceClass JourneyPrice in JourneyPriceClass ?? new List<BabylonFlightSearchResponse.JourneyPriceClass>())
                            {
                                var paxJourney = ObjFlight.IATA_AirShoppingRS.Response.DataLists.PaxJourneyList.PaxJourney.AsEnumerable().Where(f => f.PaxJourneyID == JourneyPrice.PaxJourneyRefID).FirstOrDefault();
                                if (paxJourney != null)
                                {
                                    JourneyList.Add(paxJourney);
                                }
                            }

                            if (!(Convert.ToString(FlightSearch.OfferItem) ?? "").StartsWith("["))
                            {
                                var offerItemObj = JsonConvert.DeserializeObject<BabylonFlightSearchResponse.OfferItem>(Convert.ToString(FlightSearch.OfferItem) ?? "");
                                if (offerItemObj != null)
                                {
                                    PriceOfferItem.Add(offerItemObj);
                                }
                            }
                            else { PriceOfferItem = JsonConvert.DeserializeObject<List<BabylonFlightSearchResponse.OfferItem>>(Convert.ToString(FlightSearch.OfferItem) ?? ""); }

                            var FlightPaxSegment = new List<FlightSerachResponse.PaxSegment>();
                            foreach (BabylonFlightSearchResponse.OfferItem PriceItem in PriceOfferItem ?? new List<BabylonFlightSearchResponse.OfferItem>())
                            {
                                if (!(Convert.ToString(PriceItem.FareDetail.FareComponent) ?? "").StartsWith("["))
                                {
                                    var fareComponent = JsonConvert.DeserializeObject<BabylonFlightSearchResponse.FareComponent>(Convert.ToString(PriceItem.FareDetail.FareComponent) ?? "");
                                    if (fareComponent != null)
                                    {
                                        FareComponentList.Add(fareComponent);
                                    }
                                }
                                else { FareComponentList = JsonConvert.DeserializeObject<List<BabylonFlightSearchResponse.FareComponent>>(Convert.ToString(PriceItem.FareDetail.FareComponent) ?? ""); }
                                string stringValue = await _iairApiCommonFunction.GetPassengers(PriceItem.Service.PaxType ?? "");


                                List<string>? PaxRefID = new List<string>();
                                if (!(Convert.ToString(PriceItem.Service.PaxRefID) ?? "").Contains("[")) { PaxRefID.Add(Convert.ToString(PriceItem.Service.PaxRefID) ?? ""); }
                                else { PaxRefID = JsonConvert.DeserializeObject<List<string>>(Convert.ToString(PriceItem.Service.PaxRefID) ?? ""); }
                                int PassengerCount = PaxRefID != null ? PaxRefID.Count() : 0;
                                decimal basefareperseg = (Convert.ToDecimal(PriceItem.Price.BaseAmount.text) / FlightBaseROE) * conversionrate;
                                decimal basefareforpass = basefareperseg;
                                decimal taxforpass = Convert.ToDecimal(PriceItem.Price.TotalAmount.text) - Convert.ToDecimal(PriceItem.Price.BaseAmount.text);
                                taxforpass = (taxforpass / FlightBaseROE) * conversionrate;

                                decimal IPP = 0, Servicecharges = 0, basepricewithmarkup = 0;
                                basefareforpass = basefareforpass * PassengerCount;
                                taxforpass = taxforpass * PassengerCount;
                                IPP = IPP * PassengerCount;
                                Servicecharges = Servicecharges * PassengerCount;
                                basepricewithmarkup = basepricewithmarkup * PassengerCount;


                                var PaxSegment = new FlightSerachResponse.PaxSegment();
                                PaxSegment.PaxType = stringValue;
                                PaxSegment.PaxCount = PassengerCount;
                                PaxSegment.BasePrice = basefareforpass;
                                PaxSegment.Tax = taxforpass;
                                PaxSegment.IPP = IPP;
                                PaxSegment.ServiceCharges = Servicecharges;
                                PaxSegment.TotalPrice = basefareforpass + taxforpass + IPP + Servicecharges;
                                FlightPaxSegment.Add(PaxSegment);
                            }
                            decimal publishedprice = FlightPaxSegment.Sum(f => f.TotalPrice);

                            if (FlightSearch.BaggageAllowance != null)
                            {
                                var baggageAllowanceStr = Convert.ToString(FlightSearch.BaggageAllowance) ?? "";
                                if (!baggageAllowanceStr.StartsWith("["))
                                {
                                    var baggageAllowanceObj = JsonConvert.DeserializeObject<BabylonFlightSearchResponse.BaggageAllowance>(baggageAllowanceStr);
                                    if (baggageAllowanceObj != null)
                                    {
                                        BaggageAllowanceLst.Add(baggageAllowanceObj);
                                    }
                                }
                                else { BaggageAllowanceLst = JsonConvert.DeserializeObject<List<BabylonFlightSearchResponse.BaggageAllowance>>(baggageAllowanceStr); }
                            }

                            if (JourneyList != null && JourneyList.Count() > 0)
                            {
                                var Flights = new FlightSerachResponse.FlightShoppingResponse();
                                Flights.ServiceName = "Babylon";
                                Flights.ServiceId = ServiceId;
                                Flights.SupplierId = ApiSupplierId;
                                Flights.JourneyRefID = i.ToString() + "-" + k.ToString();
                                Flights.CabinClass = ObjFlightSearchRequest.Cabinclass;
                                Flights.Adult = ObjFlightSearchRequest.Adults;
                                Flights.Child = ObjFlightSearchRequest.Childs;
                                Flights.Infant = ObjFlightSearchRequest.Infants;
                                Flights.Currency = ObjFlightSearchRequest.Currency;
                                Flights.TotalPrice = publishedprice;
                                Flights.PaxSegments = FlightPaxSegment;
                                Flights.Flights = new List<FlightSerachResponse.Flights>();
                                for (int j = 0; j < JourneyList.Count(); j++)
                                {
                                    var FlightSegments = new List<BabylonFlightSearchResponse.PaxSegment>();
                                    List<string>? PaxSegmentRefID = new List<string>();
                                    List<string>? PriceClassRefID = new List<string>();

                                    if (!(Convert.ToString(JourneyList[j].PaxSegmentRefID) ?? "").Contains("[")) { PaxSegmentRefID.Add(Convert.ToString(JourneyList[j].PaxSegmentRefID) ?? ""); }
                                    else { PaxSegmentRefID = JsonConvert.DeserializeObject<List<string>>((Convert.ToString(JourneyList[j].PaxSegmentRefID) ?? "")); }

                                    if (PaxSegmentRefID != null && PaxSegmentRefID.Count > 0)
                                    {
                                        for (int s = 0; s < PaxSegmentRefID.Count(); s++)
                                        {
                                            if (ObjFlight.IATA_AirShoppingRS.Response.DataLists.PaxSegmentList.PaxSegment != null && ObjFlight.IATA_AirShoppingRS.Response.DataLists.PaxSegmentList.PaxSegment.Count > 0)
                                            {
                                                var paxSegment = ObjFlight.IATA_AirShoppingRS.Response.DataLists.PaxSegmentList.PaxSegment.AsEnumerable().Where(f => f.PaxSegmentID == PaxSegmentRefID[s]).FirstOrDefault();
                                                if (paxSegment != null)
                                                {
                                                    FlightSegments.Add(paxSegment);
                                                }
                                            }
                                        }
                                    }

                                    if (JourneyPriceClass != null && JourneyPriceClass[0].PriceClassRefID != null)
                                    {
                                        if (!(Convert.ToString(JourneyPriceClass[0].PriceClassRefID) ?? "").Contains("[")) { PriceClassRefID.Add(Convert.ToString(JourneyPriceClass[j].PriceClassRefID) ?? ""); }
                                        else { PriceClassRefID = JsonConvert.DeserializeObject<List<string>>(Convert.ToString(JourneyPriceClass[j].PriceClassRefID) ?? ""); }
                                    }

                                    int SegmentIndex = 0;
                                    var FlightJourny = new FlightSerachResponse.Flights();
                                    FlightJourny.FlightIndex = j;
                                    FlightJourny.FlightSegments = new List<FlightSerachResponse.FlightSegment>();
                                    foreach (BabylonFlightSearchResponse.PaxSegment flightdetails in FlightSegments)
                                    {
                                        var Segment = new FlightSerachResponse.FlightSegment();
                                        Segment.SegmentIndex = SegmentIndex;

                                        Segment.FlightNumber = Convert.ToString(flightdetails.MarketingCarrierInfo.MarketingCarrierFlightNumberText) ?? "";
                                        Segment.Carrier = Convert.ToString(flightdetails.MarketingCarrierInfo.CarrierDesigCode) ?? "";
                                        Segment.AirLineNames = _airlineNamesProvider.GetNameOrDefault(Segment.Carrier);
                                        Segment.OperatingCarrier = Convert.ToString(flightdetails.MarketingCarrierInfo.CarrierDesigCode) ?? "";
                                        Segment.DepartureDate = Convert.ToString(flightdetails.Dep.AircraftScheduledDateTime) ?? "";
                                        Segment.ArrivalDate = Convert.ToString(flightdetails.Arrival.AircraftScheduledDateTime) ?? "";
                                        Segment.DepartureTime = Convert.ToString(flightdetails.Dep.AircraftScheduledDateTime.ToString("HH:mm")) ?? "";
                                        Segment.ArrivalTime = Convert.ToString(flightdetails.Arrival.AircraftScheduledDateTime.ToString("HH:mm")) ?? "";
                                        Segment.OriginTerminal = Convert.ToString(flightdetails.Dep.TerminalName) ?? "";
                                        Segment.DestinationTerminal = Convert.ToString(flightdetails.Arrival.TerminalName) ?? "";
                                        Segment.AirportCodeOrigin = flightdetails.Dep.StationName;
                                        Segment.AirportCodeDestination = flightdetails.Arrival.StationName;

                                        TimeSpan ts = XmlConvert.ToTimeSpan(Convert.ToString(flightdetails.Duration) ?? "");
                                        Segment.SegmentDuration = (int)ts.TotalHours + ":" + ts.Minutes;

                                        Segment.AirportNameOrigin = _airPortNamesProvider.GetNameOrDefault(Segment.AirportCodeOrigin ?? "");
                                        Segment.AirportNameDestination = _airPortNamesProvider.GetNameOrDefault(Segment.AirportCodeDestination ?? "");

                                        Segment.AirLineCityOrigin = _airlineCitiesProvider.GetCityNameByAirportCodeOrDefault(Segment.AirportCodeOrigin ?? "");
                                        Segment.AirLineCityDestination = _airlineCitiesProvider.GetCityNameByAirportCodeOrDefault(Segment.AirportNameDestination ?? "");


                                        if (FareComponentList != null && FareComponentList.Count > 0)
                                        {
                                            BabylonFlightSearchResponse.FareComponent SegmentFareComponentList = FareComponentList.AsEnumerable().Where(f => (Convert.ToString(f.PaxSegmentRefID) ?? "") == (Convert.ToString(flightdetails.PaxSegmentID) ?? "")).FirstOrDefault();
                                            if (SegmentFareComponentList != null)
                                            {
                                                Segment.CabinClass = SegmentFareComponentList.CabinType.CabinTypeName ?? ObjFlightSearchRequest.Cabinclass;
                                                if (SegmentFareComponentList.RBD != null && !string.IsNullOrEmpty(SegmentFareComponentList.RBD.RBD_Code))
                                                {
                                                    Segment.ClassOfService = SegmentFareComponentList.RBD.RBD_Code.ToString();
                                                }
                                            }
                                        }

                                        if (BaggageAllowanceLst != null && BaggageAllowanceLst.Count() > 0)
                                        {
                                            if (ObjFlight.IATA_AirShoppingRS.Response.DataLists.BaggageAllowanceList != null && ObjFlight.IATA_AirShoppingRS.Response.DataLists.BaggageAllowanceList.BaggageAllowance != null && ObjFlight.IATA_AirShoppingRS.Response.DataLists.BaggageAllowanceList.BaggageAllowance.Count() > 0)
                                            {
                                                var BaggageAllownaceInfo = new List<BabylonFlightSearchResponse.BaggageAllowance>();
                                                if (j < BaggageAllowanceLst.Count())
                                                {
                                                    BaggageAllownaceInfo = ObjFlight.IATA_AirShoppingRS.Response.DataLists.BaggageAllowanceList.BaggageAllowance.Where(f => f.BaggageAllowanceID == BaggageAllowanceLst[j].BaggageAllowanceRefID).ToList();
                                                }
                                                else
                                                {
                                                    BaggageAllownaceInfo = ObjFlight.IATA_AirShoppingRS.Response.DataLists.BaggageAllowanceList.BaggageAllowance.Where(f => f.BaggageAllowanceID == BaggageAllowanceLst[0].BaggageAllowanceRefID).ToList();
                                                }
                                                if (BaggageAllownaceInfo != null && BaggageAllownaceInfo.Count() > 0)
                                                {
                                                    if (BaggageAllownaceInfo[0].PieceAllowance != null && !string.IsNullOrEmpty(BaggageAllownaceInfo[0].PieceAllowance.TotalQty)) { Segment.Baggage = BaggageAllownaceInfo[0].PieceAllowance.TotalQty + " Piece"; }
                                                    else if (BaggageAllownaceInfo[0].WeightAllowance != null && BaggageAllownaceInfo[0].WeightAllowance.MaximumWeightMeasure != null && !string.IsNullOrEmpty(BaggageAllownaceInfo[0].WeightAllowance.MaximumWeightMeasure.text)) { Segment.Baggage = BaggageAllownaceInfo[0].WeightAllowance.MaximumWeightMeasure.text + " " + BaggageAllownaceInfo[0].WeightAllowance.MaximumWeightMeasure.UnitCode; }
                                                }
                                            }
                                        }
                                        FlightJourny.FlightSegments.Add(Segment);
                                        SegmentIndex++;
                                    }

                                    FlightJourny.DepartureTime = FlightJourny.FlightSegments[0].DepartureDate;
                                    FlightJourny.ArriveTime = FlightJourny.FlightSegments.Last().ArrivalDate;

                                    TimeSpan jts = XmlConvert.ToTimeSpan(Convert.ToString(JourneyList[j].JourneyTime) ?? "");
                                    FlightJourny.Duration = (int)jts.TotalHours + ":" + jts.Minutes;
                                    FlightJourny.Stops = (FlightJourny.FlightSegments.Count > 1) ? (FlightJourny.FlightSegments.Count - 1).ToString() + " Stop" : "Non-Stop";
                                    FlightJourny.Airline = FlightJourny.FlightSegments[0].AirLineNames;
                                    FlightJourny.FlightNumber = FlightJourny.FlightSegments[0].FlightNumber;
                                    FlightJourny.AirlineCode = FlightJourny.FlightSegments[0].Carrier;
                                    FlightJourny.CabinClass = FlightJourny.FlightSegments[0].CabinClass;
                                    FlightJourny.ClassOfService = FlightJourny.FlightSegments[0].ClassOfService;
                                    FlightJourny.AirportCodeOrigin = FlightJourny.FlightSegments[0].AirportCodeOrigin;
                                    FlightJourny.AirportCodeDestination = FlightJourny.FlightSegments[0].AirportCodeDestination;
                                    Flights.Flights.Add(FlightJourny);
                                }
                                Flights.DepartureTime = Flights.Flights[0].DepartureTime;
                                Flights.ArriveTime = Flights.Flights.Last().DepartureTime;
                                Flights.AirlineCode = string.Join(",", Flights.Flights.Select(h => h.AirlineCode).Distinct().ToList());
                                Flights.IsRefunddable = 0;
                                ObjFlightLst.Add(Flights);
                            }
                            k++;
                        }
                        i++;
                    }

                }
            }
            catch (Exception ex)
            {
                await _iairApiCommonFunction.InsertLogError(ex, ObjFlightSearchRequest.AgentID, ObjFlightSearchRequest.DBConnectioString ?? "");
            }
            return ObjFlightLst;
        }
    }
}
