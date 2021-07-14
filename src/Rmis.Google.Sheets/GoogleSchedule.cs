using System;

namespace Rmis.Google.Sheets
{
    public class GoogleSchedule
    {
        public DateTime Date { get; set; }

        public int Number { get; set; }

        public string TrainNumber { get; set; }

        public DateTimeOffset DepartureDate { get; set; }

        public DateTimeOffset ArrivalDate { get; set; }

        public string TrainDriver { get; set; }
        
        public string GetKey()
        {
            return $"{this.Number}_{this.Date:yyyy-MM-dd}";
        }
    }
}