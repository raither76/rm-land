using System;
using System.Collections.Generic;

namespace Rmis.Yandex.Schedule.Abstract
{
    public interface IYandexScheduleProvider
    {
        List<YandexSchedule> GetSchedules(string fromYaStationCode, string toYaStationCode, DateTimeOffset fromDate, DateTimeOffset toData);
    }
}