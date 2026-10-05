using EtosApiService.Filter;
using EtosApiServices.Business.Contract;
using EtosApiServices.Flight.Model;
using Microsoft.AspNetCore.Mvc;

namespace EtosApiService.Controllers
{
    [ApiController]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    public class FlightServiceController : Controller
    {
        private readonly IFlightService _flightService;

        public FlightServiceController(IFlightService flightService)
        {
            _flightService = flightService ?? throw new ArgumentNullException(nameof(flightService));
        }

        [HttpPost]
        [Route("api/V1.0/FlightSearch")]
        public async Task<IActionResult> FlightSearch(FlightSearchRequest ObjFlightSearchRequest)
        {
            try
            {
                ObjFlightSearchRequest.CompanyID = FirmInfo.ComapnyId(Request);
                ObjFlightSearchRequest.PlayerId = FirmInfo.PlayerId(Request);
                ObjFlightSearchRequest.DBConnectioString = FirmInfo.DBConnectionString(Request);
                var result = await _flightService.FlightSearch(ObjFlightSearchRequest);

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
