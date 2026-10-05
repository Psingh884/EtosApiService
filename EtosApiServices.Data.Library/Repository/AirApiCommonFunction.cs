using Dapper;
using EtosApiServices.Data.Library.Common;
using EtosApiServices.Flight.Model;
using EtosApiServices.GenericRepository.Contracts;
using EtosApiServices.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Data.Library.Repository
{
    public class AirApiCommonFunction : IAirApiCommonFunction
    {
        readonly IGenericRepository _genericRepository;

        public AirApiCommonFunction(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository ?? throw new ArgumentNullException(nameof(genericRepository));
        }

        public async Task<List<FlightSupplierDetails>> GetCompanyFlightSupplierList(int AgentId, int ApiServiceId, int CompanyId, string DBConnectionString)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("AgentId", AgentId);
                parameters.Add("Serviceid", ApiServiceId);
                parameters.Add("CompanyId", CompanyId);
                var resultEnumerable = await _genericRepository.ExecuteQueryListAsync<FlightSupplierDetails>("GetCompanyFlightSupplierList", parameters, DBConnectionString);
                var Result = resultEnumerable?.ToList();
                if (Result == null || !Result.Any())
                {
                    throw new UnauthorizedAccessException("Invalid credentials provided.");
                }
                return Result;
            }
            catch (Exception ex)
            {
                return new List<FlightSupplierDetails>();
            }
        }

        public async Task<List<FlightSupplierCredential>> GetCompanyFlightCredentialList(int PlayerId, string DBConnectionString)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("PlayerId", PlayerId);
                var resultEnumerable = await _genericRepository.ExecuteQueryListAsync<FlightSupplierCredential>("GetCompanyFlightSupplier", parameters, DBConnectionString);
                var Result = resultEnumerable?.ToList();
                if (Result == null || !Result.Any())
                {
                    throw new UnauthorizedAccessException("Invalid credentials provided.");
                }
                return Result;
            }
            catch (Exception ex)
            {
                return new List<FlightSupplierCredential>();
            }
        }

        public async Task<bool> Insert_Tbl_FlightRequestResponse(string RequestName, string RequestType, string RequestStr, string XmldataId, string SupplierId, string DBConnectionString)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("RequestName", RequestName);
                parameters.Add("RequestType", RequestType);
                parameters.Add("RequestStr", RequestStr);
                parameters.Add("XmldataId", XmldataId);
                parameters.Add("SupplierId", SupplierId);
                var result = await _genericRepository.ExecuteQueryAsync<bool>("Insert_Tbl_FlightRequestResponse", parameters, DBConnectionString);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> Flights_InsertAirSearchRequest(string SearchRequest, string XmlDataid, string Token, string DBConnectionString)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("SearchRequest", SearchRequest);
                parameters.Add("XmlDataid", XmlDataid);
                parameters.Add("Token", Token);
                var result = await _genericRepository.ExecuteQueryAsync<bool>("InsertAirSearchRequest", parameters, DBConnectionString);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<decimal> GetCurrencyConversionRate(string Currency, int CompanyId, string DBConnectionString)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Currency", Currency);
                parameters.Add("CompanyId", CompanyId);
                var result = await _genericRepository.ExecuteQueryAsync<CurrencyROE>("GetCurrency_from_USD_SV3", parameters, DBConnectionString);
                if (result != null)
                {
                    return result.ROE;
                }
                return 1;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }

        public async Task<int> Checkifdomestic(string DepartureCity, string DestinationCity, int ApiSupplierId, int JourneyTypeId, string DBConnectionString)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("DepartureCity", DepartureCity);
                parameters.Add("DestinationCity", DestinationCity);
                parameters.Add("ApiSupplierId", ApiSupplierId);
                parameters.Add("JourneyTypeId", JourneyTypeId);
                var result = await _genericRepository.ExecuteQueryAsync<int>("Checkifdomestic", parameters, DBConnectionString);
                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<string> GetPassengers(string pax)
        {
            string getpass = string.Empty;
            if (pax == "ADT" || pax == "ADLT")
            {
                getpass = "Adult";
            }
            else if (pax == "CH" || pax == "CHD" || pax == "CNN" || pax == "CHLD")
            {
                getpass = "Child";
            }
            else if (pax == "INF" || pax == "INY" || pax == "INFT" || pax == "IN")
            {
                getpass = "Infant";
            }
            return getpass;
        }

        public async Task<bool> InsertLogError(Exception Ex, int AgentId, string DBConnectionString)
        {
            try
            {
                string totalerror = Ex.StackTrace ?? "" + "<br />" + Ex.Message.ToString();
                string WebSiteName = AppDomain.CurrentDomain.FriendlyName;
                int IsB2c = AgentId > 0 ? 1 : 0;
                StackTrace trace = new StackTrace(Ex, true);
                StackFrame stackFrame = trace.GetFrame(trace.FrameCount - 1);
                string fileName = stackFrame.GetFileName();
                string methodName = stackFrame.GetMethod().Name;
                string lineNo = stackFrame.GetFileLineNumber().ToString();

                var parameters = new DynamicParameters();
                parameters.Add("WebsiteName", WebSiteName);
                parameters.Add("PageName", fileName);
                parameters.Add("MethodName", methodName);
                parameters.Add("ErrorLineNo", lineNo);
                parameters.Add("IsB2c", IsB2c);
                parameters.Add("ErrorText", totalerror);
                var result = await _genericRepository.ExecuteQueryAsync<bool>("Insert_tbl_ErrorLog", parameters, DBConnectionString);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
