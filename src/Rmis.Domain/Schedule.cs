using System;

namespace Rmis.Domain
{
    public class Schedule : BaseEntity
    {
        public DateTimeOffset DepartureDate { get; set; }
        
        public DateTimeOffset ArrivalDate { get; set; }

        public DateTime Date { get; set; }

        public Route Route { get; set; }

        public string TrainDriver { get; set; }

        public string TrainNumber { get; set; }

        /// <summary>
        /// Расписание синхронизировано с реестром.
        /// </summary>
        public bool IsSynchronized { get; set; }
    }
}