using EtosApiServices.Business.Contract;
using EtosApiServices.Data.Library.Common;
using EtosApiServices.Flight.Library.Contract;
using EtosApiServices.Flight.Model;
using EtosApiServices.Model.Setting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EtosApiServices.Business.Services
{
    public class FlightService : IFlightService
    {
        private readonly IAirApiCommonFunction _iairApiCommonFunction;
        private readonly IBabylon _ibabylon;
        public FlightService(IAirApiCommonFunction iAirApiCommonFunction, IBabylon iBabylon)
        {
            _iairApiCommonFunction = iAirApiCommonFunction;
            _ibabylon = iBabylon;
        }

        public async Task<string> FlightSearch(FlightSearchRequest ObjFlightSearchRequest)
        {
            var results = new ConcurrentBag<List<FlightSerachResponse.FlightShoppingResponse>>();
            try
            {
                var FlightSupplierList = await _iairApiCommonFunction.GetCompanyFlightSupplierList(ObjFlightSearchRequest.AgentID, ObjFlightSearchRequest.ApiServiceId, ObjFlightSearchRequest.CompanyID, ObjFlightSearchRequest.DBConnectioString ?? "");
                if (FlightSupplierList != null && FlightSupplierList.Count > 0)
                {
                    var FlightSupplierCredentialList = await _iairApiCommonFunction.GetCompanyFlightCredentialList(ObjFlightSearchRequest.PlayerId, ObjFlightSearchRequest.DBConnectioString ?? "");
                    if (FlightSupplierCredentialList != null && FlightSupplierCredentialList.Count > 0)
                    {
                        // Control maximum parallelism to avoid overwhelming resources or external APIs.
                        const int maxConcurrency = 8;
                        using var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);

                        var tasks = new List<Task>();
                        foreach (var supplier in FlightSupplierList)
                        {
                            await semaphore.WaitAsync();

                            // Start work for this supplier on a background task so multiple suppliers run concurrently.
                            tasks.Add(Task.Run(async () =>
                            {
                                try
                                {
                                    var credential = FlightSupplierCredentialList.FirstOrDefault(h => h.APISupplerId == supplier.APIMSupplierID);
                                    if (supplier.APIMSupplierID == (int)FlightApiSupplier.Babylon && supplier.IsActive == 1)
                                    {
                                        var res = await _ibabylon.BabylonSearch(ObjFlightSearchRequest, credential ?? new FlightSupplierCredential());
                                        if (res != null)
                                        {
                                            results.Add(res);
                                        }
                                    }
                                    else if (supplier.APIMSupplierID == 0 && supplier.IsActive == 1)
                                    {
                                        //var res = await _ibabylon.BabylonSearch(ObjFlightSearchRequest);
                                        //if (res != null)
                                        //{
                                        //    results.Add(res);
                                        //}
                                    }
                                }
                                finally
                                {
                                    semaphore.Release();
                                }

                                // If any supplier work is asynchronous, await it here.
                                await Task.CompletedTask;
                            }));
                        }

                        // Wait for all supplier tasks to complete.
                        await Task.WhenAll(tasks);
                        return JsonSerializer.Serialize(results);
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                throw new Exception(ex.Message.ToString());
            }
            return JsonSerializer.Serialize(results);
        }
    }
}
