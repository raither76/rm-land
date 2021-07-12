using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Rmis.Client.WepApi.Controllers
{
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetSchedules()
        {
            try
            {
                
                
                return this.Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return this.BadRequest(e.Message);
            }
        }
    }
}