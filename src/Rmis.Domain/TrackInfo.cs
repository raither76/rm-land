using System;

namespace Rmis.Domain
{
    public class TrackInfo : BaseEntity
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double? Speed { get; set; }

        public string TrainNumber { get; set; }

        public DateTime Date { get; set; }
    }
}