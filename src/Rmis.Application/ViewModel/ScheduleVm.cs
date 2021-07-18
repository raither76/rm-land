using System;
using System.Collections.Generic;
using System.Linq;
using Rmis.Domain;

namespace Rmis.Application
{
    public class ScheduleVm
    {
        public DateTimeOffset DepartureDate { get; set; }

        public DateTimeOffset ArrivalDate { get; set; }

        public string RouteNumber { get; set; }

        public string TrainNumber { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public DateTime Date { get; set; }
        
        public string TrainDriver { get; set; }

        public List<StopVm> Stops { get; set; }

        public static ScheduleVm CreateFrom(Schedule schedule)
        {
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule));

            return new ScheduleVm
            {
                Date = schedule.Date,
                ArrivalDate = schedule.ArrivalDate,
                DepartureDate = schedule.DepartureDate,
                RouteNumber = schedule.Route?.Number.ToString(),
                TrainNumber = schedule.TrainNumber,
                TrainDriver = schedule.TrainDriver,
                From = schedule.Route?.Direction?.FromStation?.DisplayName,
                To = schedule.Route?.Direction?.ToStation?.DisplayName,
                Stops = schedule.Route?.Stops.Select(StopVm.CreateFrom).ToList()
            };
        }
    }
}