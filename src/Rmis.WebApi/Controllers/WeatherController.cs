using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rmis.Application;
using Rmis.Application.Abstract;

namespace Rmis.WebApi.Controllers
{
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly ILogger<WeatherController> _logger;
        private readonly IWeatherService _weatherService;

        public WeatherController(ILogger<WeatherController> logger, IWeatherService weatherService)
        {
            _logger = logger;
            _weatherService = weatherService;
        }
        
        [HttpPost("/Weather")]
        public IActionResult SyncWeatherInfo()
        {
            try
            {
                _weatherService.SyncWeather();
                return this.Ok("Данные о погоде обновлены");
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpGet("/Weather")]
        public IActionResult GetWeaterByCoords(double latitude, double longitude)
        {
            try
            {
                WeatherInfo result = _weatherService.GetWeatherByCoords(latitude, longitude);
                return this.Ok(result);
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
    }
}