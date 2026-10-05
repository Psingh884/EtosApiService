using EtosApiServices.Flight.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Data.Library.Common
{
    public interface IAirApiCommonFunction
    {
        Task<List<FlightSupplierDetails>> GetCompanyFlightSupplierList(int AgentId, int ApiServiceId, int CompanyId, string DBConnectionString);

        Task<List<FlightSupplierCredential>> GetCompanyFlightCredentialList(int PlayerId, string DBConnectionString);

        Task<bool> Insert_Tbl_FlightRequestResponse(string RequestName, string RequestType, string RequestStr, string XmldataId, string SupplierId, string DBConnectionString);

        Task<bool> Flights_InsertAirSearchRequest(string SearchRequest, string XmlDataid, string Token, string DBConnectionString);

        Task<decimal> GetCurrencyConversionRate(string Currency, int CompanyId, string DBConnectionString);

        Task<int> Checkifdomestic(string DepartureCity, string DestinationCity, int ApiSupplierId, int JourneyTypeId, string DBConnectionString);

        Task<string> GetPassengers(string pax);

        Task<bool> InsertLogError(Exception Ex, int AgentId, string DBConnectionString);
    }
}
