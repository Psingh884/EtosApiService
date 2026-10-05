using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Model
{
    public class CompanyUser
    {
        public int PlayerId { get; set; }
        public int CompanyId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? AppKey { get; set; }
        public string? EmailId { get; set; }
        public string? DBConnectionString { get; set; }
    }
}
