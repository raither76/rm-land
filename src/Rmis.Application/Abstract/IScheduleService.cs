using System.Collections.Generic;
using Rmis.Domain;
using Rmis.Yandex.Schedule;

namespace Rmis.Application.Abstract
{
    public interface IScheduleService
    {
        void SyncSchedulesFromYandex();

        void SyncSchedulesFromGoogle();

        IEnumerable<ScheduleVm> GetSchedulesByTrainNumber(string trainNumber);

        void SaveTrackInfo(TrackInfoDto trackInfoDto);

        TrackInfoDto GetLastTrackInfo(string trainNumber);
    }
}