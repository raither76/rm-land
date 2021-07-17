using System;
using System.Collections.Generic;

namespace Rmis.Yandex.Schedule.Abstract
{
    public interface IYandexScheduleProvider
    {
        IEnumerable<YandexSchedule> GetSchedules(string fromYaStationCode, string toYaStationCode, DateTime fromDate);

        YandexThread GetThread(string uid);
    }
}