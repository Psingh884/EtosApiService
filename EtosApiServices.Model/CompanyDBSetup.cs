using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Model
{
    public class CompanyDBSetup
    {
        public int DBConnectionId { get; set; }
        public string? ServerName { get; set; }
        public string? InitialCatalog { get; set; }
        public string? DBUserId { get; set; }
        public string? DBpassword { get; set; }
    }
}
