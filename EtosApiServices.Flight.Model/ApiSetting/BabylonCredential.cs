using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Flight.Model.ApiSetting
{
    public class BabylonCredential
    {
        public string? Token { get; set; }
        public string? FlightSearch { get; set; }
        public string? FareRules { get; set; }
        public string? OfferPrice { get; set; }
        public string? OrderCreate { get; set; }
        public string? OrderChange { get; set; }
        public string? OrderRetrieve { get; set; }
    }

    public class BabylonSetting
    {
        public string? AgentCode { get; set; }
        public string? AppKey { get; set; }
        public string? SecretKey { get; set; }
        public string? ApiSupplierName { get; set; }
        public int ApiSupplierId { get; set; }
    }
}

public enum BabylonCabinType
{
    First = 1,
    Business = 2,
    StandardEconomy = 3,
    PremiumEconomy = 4,
    Economy = 5
}

