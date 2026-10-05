using EtosApiServices.Hotel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Library.Contract
{
    public interface IContractedHotel
    {
        Task<List<ETSearchHotel>> GetContractedHotelApiResponseAsync(EHotelSearchRequest SearchReq, Dictionary<string, string> ApiCredentials, string FolderName, MarkupClass MarkupClassObj);
       
        Task<EHotelDetail> GetContractedHotelApiDetailResponseAsync(EHotelDetailRequest DetailReq, Dictionary<string, string> ApiCredentials, string FolderName, string BHRDID, MarkupClass MarkupClassObj);
    }
}
