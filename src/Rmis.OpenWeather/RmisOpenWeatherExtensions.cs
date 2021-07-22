using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Rmis.OpenWeather
{
    public static class RmisOpenWeatherExtensions
    {
        public static IServiceCollection AddRmisOpenWeather(this IServiceCollection services, IConfiguration configuration)
        {
            IConfiguration config = configuration.GetSection("OpenWeather");
            
            return services.Configure<OpenWeatherConfig>(config)
                .AddSingleton<IOpenWeatherProvider, OpenWeatherProvider>();
        }
    }
}