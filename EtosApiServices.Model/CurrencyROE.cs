using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Model
{
    public class CurrencyROE
    {
        public int Pk_Id { get; set; }
        public string? Currency { get; set; }
        public decimal ROE { get; set; }

    }
}
