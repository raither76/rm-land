using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rmis.Application;
using Rmis.Application.Abstract;

namespace Rmis.WebApi.Controllers
{
    [Route("[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly ILogger<ScheduleController> _logger;
        private readonly IScheduleService _scheduleService;

        public ScheduleController(ILogger<ScheduleController> logger, IScheduleService scheduleService)
        {
            _logger = logger;
            _scheduleService = scheduleService;
        }

        [HttpPost("SyncYandex")]
        public IActionResult SyncYandex()
        {
            try
            {
                _scheduleService.SyncSchedulesFromYandex();
                return this.Ok("Расписание синхронизировано");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error");
                return this.BadRequest(e.Message);
            }
        }

        [HttpPost("SyncGoogle")]
        public IActionResult SyncGoogle()
        {
            try
            {
                _scheduleService.SyncSchedulesFromGoogle();
                return this.Ok("Расписание синхронизировано");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error");
                return this.BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IActionResult GetSchedules(string trainNumber)
        {
            try
            {
                return this.Ok(_scheduleService.GetSchedulesByTrainNumber(trainNumber));
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return this.BadRequest(e.Message);
            }
        }

        [HttpPost("/TrackInfo")]
        public IActionResult SaveTrackInfo([FromBody] TrackInfoDto trackInfo)
        {
            try
            {
                _scheduleService.SaveTrackInfo(trackInfo);
                
                return this.Ok("Данные о местоположении поезда успешно обработаны");
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpGet("/TrackInfo")]
        public IActionResult GetLastTrackInfo(string trainNumber)
        {
            try
            {
                TrackInfoDto trackInfo = _scheduleService.GetLastTrackInfo(trainNumber);
                if (trackInfo == null)
                    return this.NotFound("Данные о текущем положении поезда отстутствуют");
                
                return this.Ok(trackInfo);
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }

        [HttpPost("[action]")]
        public IActionResult SyncWeatherInfo()
        {
            try
            {
                _scheduleService.SyncWeatherInfo();
                return this.Ok("Данные о погоде обновлены");
            }
            catch (Exception e)
            {
                return this.BadRequest(e.Message);
            }
        }
    }
}