using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rmis.Client.Application.Abstract;
using Rmis.Client.Domain;

namespace Rmis.Client.WepApi.Controllers
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

        [HttpPost("/SyncSchedules")]
        public IActionResult SyncSchedulesFromHub()
        {
            try
            {
                _scheduleService.SyncSchedulesFromHub();
                return this.Ok("Расписание синхоринизировано");
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return this.BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IActionResult GetCurrentSchedule()
        {
            try
            {
                Schedule result = _scheduleService.GetCurrentSchedule();
                return this.Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return this.BadRequest(e.Message);
            }
        }
    }
}