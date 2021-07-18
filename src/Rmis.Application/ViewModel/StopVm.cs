using System;
using Rmis.Domain;

namespace Rmis.Application
{
    public class StopVm
    {
        public double Duration { get; set; }

        public double? StopTime { get; set; }

        public string DisplayName { get; set; }

        public string Code { get; set; }

        public static StopVm CreateFrom(Stop stop, Route route)
        {
            if (stop == null)
                throw new ArgumentNullException(nameof(stop));

            if (route == null)
                throw new ArgumentNullException(nameof(route));

            return new()
            {
                Duration = stop.Duration,
                StopTime = stop.StopTime,
                DisplayName = stop.Station?.DisplayName,
                Code = $"{route.Number}_{stop.Station?.YaCode}"
            };
        }
    }
}