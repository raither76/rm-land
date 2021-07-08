using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rmis.Application.Abstract;

namespace Rmis.WebApi.Controllers
{
    [ApiController]
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

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var result = _scheduleService.GetSchedules();
                return this.Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error");
                return this.BadRequest(e);
            }
        }
    }
}