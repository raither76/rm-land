using Microsoft.Extensions.DependencyInjection;
using Rmis.Application.Abstract;

namespace Rmis.Application
{
    public static class RmisApplicationExtensions
    {
        public static IServiceCollection AddRmisApplication(this IServiceCollection services)
        {
            return services.AddTransient<IScheduleService, ScheduleService>();
        }
    }
}