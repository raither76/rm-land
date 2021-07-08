using System;

namespace Rmis.Domain
{
    public class Schedule : BaseEntity
    {
        public DateTimeOffset DepartureDate { get; set; }
        
        public DateTimeOffset ArrivalDate { get; set; }

        public Route Route { get; set; }
    }
}