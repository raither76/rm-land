using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rmis.Client.Application.Abstract;
using Rmis.Client.Domain;

namespace Rmis.Client.Application
{
    public static class RmisClientApplicationExtensions
    {
        public static IServiceCollection AddRmisClientApplication(this IServiceCollection services, IConfiguration configuration)
        {
            string rmisHubUrl = configuration["RmisHubUrl"];
            string routeNumber = configuration["RouteNumber"];
            AppConfig config = new()
            {
                RmisHubUrl = rmisHubUrl,
                RouteNumber = routeNumber
            };
            
            return services.AddSingleton(config)
                .AddScoped<IScheduleService, ScheduleService>();
        }
    }
}