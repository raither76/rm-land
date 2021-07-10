namespace Rmis.Yandex.Schedule
{
    public class YandexScheduleOptions
    {
        public string ApiKey { get; init; }

        public string ScheduleUri { get; init; }

        public int Limit { get; init; }

        public int ScheduledDaysCount { get; init; }

        public string RouteNumberFilteringRegExp { get; init; }
    }
}