using EtosApiService.Filter;
using EtosApiServices.Business;
using EtosApiServices.Data.Library;
using EtosApiServices.DataBase.Dapper;
using EtosApiServices.Flight.Library;
using EtosApiServices.Flight.Model.ApiSetting;
using EtosApiServices.GenericRepository;
using EtosApiServices.Hotel.Library;
using EtosApiServices.Hotel.Model;
using EtosApiServices.Hotel.Model.ApiSetting;
using EtosApiServices.Model.Setting;

namespace EtosApiService.Common
{
    public static class ServiceConfiguration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddDBContextServices();
            services.AddGenericRepositoryServices();
            services.AddDataAccessServices();
            services.AddBusinessServices();
            services.AddFlightDataAccessServices();
            services.AddHotelDataAccessServices();
        }

        public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConnectionStrings>(configuration.GetSection("ConnectionStrings"));
            services.Configure<APIKeySettings>(configuration.GetSection("APIKeySettings"));
            services.Configure<BabylonCredential>(configuration.GetSection("FlightCredentials:Babylon"));
            services.Configure<HotelApiSetting>(configuration.GetSection("HotelApiSetting"));
            services.Configure<HotelBedsCredential>(configuration.GetSection("HotelApiCredentials:HotelBeds"));
            services.Configure<ApiFileServerPathPaths>(configuration.GetSection("Paths"));

            return services;
        }

        public static IServiceCollection AddActionFilter(this IServiceCollection services)
        {
            services.AddScoped<ValidateTokenFilter>();
            return services;
        }

        public static IServiceCollection AddEtosAPIServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationConfiguration(configuration);
            services.AddApplicationServices();
            services.AddActionFilter();
            return services;
        }
    }
}
