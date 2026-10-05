using EtosApiServices.Data.Library.Common;
using EtosApiServices.Hotel.Library.Contract;
using EtosApiServices.Hotel.Model;
using EtosApiServices.MongoDb.Library;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Library.Services
{
    public class ContractedHotel : IContractedHotel
    {
        private readonly IAPICredentials _iapiCredentials;
        readonly HotelApiSetting _hotelApiSetting;
        public readonly string APIName = AppConfig.GetConfigValue("Hotel:HotelBed:APIName");
        public readonly int ApiSupplierId = int.Parse(AppConfig.GetConfigValue("Hotel:HotelBed:SupplierId"));
        public readonly string APISupplierName = AppConfig.GetConfigValue("Hotel:HotelBed:SupplierName");//"HB";

        public ContractedHotel(IAPICredentials aPICredentials, IOptions<HotelApiSetting> hotelApiSetting)
        {
            _iapiCredentials = aPICredentials ?? throw new ArgumentException(nameof(aPICredentials));
            _hotelApiSetting = hotelApiSetting?.Value ?? throw new ArgumentException(nameof(hotelApiSetting));
        }

        public async Task<List<ETSearchHotel>> GetContractedHotelApiResponseAsync(EHotelSearchRequest SearchReq, Dictionary<string, string> ApiCredentials, string FolderName, MarkupClass MarkupClassObj)
        {
            var resultList = new List<ETSearchHotel>();


            return resultList;
        }

        public async Task<EHotelDetail> GetContractedHotelApiDetailResponseAsync(EHotelDetailRequest DetailReq, Dictionary<string, string> ApiCredentials, string FolderName, string BHRDID, MarkupClass MarkupClassObj)
        {
            var HotelObj = new EHotelDetail();

            return HotelObj;
        }

        }
}
