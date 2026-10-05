using EtosApiServices.Flight.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Business.Contract
{
    public interface IFlightService
    {
        Task<string> FlightSearch(FlightSearchRequest ObjFlightSearchRequest);
    }
}
