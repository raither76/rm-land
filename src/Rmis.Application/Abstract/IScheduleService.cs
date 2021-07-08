using System.Collections.Generic;
using Rmis.Yandex.Schedule;

namespace Rmis.Application.Abstract
{
    public interface IScheduleService
    {
        List<YandexSchedule> GetSchedules();
    }
}