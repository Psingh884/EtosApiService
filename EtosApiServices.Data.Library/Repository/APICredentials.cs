using Dapper;
using EtosApiServices.Data.Library.Common;
using EtosApiServices.GenericRepository.Contracts;
using EtosApiServices.Hotel.Model;
using EtosApiServices.Model;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Data.Library.Repository
{
    public class APICredentials : IAPICredentials
    {
        readonly IGenericRepository _genericRepository;

        public APICredentials(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository ?? throw new ArgumentNullException(nameof(genericRepository));
        }

        public async Task<Dictionary<int, Dictionary<string, string>>> GetAPICredentialsByApiKey(int CompanyID, string ApiKey, int ServiceTypeID, bool IsLive, string DBConnectionString)
        {
            var Result = new Dictionary<int, Dictionary<string, string>>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyID", CompanyID);
                parameters.Add("ApiKey", ApiKey);
                parameters.Add("ServiceTypeID", ServiceTypeID);
                parameters.Add("IsLive", IsLive);
                var resultEnumerable = await _genericRepository.ExecuteQueryListAsync<HotelApiCredential>("GetAPICredentialsByApiKey", parameters, DBConnectionString);
                if (resultEnumerable != null && resultEnumerable.Count() > 0)
                {
                    Result = resultEnumerable
                        .Where(h => h != null)
                        .Select(h => new { h.APIID, Dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(h.APIRawData ?? "") ?? new Dictionary<string, string>() })
                        .GroupBy(x => x.APIID)
                        .ToDictionary(
                            g => g.Key,
                            g => g
                                .SelectMany(x => x.Dict)
                                .GroupBy(kv => kv.Key)
                                .ToDictionary(kg => kg.Key, kg => kg.Last().Value) // if duplicate keys keep last value
                        );
                }
            }
            catch (Exception ex)
            {
                //Errorlog.InsertLogError(ex, 1);
            }
            return Result;
        }

        public async Task<Dictionary<int, Dictionary<string, string>>> SortAgentWiseAvaibaleAPIBy(Dictionary<int, Dictionary<string, string>> apic, int AgentContractID, string DBConnectionString)
        {
            var Result = new Dictionary<int, Dictionary<string, string>>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("AgentContractID", AgentContractID);
                var resultEnumerable = await _genericRepository.ExecuteQueryListAsync<HotelApiCredential>("GetAgentWiseAvaibaleAPI", parameters, DBConnectionString);
                if (resultEnumerable != null && resultEnumerable.Count() > 0)
                {
                    foreach (var credential in resultEnumerable)
                    {
                        int APIId = Convert.ToInt32(credential.APIID);
                        if (apic.ContainsKey(APIId))
                        {
                            Result.Add(APIId, apic[APIId]);
                        }
                    }
                }
                else
                {
                    Result = apic;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Result;
        }

        public async Task<Dictionary<int, MarkupClass>> GetAPIMarkupsByAPIKey(int CompanyID, string APIKey, int ServiceID, int IsB2C, int CountryID, int CityID, int AgentContractID, string DBConnectionString)
        {
            Dictionary<int, MarkupClass> Result = new Dictionary<int, MarkupClass>();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("CompanyID", CompanyID);
                parameters.Add("ServiceID", ServiceID);
                parameters.Add("IsB2C", IsB2C);
                parameters.Add("CountryID", CountryID);
                parameters.Add("CityID", CityID);
                parameters.Add("AgentContractID", AgentContractID);
                var resultEnumerable = await _genericRepository.ExecuteQueryListAsync<APIMarkupClass>("GetAPIMarkupsByCompanyID", parameters, DBConnectionString);
                if (resultEnumerable != null && resultEnumerable.Count() > 0)
                {
                    Result = resultEnumerable
               .Where(h => h != null)
               .GroupBy(h => h.APIMSupplierID)
               .ToDictionary(
                   g => g.Key,
                   g => new MarkupClass
                   {
                       IsPercentage = g.Last().MarkUpIsPercentage,
                       MVal = g.Last().MarkUP,
                       TVal = g.Last().TaxVal,
                       TaxType = g.Last().TaxType,
                       IsOverAll = g.Last().IsOverAllPerPax
                   }
               );
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return Result;
        }

        public async Task<decimal> GetCurrancyRate(string Currency, int CompanyID, string DBConnectionString)
        {
            decimal rate = 1;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Currency", Currency);
                parameters.Add("CompanyID", CompanyID);
                var resultEnumerable = await _genericRepository.ExecuteQueryAsync<CurrencyROE>("GetCurrency_from_USD_SV3", parameters, DBConnectionString);
                if (resultEnumerable != null)
                {
                    rate= resultEnumerable.ROE;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return rate;
        }
    }
}