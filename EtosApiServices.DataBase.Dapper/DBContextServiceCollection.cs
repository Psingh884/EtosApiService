using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace EtosApiServices.DataBase.Dapper
{
    public static class DBContextServiceCollection
    {
        public static IServiceCollection AddDBContextServices(this IServiceCollection services)
        {
            services.AddScoped<DBContext>();
            return services;
        }
    }
}
