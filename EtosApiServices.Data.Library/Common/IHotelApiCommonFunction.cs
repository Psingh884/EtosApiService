using EtosApiServices.Hotel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Data.Library.Common
{
    public interface IHotelApiCommonFunction
    {
        Task<(IEnumerable<PreHotelDetails> Result1, IEnumerable<PreHotelServiceCart> Result2)> GetPreHotelDetailsAsync(string HRDID, int SID, string DBConnectionString);

    }
}
