using System;
using System.Collections.Generic;
using Rmis.Application.Abstract;
using Rmis.Persistence.Abstract;
using Rmis.Yandex.Schedule;
using Rmis.Yandex.Schedule.Abstract;

namespace Rmis.Application
{
    internal class ScheduleService : IScheduleService
    {
        private readonly IYandexScheduleProvider _scheduleProvider;
        private readonly IRmisDbContext _context;

        public ScheduleService(IYandexScheduleProvider scheduleProvider, IRmisDbContext context)
        {
            _scheduleProvider = scheduleProvider;
            _context = context;
        }

        public List<YandexSchedule> GetSchedules()
        {
            return _scheduleProvider.GetSchedules("s9602494", "s2006004", DateTimeOffset.Now, DateTimeOffset.Now);
        }
    }
}