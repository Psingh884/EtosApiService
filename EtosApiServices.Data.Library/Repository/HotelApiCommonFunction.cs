using Dapper;
using EtosApiServices.Data.Library.Common;
using EtosApiServices.Flight.Model;
using EtosApiServices.GenericRepository.Contracts;
using EtosApiServices.Hotel.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Data.Library.Repository
{
    public class HotelApiCommonFunction : IHotelApiCommonFunction
    {
        readonly IGenericRepository _genericRepository;

        public HotelApiCommonFunction(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository ?? throw new ArgumentNullException(nameof(genericRepository));
        }
        public async Task<(IEnumerable<PreHotelDetails> Result1, IEnumerable<PreHotelServiceCart> Result2)> GetPreHotelDetailsAsync(string HRDID, int SID, string DBConnectionString)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("ID", HRDID);
                parameters.Add("SID", SID);
                var resultEnumerable = await _genericRepository.ExecuteQueryMultipleAsync<PreHotelDetails, PreHotelServiceCart>("GetPreHotelDetails", parameters, DBConnectionString);
                return resultEnumerable;
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it)
                throw;
            }
        }
    }
}
