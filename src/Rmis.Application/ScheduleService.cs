﻿using System;
using Rmis.Application.Abstract;
using Rmis.Persistence.Abstract;
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
    }
}