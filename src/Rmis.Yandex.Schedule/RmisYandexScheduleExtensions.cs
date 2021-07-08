using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rmis.Yandex.Schedule.Abstract;


namespace Rmis.Yandex.Schedule
{
    public static class RmisYandexScheduleExtensions
    {
        public static IServiceCollection AddRmisYandexSchedule(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<YandexScheduleOptions>(configuration.GetSection("YandexSchedule"));
            services.AddSingleton<IYandexScheduleProvider, YandexScheduleProvider>();
            return services;
        }
    }
}