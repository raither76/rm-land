using System;
using Microsoft.Extensions.Logging;
using Rmis.Application.Abstract;
using Rmis.Domain;
using Rmis.OpenWeather;
using Rmis.Persistence.Abstract;

namespace Rmis.Application
{
    internal class WeatherService : IWeatherService
    {
        private readonly IRmisDbContext _context;
        private readonly ILogger<WeatherService> _logger;
        private readonly IOpenWeatherProvider _weatherProvider;

        public WeatherService(IRmisDbContext context, ILogger<WeatherService> logger, IOpenWeatherProvider weatherProvider)
        {
            _context = context;
            _logger = logger;
            _weatherProvider = weatherProvider;
        }

        public void SyncWeather()
        {
            try
            {
                foreach (Station station in _context.StationRepository)
                {
                    WeatherResponse weatherResponse = _weatherProvider.GetWeatherInfoByGeo(station.Latitude, station.Longitude);
                    if (weatherResponse != null)
                    {
                        station.TemperatureC = weatherResponse.main.temp;
                        station.WindSpeed = weatherResponse.wind.speed;
                        station.WindDirectionDeg = weatherResponse.wind.deg;
                        station.WeatherDescription = weatherResponse.weather[0].main;
                    }
                }

                _context.SaveChanges();
            }
            catch (Exception e)
            {
                string message = $"Ошибка при получении текущей информации о погоде";
                _logger.LogError(e, message);
                throw new Exception(message, e);
            }
        }

        public WeatherInfo GetWeatherByCoords(double latitude, double longitude)
        {
            try
            {
                WeatherResponse weatherResponse = _weatherProvider.GetWeatherInfoByGeo(latitude, longitude);
                WeatherInfo result = new()
                {
                    Temperature = weatherResponse.main.temp,
                    WeatherDescription = weatherResponse.weather[0].main,
                    WindSpeed = weatherResponse.wind.speed,
                    WindDirectionDeg = weatherResponse.wind.deg
                };

                return result;
            }
            catch (Exception e)
            {
                string message = $"Ошибка при получении текущей информации о погоде по заданным координатам";
                _logger.LogError(e, message);
                throw new Exception(message, e);
            }
        }
    }
}