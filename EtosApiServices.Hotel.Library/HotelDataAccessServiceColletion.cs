using EtosApiServices.Hotel.Library.Contract;
using EtosApiServices.Hotel.Library.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtosApiServices.Hotel.Library
{
    public static class HotelDataAccessServiceColletion
    {
        public static IServiceCollection AddHotelDataAccessServices(this IServiceCollection services)
        {
            services.AddScoped<HotelBedsApiRequest>();
            services.AddScoped<IHotelBeds, HotelBeds>();
            services.AddScoped<TboApiRequest>();
            services.AddScoped<ITBOAPI, TBOAPI>();
            services.AddScoped<IContractedHotel, ContractedHotel>();
            return services;
        }
    }
}
