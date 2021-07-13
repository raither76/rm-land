using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rmis.Client.Application.Abstract;

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

        [HttpPost]
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
    }
}