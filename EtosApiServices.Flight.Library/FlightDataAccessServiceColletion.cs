using EtosApiServices.Flight.Library.Contract;
using EtosApiServices.Flight.Library.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Flight.Library
{
    public static class FlightDataAccessServiceColletion
    {
        public static IServiceCollection AddFlightDataAccessServices(this IServiceCollection services)
        {
            services.AddScoped<IBabylon, Babylon>();
            services.AddScoped<BabylonApiRequest>();
            return services;
        }
    }
}
