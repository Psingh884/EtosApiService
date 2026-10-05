using EtosApiServices.GenericRepository.Contracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.GenericRepository
{
    public static class GenericRepositoryServiceCollection
    {
        public static IServiceCollection AddGenericRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped<IGenericRepository, GenericRepository.Repository.GenericRepository>();
            return services;
        }
    }
}
