using EtosApiServices.Flight.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Flight.Library.Contract
{
    public interface IBabylon
    {
        Task<List<FlightSerachResponse.FlightShoppingResponse>> BabylonSearch(FlightSearchRequest ObjFlightSearchRequest, FlightSupplierCredential ObjBabylonCredential);
    }
}
