using EtosApiServices.Business.Contract;
using EtosApiServices.Business.Services;
using EtosApiServices.Data.Library.Common;
using EtosApiServices.Data.Library.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Business
{
    public static class BusinessServiceCollection
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IAirApiCommonFunction, AirApiCommonFunction>();
            services.AddScoped<IFlightService, FlightService>();
            services.AddScoped<IHotelService, HotelService>();
            return services;
        }
    }
}
