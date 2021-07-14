using System;

namespace Rmis.Client.Domain
{
    public class Schedule : BaseEntity
    {
        public DateTime Date { get; set; }

        public DateTimeOffset DepartureDate { get; set; }

        public DateTimeOffset ArrivalDate { get; set; }

        public string TrainNumber { get; set; }

        public string TrainDriver { get; set; }

        public string RouteNumber { get; set; }

        public string From { get; set; }

        public string To { get; set; }
        
        public string GetKey()
        {
            return $"{this.TrainNumber}_{this.Date:dd.MM.yyyy}";
        }
    }
}