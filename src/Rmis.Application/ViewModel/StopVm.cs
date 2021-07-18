using System;
using Rmis.Domain;

namespace Rmis.Application
{
    public class StopVm
    {
        public double Duration { get; set; }

        public double? StopTime { get; set; }

        public string DisplayName { get; set; }

        public static StopVm CreateFrom(Stop stop)
        {
            if (stop == null)
                throw new ArgumentNullException(nameof(stop));

            return new()
            {
                Duration = stop.Duration,
                StopTime = stop.StopTime,
                DisplayName = stop.Station?.DisplayName
            };
        }
    }
}