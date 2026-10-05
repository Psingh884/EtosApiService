using EtosApiServices.Hotel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Data.Library.Common
{
    public interface IAPICredentials
    {
        Task<Dictionary<int, Dictionary<string, string>>> GetAPICredentialsByApiKey(int CompanyID, string ApiKey, int ServiceTypeID, bool IsLive, string DBConnectionString);
        Task<Dictionary<int, Dictionary<string, string>>> SortAgentWiseAvaibaleAPIBy(Dictionary<int, Dictionary<string, string>> apic, int AgentContractID, string DBConnectionString);
        Task<Dictionary<int, MarkupClass>> GetAPIMarkupsByAPIKey(int CompanyID, string APIKey, int ServiceID, int IsB2C, int CountryID, int CityID, int AgentContractID, string DBConnectionString);
        Task<decimal> GetCurrancyRate(string Currency, int CompanyId, string DBConnectionString);
    }
}
