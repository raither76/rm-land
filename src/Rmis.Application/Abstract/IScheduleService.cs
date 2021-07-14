using System.Collections.Generic;
using Rmis.Domain;
using Rmis.Yandex.Schedule;

namespace Rmis.Application.Abstract
{
    public interface IScheduleService
    {
        void SyncSchedulesFromYandex();

        void SyncSchedulesFromGoogle();

        IEnumerable<ScheduleVm> GetSchedulesByRouteNumber(string trainNumber);
    }
}