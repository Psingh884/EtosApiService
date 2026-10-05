using EtosApiServices.Data.Library.Common;
using EtosApiServices.Flight.Model;
using EtosApiServices.Flight.Model.ApiSetting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EtosApiServices.Flight.Library
{
    public class BabylonApiRequest
    {
        private readonly IAirApiCommonFunction _iairApiCommonFunction;
        readonly BabylonCredential _babylonCredential;
        public BabylonApiRequest(IAirApiCommonFunction iairApiCommonFunction, IOptions<BabylonCredential> babylonCredential)
        {
            _iairApiCommonFunction = iairApiCommonFunction ?? throw new ArgumentException(nameof(iairApiCommonFunction));
            _babylonCredential = babylonCredential?.Value ?? throw new ArgumentException(nameof(babylonCredential));
        }

        public async Task<BabylonTokenResponse.BabylonToken> GetToken(BabylonSetting ObjBabylonSetting)
        {
            BabylonTokenResponse.BabylonToken ObjTokenResponse = new BabylonTokenResponse.BabylonToken();
            try
            {
                BabylonTokenRequest.TokenRequest ObjTokenRequest = new BabylonTokenRequest.TokenRequest();
                ObjTokenRequest.SecretKey = ObjBabylonSetting.SecretKey;
                ObjTokenRequest.APIKey = ObjBabylonSetting.AppKey;
                string XmlReq = jsonFunction.CreateXMLByClass(ObjTokenRequest);
                string XmlResponse = await GetBabylonResponsesXml(XmlReq, _babylonCredential.Token ?? "", string.Empty);
                if (!string.IsNullOrEmpty(XmlResponse))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(XmlResponse);
                    string json = JsonConvert.SerializeXmlNode(doc);
                    ObjTokenResponse = JsonConvert.DeserializeObject<BabylonTokenResponse.BabylonToken>(json);
                }
            }
            catch (Exception ex)
            {
            }
            return ObjTokenResponse ?? null;
        }

        public async Task<BabylonFlightSearchResponse.Babylon_AirShoppingRS> GetFlight(FlightSearchRequest ObjFlightSearchRequest, BabylonSetting ObjBabylonSetting, string Token)
        {
            BabylonFlightSearchResponse.Babylon_AirShoppingRS ObjFlight = new BabylonFlightSearchResponse.Babylon_AirShoppingRS();
            try
            {
                BabylonFlightSearchRequest.IATAAirShoppingRQ ObjSearchFlight = new BabylonFlightSearchRequest.IATAAirShoppingRQ();
                ObjSearchFlight.Request = new BabylonFlightSearchRequest.Request();
                ObjSearchFlight.Request.FlightCriteria = new BabylonFlightSearchRequest.FlightCriteria();
                ObjSearchFlight.Request.FlightCriteria.OriginDestCriteria = new BabylonFlightSearchRequest.OriginDestCriteria[ObjFlightSearchRequest.JourneyTypeId];
                ObjSearchFlight.Request.FlightCriteria.OriginDestCriteria[0] = new BabylonFlightSearchRequest.OriginDestCriteria();
                ObjSearchFlight.Request.FlightCriteria.OriginDestCriteria[0].OriginDepCriteria = new BabylonFlightSearchRequest.OriginDepCriteria();
                ObjSearchFlight.Request.FlightCriteria.OriginDestCriteria[0].OriginDepCriteria.IATALocationCode = ObjFlightSearchRequest.DepartureCity;
                ObjSearchFlight.Request.FlightCriteria.OriginDestCriteria[0].OriginDepCriteria.Date = DateTime.ParseExact(ObjFlightSearchRequest.DepartureDate ?? "", "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                ObjSearchFlight.Request.FlightCriteria.OriginDestCriteria[0].DestArrivalCriteria = new BabylonFlightSearchRequest.DestArrivalCriteria();
                ObjSearchFlight.Request.FlightCriteria.OriginDestCriteria[0].DestArrivalCriteria.IATALocationCode = ObjFlightSearchRequest.DestinationCity;
                if (ObjFlightSearchRequest.JourneyTypeId == 2)
                {
                    ObjSearchFlight.Request.FlightCriteria.OriginDestCriteria[1] = new BabylonFlightSearchRequest.OriginDestCriteria();
                    ObjSearchFlight.Request.FlightCriteria.OriginDestCriteria[1].OriginDepCriteria = new BabylonFlightSearchRequest.OriginDepCriteria();
                    ObjSearchFlight.Request.FlightCriteria.OriginDestCriteria[1].OriginDepCriteria.IATALocationCode = ObjFlightSearchRequest.DestinationCity;
                    ObjSearchFlight.Request.FlightCriteria.OriginDestCriteria[1].OriginDepCriteria.Date = DateTime.ParseExact(ObjFlightSearchRequest.DestinationDate ?? "", "dd-MM-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    ObjSearchFlight.Request.FlightCriteria.OriginDestCriteria[1].DestArrivalCriteria = new BabylonFlightSearchRequest.DestArrivalCriteria();
                    ObjSearchFlight.Request.FlightCriteria.OriginDestCriteria[1].DestArrivalCriteria.IATALocationCode = ObjFlightSearchRequest.DepartureCity;
                }

                int TotalPaxCount = ObjFlightSearchRequest.Adults + ObjFlightSearchRequest.Childs + ObjFlightSearchRequest.Infants;
                int PaxCount = 0;
                ObjSearchFlight.Request.Paxs = new BabylonFlightSearchRequest.Paxs();
                ObjSearchFlight.Request.Paxs.Pax = new BabylonFlightSearchRequest.Pax[TotalPaxCount];
                for (int i = 0; i < ObjFlightSearchRequest.Adults; i++)
                {
                    ObjSearchFlight.Request.Paxs.Pax[PaxCount] = new BabylonFlightSearchRequest.Pax();
                    ObjSearchFlight.Request.Paxs.Pax[PaxCount].PaxID = "SHA" + (PaxCount + 1);
                    ObjSearchFlight.Request.Paxs.Pax[PaxCount].PTC = "ADT";
                    PaxCount++;
                }
                for (int i = 0; i < ObjFlightSearchRequest.Childs; i++)
                {
                    ObjSearchFlight.Request.Paxs.Pax[PaxCount] = new BabylonFlightSearchRequest.Pax();
                    ObjSearchFlight.Request.Paxs.Pax[PaxCount].PaxID = "SHA" + (PaxCount + 1);
                    ObjSearchFlight.Request.Paxs.Pax[PaxCount].PTC = "CHD";
                    PaxCount++;
                }
                for (int i = 0; i < ObjFlightSearchRequest.Infants; i++)
                {
                    ObjSearchFlight.Request.Paxs.Pax[PaxCount] = new BabylonFlightSearchRequest.Pax();
                    ObjSearchFlight.Request.Paxs.Pax[PaxCount].PaxID = "SHA" + (PaxCount + 1);
                    ObjSearchFlight.Request.Paxs.Pax[PaxCount].PTC = "INF";
                    PaxCount++;
                }
                ObjSearchFlight.Request.ShoppingCriteria = new BabylonFlightSearchRequest.ShoppingCriteria();
                ObjSearchFlight.Request.ShoppingCriteria.CabinTypeCriteria = new BabylonFlightSearchRequest.CabinTypeCriteria();
                ObjSearchFlight.Request.ShoppingCriteria.CabinTypeCriteria.CabinTypeCode = (ObjFlightSearchRequest.Cabinclass == BabylonCabinType.Economy.ToString() ? (int)BabylonCabinType.Economy : ObjFlightSearchRequest.Cabinclass == BabylonCabinType.First.ToString() ? (int)BabylonCabinType.First : ObjFlightSearchRequest.Cabinclass == BabylonCabinType.Business.ToString() ? (int)BabylonCabinType.Business : ObjFlightSearchRequest.Cabinclass == BabylonCabinType.PremiumEconomy.ToString() ? (int)BabylonCabinType.PremiumEconomy : (int)BabylonCabinType.Economy);//searchs.Cabinclass;

                ObjSearchFlight.Party = new BabylonFlightSearchRequest.Party();
                ObjSearchFlight.Party.Sender = new BabylonFlightSearchRequest.Sender();
                ObjSearchFlight.Party.Sender.TravelAgency = new BabylonFlightSearchRequest.TravelAgency();
                ObjSearchFlight.Party.Sender.TravelAgency.AgencyID = ObjBabylonSetting.AgentCode;

                string XmlReq = jsonFunction.CreateXMLByClass(ObjSearchFlight);
                await _iairApiCommonFunction.Flights_InsertAirSearchRequest(XmlReq, ObjFlightSearchRequest.XmlDataId + "_BLN", Token, ObjFlightSearchRequest.DBConnectioString ?? "");
                await _iairApiCommonFunction.Insert_Tbl_FlightRequestResponse("AirShopping", "Request", XmlReq, ObjFlightSearchRequest.XmlDataId ?? "", ObjBabylonSetting.ApiSupplierId.ToString(), ObjFlightSearchRequest.DBConnectioString ?? "");

                string XmlResponse = await GetBabylonResponsesXml(XmlReq, _babylonCredential.FlightSearch ?? "", Token);
                await _iairApiCommonFunction.Insert_Tbl_FlightRequestResponse("AirShopping", "Response", XmlResponse, ObjFlightSearchRequest.XmlDataId ?? "", ObjBabylonSetting.ApiSupplierId.ToString(), ObjFlightSearchRequest.DBConnectioString ?? "");
                if (!string.IsNullOrEmpty(XmlResponse))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(XmlResponse);
                    string json = JsonConvert.SerializeXmlNode(doc);
                    ObjFlight = JsonConvert.DeserializeObject<BabylonFlightSearchResponse.Babylon_AirShoppingRS>(json.Replace("@CurCode", "CurCode").Replace("#text", "text"));
                }
            }
            catch (Exception ex)
            {
                await _iairApiCommonFunction.InsertLogError(ex, 1, ObjFlightSearchRequest.DBConnectioString ?? "");
            }
            return ObjFlight ?? null;
        }

        //public async Task<BabylonFlightFareRulesResponse.IATAOrderRulesRS> GetFlightFareRules(BabylonFlightSearchResponse.Babylon_AirShoppingRS ObjAirShoppingRS, BabylonSetting ObjBabylonSetting, string XmlDataid)
        //{
        //    BabylonFlightFareRulesResponse.IATAOrderRulesRS ObjIATAOrderRulesRS = new BabylonFlightFareRulesResponse.IATAOrderRulesRS();
        //    try
        //    {
        //        DataTable DtFlight = ExecuteQuery.LoadDataTableWithQuery("Select Top 1 * From AirSearchRequest Where XmlDataid = '" + XmlDataid + "_BLN' order by 1 desc");
        //        if (DtFlight != null && DtFlight.Rows.Count > 0)
        //        {
        //            if (!string.IsNullOrEmpty(DtFlight.Rows[0]["Token"].ToString()))
        //            {
        //                string Token = DtFlight.Rows[0]["Token"].ToString();
        //                List<BabylonFlightSearchResponse.OfferItem> OfferItemLst = new List<BabylonFlightSearchResponse.OfferItem>();
        //                if (!ObjAirShoppingRS.IATA_AirShoppingRS.Response.OffersGroup.CarrierOffers[0].Offer[0].OfferItem.ToString().StartsWith("[")) { OfferItemLst.Add(JsonConvert.DeserializeObject<BabylonFlightSearchResponse.OfferItem>(ObjAirShoppingRS.IATA_AirShoppingRS.Response.OffersGroup.CarrierOffers[0].Offer[0].OfferItem.ToString())); }
        //                else { OfferItemLst = JsonConvert.DeserializeObject<List<BabylonFlightSearchResponse.OfferItem>>(ObjAirShoppingRS.IATA_AirShoppingRS.Response.OffersGroup.CarrierOffers[0].Offer[0].OfferItem.ToString()); }

        //                var FareComponentList = new List<BabylonFlightSearchResponse.FareComponent>();
        //                if (!OfferItemLst[0].FareDetail.FareComponent.ToString().StartsWith("[")) { FareComponentList.Add(JsonConvert.DeserializeObject<BabylonFlightSearchResponse.FareComponent>(OfferItemLst[0].FareDetail.FareComponent.ToString())); }
        //                else { FareComponentList = JsonConvert.DeserializeObject<List<BabylonFlightSearchResponse.FareComponent>>(OfferItemLst[0].FareDetail.FareComponent.ToString()); }


        //                BabylonFlightFareRulesRequest.IATAAirShoppingRequest ObjAirShoppingRequest = new BabylonFlightFareRulesRequest.IATAAirShoppingRequest();
        //                ObjAirShoppingRequest.IATA_OrderRulesRQ = new BabylonFlightFareRulesRequest.IATA_OrderRulesRQ();
        //                ObjAirShoppingRequest.IATA_OrderRulesRQ.Request = new BabylonFlightFareRulesRequest.Request();
        //                ObjAirShoppingRequest.IATA_OrderRulesRQ.Request.CoreRequest = new BabylonFlightFareRulesRequest.CoreRequest();
        //                ObjAirShoppingRequest.IATA_OrderRulesRQ.Request.CoreRequest.FareRef = new BabylonFlightFareRulesRequest.FareRef();
        //                ObjAirShoppingRequest.IATA_OrderRulesRQ.Request.CoreRequest.FareRef.AirlineDesigCode = ObjAirShoppingRS.IATA_AirShoppingRS.Response.OffersGroup.CarrierOffers[0].Offer[0].ValidatingCarrierCode;
        //                ObjAirShoppingRequest.IATA_OrderRulesRQ.Request.CoreRequest.FareRef.FareRefText = OfferItemLst[0].FareDetail.FareRefText;
        //                ObjAirShoppingRequest.IATA_OrderRulesRQ.Request.CoreRequest.FareRef.FareTypeCode = FareComponentList[0].FareTypeCode;
        //                ObjAirShoppingRequest.IATA_OrderRulesRQ.Request.CoreRequest.FareRef.FareBasisCode = FareComponentList[0].FareBasisCode;
        //                ObjAirShoppingRequest.IATA_OrderRulesRQ.Request.CoreRequest.FareRef.Arrival = new BabylonFlightFareRulesRequest.Arrival();
        //                ObjAirShoppingRequest.IATA_OrderRulesRQ.Request.CoreRequest.FareRef.Arrival.AircraftScheduledDateTime = ObjAirShoppingRS.IATA_AirShoppingRS.Response.DataLists.PaxSegmentList.PaxSegment[0].Arrival.AircraftScheduledDateTime.ToString("yyyy-MM-dd");
        //                ObjAirShoppingRequest.IATA_OrderRulesRQ.Request.CoreRequest.FareRef.Arrival.IATALocationCode = ObjAirShoppingRS.IATA_AirShoppingRS.Response.DataLists.PaxSegmentList.PaxSegment[0].Arrival.IATALocationCode;
        //                ObjAirShoppingRequest.IATA_OrderRulesRQ.Request.CoreRequest.FareRef.Dep = new BabylonFlightFareRulesRequest.Dep();
        //                ObjAirShoppingRequest.IATA_OrderRulesRQ.Request.CoreRequest.FareRef.Dep.AircraftScheduledDateTime = ObjAirShoppingRS.IATA_AirShoppingRS.Response.DataLists.PaxSegmentList.PaxSegment[0].Dep.AircraftScheduledDateTime.ToString("yyyy-MM-dd");
        //                ObjAirShoppingRequest.IATA_OrderRulesRQ.Request.CoreRequest.FareRef.Dep.IATALocationCode = ObjAirShoppingRS.IATA_AirShoppingRS.Response.DataLists.PaxSegmentList.PaxSegment[0].Dep.IATALocationCode;

        //                ObjAirShoppingRequest.IATA_OrderRulesRQ.Party = new BabylonFlightFareRulesRequest.Party();
        //                ObjAirShoppingRequest.IATA_OrderRulesRQ.Party.Sender = new BabylonFlightFareRulesRequest.Sender();
        //                ObjAirShoppingRequest.IATA_OrderRulesRQ.Party.Sender.TravelAgency = new BabylonFlightFareRulesRequest.TravelAgency();
        //                ObjAirShoppingRequest.IATA_OrderRulesRQ.Party.Sender.TravelAgency.AgencyID = ObjBabylonSetting.AgentCode;


        //                string XmlReq = jsonFunction.CreateXMLByClass(ObjAirShoppingRequest.IATA_OrderRulesRQ);
        //                await _iairApiCommonFunction.Insert_Tbl_FlightRequestResponse("FareRule", "Request", XmlReq, XmlDataid, ObjBabylonSetting.ApiSupplierId.ToString());
        //                string XmlResponse = await GetBabylonResponsesXml(XmlReq, _babylonCredential.FareRules ?? "", Token);
        //                await _iairApiCommonFunction.Insert_Tbl_FlightRequestResponse("FareRule", "Response", XmlResponse, XmlDataid, ObjBabylonSetting.ApiSupplierId.ToString());
        //                if (!string.IsNullOrEmpty(XmlResponse))
        //                {
        //                    XmlDocument doc = new XmlDocument();
        //                    doc.LoadXml(XmlResponse);
        //                    string json = JsonConvert.SerializeXmlNode(doc);
        //                    ObjIATAOrderRulesRS = JsonConvert.DeserializeObject<BabylonFlightFareRulesResponse.IATAOrderRulesRS>(json.Replace("@CurCode", "CurCode").Replace("#text", "text"));
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Errorlog.InsertLogError(ex, 1);
        //    }
        //    return ObjIATAOrderRulesRS;
        //}

        //public async Task<BabylonFlightOfferFareResponse.IATAOfferPriceRS> GetFlightOferfare(BabylonFlightSearchResponse.Babylon_AirShoppingRS ObjAirShoppingRS, BabylonSetting ObjBabylonSetting, string XmlDataid, string Token)
        //{
        //    BabylonFlightOfferFareResponse.IATAOfferPriceRS ObjIATAOfferPriceRS = new BabylonFlightOfferFareResponse.IATAOfferPriceRS();
        //    try
        //    {
        //        BabylonFlightOfferFareRequest.IATAOfferPriceRQ ObjIATAOfferPriceRQ = new BabylonFlightOfferFareRequest.IATAOfferPriceRQ();
        //        ObjIATAOfferPriceRQ.Request = new BabylonFlightOfferFareRequest.Request();
        //        ObjIATAOfferPriceRQ.Request.PricedOffer = new BabylonFlightOfferFareRequest.PricedOffer();
        //        ObjIATAOfferPriceRQ.Request.PricedOffer.SelectedOffer = new BabylonFlightOfferFareRequest.SelectedOffer();
        //        ObjIATAOfferPriceRQ.Request.PricedOffer.SelectedOffer.OfferRefID = ObjAirShoppingRS.IATA_AirShoppingRS.Response.OffersGroup.CarrierOffers[0].Offer[0].OfferID;
        //        ObjIATAOfferPriceRQ.Request.PricedOffer.SelectedOffer.OwnerCode = ObjAirShoppingRS.IATA_AirShoppingRS.Response.OffersGroup.CarrierOffers[0].Offer[0].OwnerCode;
        //        ObjIATAOfferPriceRQ.Request.PricedOffer.SelectedOffer.ShoppingResponseRefID = ObjAirShoppingRS.IATA_AirShoppingRS.Response.ShoppingResponse.ShoppingResponseID;
        //        ObjIATAOfferPriceRQ.Request.PricedOffer.SelectedOffer.SelectedOfferItem = new List<BabylonFlightOfferFareRequest.SelectedOfferItem>();

        //        var OfferItem = new List<BabylonFlightSearchResponse.OfferItem>();
        //        if (!ObjAirShoppingRS.IATA_AirShoppingRS.Response.OffersGroup.CarrierOffers[0].Offer[0].OfferItem.ToString().StartsWith("[")) { OfferItem.Add(JsonConvert.DeserializeObject<BabylonFlightSearchResponse.OfferItem>(ObjAirShoppingRS.IATA_AirShoppingRS.Response.OffersGroup.CarrierOffers[0].Offer[0].OfferItem.ToString())); }
        //        else { OfferItem = JsonConvert.DeserializeObject<List<BabylonFlightSearchResponse.OfferItem>>(ObjAirShoppingRS.IATA_AirShoppingRS.Response.OffersGroup.CarrierOffers[0].Offer[0].OfferItem.ToString()); }

        //        foreach (BabylonFlightSearchResponse.OfferItem OfferItems in OfferItem)
        //        {
        //            var ObjOfferItem = new BabylonFlightOfferFareRequest.SelectedOfferItem();
        //            ObjOfferItem.OfferItemRefID = OfferItems.OfferItemID;

        //            var PaxRefID = new List<string>();
        //            if (!OfferItems.Service.PaxRefID.ToString().StartsWith("[")) { PaxRefID.Add(OfferItems.Service.PaxRefID.ToString()); }
        //            else { PaxRefID = JsonConvert.DeserializeObject<List<string>>(OfferItems.Service.PaxRefID.ToString()); }
        //            ObjOfferItem.PaxRefID = new List<string>();
        //            ObjOfferItem.PaxRefID = PaxRefID;
        //            ObjIATAOfferPriceRQ.Request.PricedOffer.SelectedOffer.SelectedOfferItem.Add(ObjOfferItem);
        //        }
        //        ObjIATAOfferPriceRQ.Request.DataLists = new BabylonFlightOfferFareRequest.DataLists();
        //        ObjIATAOfferPriceRQ.Request.DataLists.PaxList = new BabylonFlightOfferFareRequest.PaxList();

        //        var Paxlist = new List<BabylonFlightOfferFareRequest.Pax>();
        //        if (!ObjAirShoppingRS.IATA_AirShoppingRS.Response.DataLists.PaxList.Pax.ToString().StartsWith("[")) { Paxlist.Add(JsonConvert.DeserializeObject<BabylonFlightOfferFareRequest.Pax>(ObjAirShoppingRS.IATA_AirShoppingRS.Response.DataLists.PaxList.Pax.ToString())); }
        //        else { Paxlist = JsonConvert.DeserializeObject<List<BabylonFlightOfferFareRequest.Pax>>(ObjAirShoppingRS.IATA_AirShoppingRS.Response.DataLists.PaxList.Pax.ToString()); }

        //        ObjIATAOfferPriceRQ.Request.DataLists.PaxList.Pax = new List<BabylonFlightOfferFareRequest.Pax>();
        //        ObjIATAOfferPriceRQ.Request.DataLists.PaxList.Pax = Paxlist;

        //        string XmlReq = jsonFunction.CreateXMLByClass(ObjIATAOfferPriceRQ);
        //        await _iairApiCommonFunction.Insert_Tbl_FlightRequestResponse("OfferPrice", "Request", XmlReq, XmlDataid, ObjBabylonSetting.ApiSupplierId.ToString());

        //        string XmlResponse = await GetBabylonResponsesXml(XmlReq, _babylonCredential.OfferPrice ?? "", Token);
        //        await _iairApiCommonFunction.Insert_Tbl_FlightRequestResponse("OfferPrice", "Response", XmlResponse, XmlDataid, ObjBabylonSetting.ApiSupplierId.ToString());
        //        if (!string.IsNullOrEmpty(XmlResponse))
        //        {
        //            XmlDocument doc = new XmlDocument();
        //            doc.LoadXml(XmlResponse);
        //            string json = JsonConvert.SerializeXmlNode(doc);
        //            ObjIATAOfferPriceRS = JsonConvert.DeserializeObject<BabylonFlightOfferFareResponse.IATAOfferPriceRS>(json.Replace("@CurCode", "CurCode").Replace("#text", "text"));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Errorlog.InsertLogError(ex, 1);
        //    }
        //    return ObjIATAOfferPriceRS;
        //}

        //public async Task<BabylonFlightCreateOrderResponse.Babylon_AirCreateOrderRS> CreateOrder(BabylonFlightOfferFareResponse.IATAOfferPriceRS ObjIATAOfferPriceRS, BabylonSetting ObjBabylonSetting, PaxDetails[] adultdata, PaxDetails[] childdata, PaxDetails[] infant, string XmlDataid, string Token)
        //{
        //    BabylonFlightCreateOrderResponse.Babylon_AirCreateOrderRS ObjAirCreateOrderRS = new BabylonFlightCreateOrderResponse.Babylon_AirCreateOrderRS();
        //    try
        //    {
        //        BabylonFlightCreateOrderRequest.IATAOrderCreateRQ ObjIATAOrderCreateRQ = new BabylonFlightCreateOrderRequest.IATAOrderCreateRQ();
        //        ObjIATAOrderCreateRQ.Request = new BabylonFlightCreateOrderRequest.Request();
        //        ObjIATAOrderCreateRQ.Request.Party = new BabylonFlightCreateOrderRequest.Party();
        //        ObjIATAOrderCreateRQ.Request.Party.Sender = new BabylonFlightCreateOrderRequest.Sender();
        //        ObjIATAOrderCreateRQ.Request.Party.Sender.TravelAgencySender = new BabylonFlightCreateOrderRequest.TravelAgencySender();
        //        ObjIATAOrderCreateRQ.Request.Party.Sender.TravelAgencySender.AgencyID = ObjBabylonSetting.AgentCode;

        //        ObjIATAOrderCreateRQ.Request.CreateOrder = new BabylonFlightCreateOrderRequest.CreateOrder();
        //        ObjIATAOrderCreateRQ.Request.CreateOrder.SelectedOffer = new BabylonFlightCreateOrderRequest.SelectedOffer();
        //        ObjIATAOrderCreateRQ.Request.CreateOrder.SelectedOffer.OfferRefID = ObjIATAOfferPriceRS.IATA_OfferPriceRS.Response.PricedOffer.OfferID;
        //        ObjIATAOrderCreateRQ.Request.CreateOrder.SelectedOffer.OwnerCode = ObjIATAOfferPriceRS.IATA_OfferPriceRS.Response.PricedOffer.OwnerCode;
        //        ObjIATAOrderCreateRQ.Request.CreateOrder.SelectedOffer.ShoppingResponseRefID = ObjIATAOfferPriceRS.IATA_OfferPriceRS.Response.ShoppingResponse.ShoppingResponseID;
        //        ObjIATAOrderCreateRQ.Request.CreateOrder.SelectedOffer.TotalOfferPriceAmount = Convert.ToDecimal(ObjIATAOfferPriceRS.IATA_OfferPriceRS.Response.PricedOffer.TotalPrice.TotalAmount.text);
        //        ObjIATAOrderCreateRQ.Request.CreateOrder.SelectedOffer.Currency = ObjIATAOfferPriceRS.IATA_OfferPriceRS.Response.PricedOffer.TotalPrice.TotalAmount.CurCode;

        //        ObjIATAOrderCreateRQ.Request.CreateOrder.SelectedOffer.SelectedOfferItem = new List<BabylonFlightCreateOrderRequest.SelectedOfferItem>();

        //        var PriceOfferItem = new List<BabylonFlightOfferFareResponse.OfferItem>();
        //        if (!ObjIATAOfferPriceRS.IATA_OfferPriceRS.Response.PricedOffer.OfferItem.ToString().StartsWith("[")) { PriceOfferItem.Add(JsonConvert.DeserializeObject<BabylonFlightOfferFareResponse.OfferItem>(ObjIATAOfferPriceRS.IATA_OfferPriceRS.Response.PricedOffer.OfferItem.ToString())); }
        //        else { PriceOfferItem = JsonConvert.DeserializeObject<List<BabylonFlightOfferFareResponse.OfferItem>>(ObjIATAOfferPriceRS.IATA_OfferPriceRS.Response.PricedOffer.OfferItem.ToString()); }
        //        foreach (BabylonFlightOfferFareResponse.OfferItem PriceItem in PriceOfferItem)
        //        {
        //            var SelectedPaxOffers = new BabylonFlightCreateOrderRequest.SelectedOfferItem();
        //            SelectedPaxOffers.OfferItemRefID = PriceItem.OfferItemID;
        //            SelectedPaxOffers.PaxRefID = new List<string>();
        //            if (!PriceItem.Service.PaxRefID.ToString().StartsWith("[")) { SelectedPaxOffers.PaxRefID.Add(PriceItem.Service.PaxRefID.ToString()); }
        //            else { SelectedPaxOffers.PaxRefID = JsonConvert.DeserializeObject<List<string>>(PriceItem.Service.PaxRefID.ToString()); }
        //            ObjIATAOrderCreateRQ.Request.CreateOrder.SelectedOffer.SelectedOfferItem.Add(SelectedPaxOffers);
        //        }

        //        ObjIATAOrderCreateRQ.Request.DataLists = new BabylonFlightCreateOrderRequest.DataLists();
        //        ObjIATAOrderCreateRQ.Request.DataLists.PaxList = new BabylonFlightCreateOrderRequest.PaxList();
        //        ObjIATAOrderCreateRQ.Request.DataLists.PaxList.Pax = new List<BabylonFlightCreateOrderRequest.Pax>();

        //        int PaxCount = 1; int InfantCount = 1; int TotalPaxWithoutInfant = adultdata.Count() + childdata.Count();
        //        for (int i = 0; i < adultdata.Length; i++)
        //        {
        //            var PaxData = new BabylonFlightCreateOrderRequest.Pax();
        //            PaxData.IdentityDoc = new BabylonFlightCreateOrderRequest.IdentityDoc();
        //            PaxData.IdentityDoc.IdentityDocTypeCode = "PP";
        //            PaxData.IdentityDoc.IdentityDocID = adultdata[i].PassportNo;
        //            PaxData.IdentityDoc.IssuingCountryCode = adultdata[i].CountryCode;
        //            PaxData.IdentityDoc.ExpiryDate = DateTime.ParseExact(Convert.ToString(adultdata[i].DateofExpiry), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
        //            PaxData.IdentityDoc.ResidenceCountryCode = adultdata[i].CountryCode;
        //            if (i == 0)
        //            {
        //                PaxData.ContactInfoRefID = "CIADT" + PaxCount.ToString();
        //            }
        //            PaxData.PaxID = "SH" + PaxCount.ToString();
        //            PaxData.PTC = "ADT";
        //            if (infant.Count() > 0 && i < infant.Count())
        //            {
        //                PaxData.PaxRefID = "SH" + (TotalPaxWithoutInfant + InfantCount).ToString();
        //                InfantCount++;
        //            }

        //            PaxData.Individual = new BabylonFlightCreateOrderRequest.Individual();
        //            PaxData.Individual.Birthdate = DateTime.ParseExact(Convert.ToString(adultdata[i].DOB), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
        //            PaxData.Individual.IndividualID = "SH" + PaxCount.ToString();
        //            PaxData.Individual.NameTitle = adultdata[i].Title.Replace(".", "");
        //            PaxData.Individual.GivenName = adultdata[i].FirstName;
        //            PaxData.Individual.MiddleName = adultdata[i].MiddleName;
        //            PaxData.Individual.Surname = adultdata[i].LastName;

        //            ObjIATAOrderCreateRQ.Request.DataLists.PaxList.Pax.Add(PaxData);
        //            PaxCount++;
        //        }

        //        for (int i = 0; i < childdata.Length; i++)
        //        {
        //            var PaxData = new BabylonFlightCreateOrderRequest.Pax();
        //            PaxData.IdentityDoc = new BabylonFlightCreateOrderRequest.IdentityDoc();
        //            PaxData.IdentityDoc.IdentityDocTypeCode = "PP";
        //            PaxData.IdentityDoc.IdentityDocID = childdata[i].PassportNo;
        //            PaxData.IdentityDoc.IssuingCountryCode = childdata[i].CountryCode;
        //            PaxData.IdentityDoc.ExpiryDate = DateTime.ParseExact(Convert.ToString(childdata[i].DateofExpiry), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
        //            PaxData.IdentityDoc.ResidenceCountryCode = childdata[i].CountryCode;
        //            PaxData.PaxID = "SH" + PaxCount.ToString();
        //            PaxData.PTC = "CHD";

        //            PaxData.Individual = new BabylonFlightCreateOrderRequest.Individual();
        //            PaxData.Individual.Birthdate = DateTime.ParseExact(Convert.ToString(childdata[i].DOB), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
        //            PaxData.Individual.IndividualID = "SH" + PaxCount.ToString();
        //            PaxData.Individual.NameTitle = childdata[i].Title.Replace(".", "");
        //            PaxData.Individual.GivenName = childdata[i].FirstName;
        //            PaxData.Individual.MiddleName = childdata[i].MiddleName;
        //            PaxData.Individual.Surname = childdata[i].LastName;

        //            ObjIATAOrderCreateRQ.Request.DataLists.PaxList.Pax.Add(PaxData);
        //            PaxCount++;
        //        }

        //        for (int i = 0; i < infant.Length; i++)
        //        {
        //            var PaxData = new BabylonFlightCreateOrderRequest.Pax();
        //            PaxData.IdentityDoc = new BabylonFlightCreateOrderRequest.IdentityDoc();
        //            PaxData.IdentityDoc.IdentityDocTypeCode = "PP";
        //            PaxData.IdentityDoc.IdentityDocID = infant[i].PassportNo;
        //            PaxData.IdentityDoc.IssuingCountryCode = infant[i].CountryCode;
        //            PaxData.IdentityDoc.ExpiryDate = DateTime.ParseExact(Convert.ToString(infant[i].DateofExpiry), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
        //            PaxData.IdentityDoc.ResidenceCountryCode = infant[i].CountryCode;
        //            PaxData.PaxID = "SH" + PaxCount.ToString();
        //            PaxData.PTC = "INF";

        //            PaxData.Individual = new BabylonFlightCreateOrderRequest.Individual();
        //            PaxData.Individual.Birthdate = DateTime.ParseExact(Convert.ToString(infant[i].DOB), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
        //            PaxData.Individual.IndividualID = "SH" + PaxCount.ToString();
        //            PaxData.Individual.NameTitle = infant[i].Title.Replace(".", "");
        //            PaxData.Individual.GivenName = infant[i].FirstName;
        //            PaxData.Individual.MiddleName = infant[i].MiddleName;
        //            PaxData.Individual.Surname = infant[i].LastName;

        //            ObjIATAOrderCreateRQ.Request.DataLists.PaxList.Pax.Add(PaxData);
        //            PaxCount++;
        //        }

        //        ObjIATAOrderCreateRQ.Request.DataLists.ContactInfoList = new BabylonFlightCreateOrderRequest.ContactInfoList();
        //        ObjIATAOrderCreateRQ.Request.DataLists.ContactInfoList.ContactInfo = new BabylonFlightCreateOrderRequest.ContactInfo();
        //        ObjIATAOrderCreateRQ.Request.DataLists.ContactInfoList.ContactInfo.ContactInfoID = "CIADT1";
        //        ObjIATAOrderCreateRQ.Request.DataLists.ContactInfoList.ContactInfo.EmailAddress = new BabylonFlightCreateOrderRequest.EmailAddress();
        //        ObjIATAOrderCreateRQ.Request.DataLists.ContactInfoList.ContactInfo.EmailAddress.ContactTypeText = "PER";
        //        ObjIATAOrderCreateRQ.Request.DataLists.ContactInfoList.ContactInfo.EmailAddress.EmailAddressText = adultdata[0].Email;
        //        ObjIATAOrderCreateRQ.Request.DataLists.ContactInfoList.ContactInfo.Phone = new BabylonFlightCreateOrderRequest.Phone();
        //        ObjIATAOrderCreateRQ.Request.DataLists.ContactInfoList.ContactInfo.Phone.ContactTypeText = "PER";
        //        ObjIATAOrderCreateRQ.Request.DataLists.ContactInfoList.ContactInfo.Phone.CountryDialingCode = adultdata[0].MobileCode;
        //        ObjIATAOrderCreateRQ.Request.DataLists.ContactInfoList.ContactInfo.Phone.PhoneNumber = adultdata[0].MobileNo;

        //        if (ObjIATAOfferPriceRS.IATA_OfferPriceRS.Response.AllowedOperations != null && !string.IsNullOrEmpty(ObjIATAOfferPriceRS.IATA_OfferPriceRS.Response.AllowedOperations.HoldPNR) && ObjIATAOfferPriceRS.IATA_OfferPriceRS.Response.AllowedOperations.HoldPNR.ToString().ToLower() == "n")
        //        {
        //            ObjIATAOrderCreateRQ.Request.PaymentFunctions = new BabylonFlightCreateOrderRequest.PaymentFunctions();
        //            ObjIATAOrderCreateRQ.Request.PaymentFunctions.PaymentMethodCriteria = new BabylonFlightCreateOrderRequest.PaymentMethodCriteria();
        //            ObjIATAOrderCreateRQ.Request.PaymentFunctions.PaymentMethodCriteria.TypeCode = "CA";
        //        }

        //        string XmlReq = jsonFunction.CreateXMLByClass(ObjIATAOrderCreateRQ);
        //        await _iairApiCommonFunction.Insert_Tbl_FlightRequestResponse("OrderCreate", "Request", XmlReq, XmlDataid, ObjBabylonSetting.ApiSupplierId.ToString());
        //        string XmlResponse = await GetBabylonResponsesXml(XmlReq, _babylonCredential.OrderCreate ?? "", Token);
        //        await _iairApiCommonFunction.Insert_Tbl_FlightRequestResponse("OrderCreate", "Response", XmlResponse, XmlDataid, ObjBabylonSetting.ApiSupplierId.ToString());
        //        if (!string.IsNullOrEmpty(XmlResponse))
        //        {
        //            XmlDocument doc = new XmlDocument();
        //            doc.LoadXml(XmlResponse);
        //            string json = JsonConvert.SerializeXmlNode(doc);
        //            ObjAirCreateOrderRS = JsonConvert.DeserializeObject<BabylonFlightCreateOrderResponse.Babylon_AirCreateOrderRS>(json.Replace("@CurCode", "CurCode").Replace("#text", "text"));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //await _iairApiCommonFunction.InsertLogError(ex, 1, );
        //    }
        //    return ObjAirCreateOrderRS;
        //}

        //public async Task<BabylonFlightCreateOrderResponse.Babylon_AirCreateOrderRS> OrderChange(BabylonFlightOfferFareResponse.IATAOfferPriceRS ObjIATAOfferPriceRS, BabylonFlightCreateOrderResponse.Babylon_AirCreateOrderRS ObjAirCreateOrderRS, BabylonSetting ObjBabylonSetting, PaxDetails[] adultdata, PaxDetails[] childdata, PaxDetails[] infant, string XmlDataid, string Token)
        //{
        //    BabylonFlightCreateOrderResponse.Babylon_AirCreateOrderRS ObjAirrderChangeRS = new BabylonFlightCreateOrderResponse.Babylon_AirCreateOrderRS();
        //    try
        //    {
        //        BabylonFlightOrderChangeRequest.IATAOrderChangeRQ ObjIATAOrderChangeRQ = new BabylonFlightOrderChangeRequest.IATAOrderChangeRQ();
        //        ObjIATAOrderChangeRQ.MessageDoc = new BabylonFlightOrderChangeRequest.MessageDoc();
        //        ObjIATAOrderChangeRQ.MessageDoc.RefVersionNumber = 19.2;
        //        ObjIATAOrderChangeRQ.MessageDoc.Name = "NDC GATEWAY";
        //        ObjIATAOrderChangeRQ.Party = new BabylonFlightOrderChangeRequest.Party();
        //        ObjIATAOrderChangeRQ.Party.Sender = new BabylonFlightOrderChangeRequest.Sender();
        //        ObjIATAOrderChangeRQ.Party.Sender.TravelAgencySender = new BabylonFlightOrderChangeRequest.TravelAgencySender();
        //        ObjIATAOrderChangeRQ.Party.Sender.TravelAgencySender.AgencyID = ObjBabylonSetting.AgentCode;
        //        ObjIATAOrderChangeRQ.Party.Sender.TravelAgencySender.PseudoCity = string.Empty;
        //        ObjIATAOrderChangeRQ.Request = new BabylonFlightOrderChangeRequest.Request();
        //        ObjIATAOrderChangeRQ.Request.Order = new BabylonFlightOrderChangeRequest.Order();
        //        ObjIATAOrderChangeRQ.Request.Order.OrderID = ObjAirCreateOrderRS.IATA_OrderViewRS.Response.Order.OrderID;
        //        ObjIATAOrderChangeRQ.Request.Order.OwnerCode = ObjIATAOfferPriceRS.IATA_OfferPriceRS.Response.PricedOffer.OwnerCode;
        //        ObjIATAOrderChangeRQ.Request.PaymentFunctions = new BabylonFlightOrderChangeRequest.PaymentFunctions();
        //        ObjIATAOrderChangeRQ.Request.PaymentFunctions.PaymentMethodCriteria = new BabylonFlightOrderChangeRequest.PaymentMethodCriteria();
        //        ObjIATAOrderChangeRQ.Request.PaymentFunctions.PaymentMethodCriteria.TypeCode = "CA";
        //        ObjIATAOrderChangeRQ.Request.DataLists = new BabylonFlightOrderChangeRequest.DataLists();
        //        ObjIATAOrderChangeRQ.Request.DataLists.PaxList = new BabylonFlightOrderChangeRequest.PaxList();
        //        ObjIATAOrderChangeRQ.Request.DataLists.PaxList.Pax = new List<BabylonFlightOrderChangeRequest.Pax>();

        //        for (int i = 0; i < adultdata.Length; i++)
        //        {
        //            var PaxData = new BabylonFlightOrderChangeRequest.Pax();
        //            PaxData.PaxID = "ADT" + (i + 1).ToString();
        //            PaxData.PTC = "ADT";

        //            PaxData.Individual = new BabylonFlightOrderChangeRequest.Individual();
        //            PaxData.Individual.Birthdate = DateTime.ParseExact(Convert.ToString(adultdata[i].DOB), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
        //            PaxData.Individual.NameTitle = adultdata[i].Title.Replace(".", "");
        //            PaxData.Individual.GivenName = adultdata[i].FirstName;
        //            PaxData.Individual.Surname = adultdata[i].LastName;
        //            ObjIATAOrderChangeRQ.Request.DataLists.PaxList.Pax.Add(PaxData);

        //        }

        //        for (int i = 0; i < childdata.Length; i++)
        //        {
        //            var PaxData = new BabylonFlightOrderChangeRequest.Pax();
        //            PaxData.PaxID = "CHD" + (i + 1).ToString();
        //            PaxData.PTC = "CHD";

        //            PaxData.Individual = new BabylonFlightOrderChangeRequest.Individual();
        //            PaxData.Individual.Birthdate = DateTime.ParseExact(Convert.ToString(childdata[i].DOB), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
        //            PaxData.Individual.NameTitle = childdata[i].Title.Replace(".", "");
        //            PaxData.Individual.GivenName = childdata[i].FirstName;
        //            PaxData.Individual.Surname = childdata[i].LastName;
        //            ObjIATAOrderChangeRQ.Request.DataLists.PaxList.Pax.Add(PaxData);
        //        }

        //        for (int i = 0; i < infant.Length; i++)
        //        {
        //            var PaxData = new BabylonFlightOrderChangeRequest.Pax();
        //            PaxData.PaxID = "INF" + (i + 1).ToString();
        //            PaxData.PTC = "INF";

        //            PaxData.Individual = new BabylonFlightOrderChangeRequest.Individual();
        //            PaxData.Individual.Birthdate = DateTime.ParseExact(Convert.ToString(infant[i].DOB), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
        //            PaxData.Individual.NameTitle = infant[i].Title.Replace(".", "");
        //            PaxData.Individual.GivenName = infant[i].FirstName;
        //            PaxData.Individual.Surname = infant[i].LastName;
        //            ObjIATAOrderChangeRQ.Request.DataLists.PaxList.Pax.Add(PaxData);
        //        }
        //        string XmlReq = jsonFunction.CreateXMLByClass(ObjIATAOrderChangeRQ);
        //        await _iairApiCommonFunction.Insert_Tbl_FlightRequestResponse("OrderChange", "Request", XmlReq, XmlDataid, ObjBabylonSetting.ApiSupplierId.ToString());
        //        string XmlResponse = await GetBabylonResponsesXml(XmlReq, _babylonCredential.OrderChange ?? "", Token);
        //        await _iairApiCommonFunction.Insert_Tbl_FlightRequestResponse("OrderChange", "Response", XmlResponse, XmlDataid, ObjBabylonSetting.ApiSupplierId.ToString());
        //        if (!string.IsNullOrEmpty(XmlResponse))
        //        {
        //            XmlDocument doc = new XmlDocument();
        //            doc.LoadXml(XmlResponse);
        //            string json = JsonConvert.SerializeXmlNode(doc);
        //            ObjAirrderChangeRS = JsonConvert.DeserializeObject<BabylonFlightCreateOrderResponse.Babylon_AirCreateOrderRS>(json.Replace("@CurCode", "CurCode").Replace("#text", "text"));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Errorlog.InsertLogError(ex, 1);
        //    }
        //    return ObjAirrderChangeRS;
        //}

        //public async Task<BabylonFlightCreateOrderResponse.Babylon_AirCreateOrderRS> OrderRetrieve(BabylonSetting ObjBabylonSetting, string OrderId, string XmlDataid, string Token)
        //{
        //    BabylonFlightCreateOrderResponse.Babylon_AirCreateOrderRS ObjIATAOrderRetrieve = new BabylonFlightCreateOrderResponse.Babylon_AirCreateOrderRS();
        //    try
        //    {
        //        BabylonIATA_OrderRetrieveRequest.IATAOrderRetrieveRQ ObjIATAOrderRetrieveRQ = new BabylonIATA_OrderRetrieveRequest.IATAOrderRetrieveRQ();
        //        ObjIATAOrderRetrieveRQ.Request = new BabylonIATA_OrderRetrieveRequest.Request();
        //        ObjIATAOrderRetrieveRQ.Request.OrderFilterCriteria = new BabylonIATA_OrderRetrieveRequest.OrderFilterCriteria();
        //        ObjIATAOrderRetrieveRQ.Request.OrderFilterCriteria.BookingRefFilterCriteria = new BabylonIATA_OrderRetrieveRequest.BookingRefFilterCriteria();
        //        ObjIATAOrderRetrieveRQ.Request.OrderFilterCriteria.BookingRefFilterCriteria.BookingEntity = string.Empty;
        //        ObjIATAOrderRetrieveRQ.Request.OrderFilterCriteria.BookingRefFilterCriteria.BookingID = string.Empty;
        //        ObjIATAOrderRetrieveRQ.Request.OrderFilterCriteria.Order = new BabylonIATA_OrderRetrieveRequest.Order();
        //        ObjIATAOrderRetrieveRQ.Request.OrderFilterCriteria.Order.OrderID = OrderId;
        //        ObjIATAOrderRetrieveRQ.Request.OrderFilterCriteria.Order.OwnerCode = string.Empty;

        //        string XmlReq = jsonFunction.CreateXMLByClass(ObjIATAOrderRetrieveRQ);
        //        await _iairApiCommonFunction.Insert_Tbl_FlightRequestResponse("OrderRetrieve", "Request", XmlReq, XmlDataid, ObjBabylonSetting.ApiSupplierId.ToString());

        //        string XmlResponse = await GetBabylonResponsesXml(XmlReq, _babylonCredential.OrderRetrieve ?? "", Token);
        //        await _iairApiCommonFunction.Insert_Tbl_FlightRequestResponse("OrderRetrieve", "Response", XmlResponse, XmlDataid, ObjBabylonSetting.ApiSupplierId.ToString());
        //        if (!string.IsNullOrEmpty(XmlResponse))
        //        {
        //            XmlDocument doc = new XmlDocument();
        //            doc.LoadXml(XmlResponse);
        //            string json = JsonConvert.SerializeXmlNode(doc);
        //            ObjIATAOrderRetrieve = JsonConvert.DeserializeObject<BabylonFlightCreateOrderResponse.Babylon_AirCreateOrderRS>(json.Replace("@CurCode", "CurCode").Replace("#text", "text"));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Errorlog.InsertLogError(ex, 1);
        //    }
        //    return ObjIATAOrderRetrieve;
        //}

        public async Task<string> GetBabylonResponsesXml(string xmlRequest, string url, string token)
        {
            string XmlResponse = string.Empty;
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest();
                request.Method = Method.Post;
                request.AddHeader("Content-Type", "application/xml");
                if (!string.IsNullOrEmpty(token)) { request.AddHeader("Authorization", token); }
                request.AddParameter("application/xml", xmlRequest, ParameterType.RequestBody);

                var response = await client.ExecuteAsync(request);
                if (response.Content != null && response.Content != "")
                {
                    XmlResponse = response.Content;
                }
            }
            catch (OperationCanceledException)
            {
                // Timeout
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return XmlResponse;
        }
    }
}
