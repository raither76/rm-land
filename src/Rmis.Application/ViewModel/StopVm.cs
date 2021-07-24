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

        public DateTimeOffset? DepartureDate { get; set; }
        
        public DateTimeOffset? ArrivalDate { get; set; }
        
        public decimal TemperatureC { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public decimal WindSpeed { get; set; }

        public int WindDirectionDeg { get; set; }

        public string WeatherDescription { get; set; }

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
                DepartureDate = stop.DepartureDate,
                ArrivalDate = stop.ArrivalDate,
                Code = $"{route.Number}_{stop.Station?.YaCode}",
                TemperatureC = stop.Station.TemperatureC,
                Latitude = stop.Station.Latitude,
                Longitude = stop.Station.Longitude,
                WindSpeed = stop.Station.WindSpeed,
                WindDirectionDeg = stop.Station.WindDirectionDeg,
                WeatherDescription = stop.Station.WeatherDescription
            };
        }
    }
}