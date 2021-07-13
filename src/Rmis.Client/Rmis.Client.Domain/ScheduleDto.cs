using System;

namespace Rmis.Client.Domain
{
    public class ScheduleDto
    {
        public DateTimeOffset DepartureDate { get; set; }

        public DateTimeOffset ArrivalDate { get; set; }

        public string RouteNumber { get; set; }

        public string TrainNumber { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public DateTime Date { get; set; }

        public string GetKey()
        {
            return $"{this.RouteNumber}_{this.Date:dd.MM.yyyy}";
        }
    }
}