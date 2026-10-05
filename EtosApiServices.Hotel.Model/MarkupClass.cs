using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Model
{
    public class MarkupClass
    {
        public bool IsPercentage { get; set; }
        public decimal MVal { get; set; }
        public decimal TVal { get; set; }
        /// <summary>
        /// Tax on Markup Or Overall
        /// </summary>
        public int TaxType { get; set; }
        public bool IsOverAll { get; set; }
    }

    public class APIMarkupClass
    {
        public int APIMSupplierID { get; set; }
        public bool MarkUpIsPercentage { get; set; }
        public decimal MarkUP { get; set; }
        public decimal TaxVal { get; set; }        
        public int TaxType { get; set; }
        public bool IsOverAllPerPax { get; set; }
    }
}
