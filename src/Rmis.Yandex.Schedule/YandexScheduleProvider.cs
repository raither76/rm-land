using Microsoft.Extensions.Options;
using Rmis.Yandex.Schedule.Abstract;

namespace Rmis.Yandex.Schedule
{
    internal class YandexScheduleProvider : IYandexScheduleProvider 
    {
        private readonly YandexScheduleOptions _config;

        public YandexScheduleProvider(IOptions<YandexScheduleOptions> options)
        {
            _config = options.Value;
        }
    }
}