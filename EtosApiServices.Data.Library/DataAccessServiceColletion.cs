using EtosApiServices.Data.Library.Common;
using EtosApiServices.Data.Library.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Data.Library
{
    public static class DataAccessServiceColletion
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            services.AddScoped<IAPICredentials, APICredentials>();
            services.AddScoped<IHotelApiCommonFunction, HotelApiCommonFunction>();
            return services;
        }
    }
}
